using Microsoft.AspNetCore.SignalR;
using Neo4jClient;
using Sirius.DTOs;
using Sirius.Entities;
using Sirius.Hubs;
using Sirius.Services.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class SeriesService
    {
        private readonly IGraphClient _client;
        private int maxID;
        private readonly IHubContext<MessageHub> _hub;
        private readonly IConnectionMultiplexer _redisConnection;

        public SeriesService(IGraphClient client, IRedisService builder, IHubContext<MessageHub> hub)
        {
            _client = client;
            maxID = 0;
            _redisConnection = builder.Connection;
            _hub = hub;
        }

        private async Task<int> MaxID()
        {
            try
            {
                var query = await _client.Cypher
                        .Match("(s:Series)")
                        .Return<int>(s => s.As<Series>().ID)
                        .OrderByDescending("s.ID")
                        .ResultsAsync;

                return query.FirstOrDefault();
            }
            catch(Exception e)
            {
                return -1;
            }   
        }

        public async Task<Object> GetAll()
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(series:Series)")
                        .Return(series => series.As<Series>())
                        .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Series> GetSeries(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(s:Series)")
                       .Where((Series s) => s.ID == seriesID)
                       .Return(s => s.As<Series>())
                       .ResultsAsync;
              
               return res.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<int> Post(Series s)
        {
            maxID = await MaxID();

            if (maxID != -1)
            {
                try
                {
                    var newSeries = new Series { ID = maxID + 1, Title = s.Title, Year = s.Year, Genre = s.Genre, Plot = s.Plot, Seasons = s.Seasons, Rating = 0.0f };
                    var res = _client.Cypher
                                .Create("(series:Series $newSeries)")
                                .WithParam("newSeries", newSeries);

                    await res.ExecuteWithoutResultsAsync();

                    IDatabase redisDB = _redisConnection.GetDatabase();
                    var result = await redisDB.SetMembersAsync("genre:" + s.Genre + ":subsriber");

                    RecommendationDTO message = new RecommendationDTO
                    {
                        SeriesID = newSeries.ID,
                        Title = newSeries.Title,
                        Genre = newSeries.Genre
                    };

                    var msgForSet = JsonSerializer.Serialize(message);

                    foreach (var r in result)
                    {
                        await redisDB.SetAddAsync("user:" + Convert.ToInt32(r) + ":recommendations", msgForSet);
                    }

                    var jsonMessage = JsonSerializer.Serialize(message);
                    ISubscriber chatPubSub = _redisConnection.GetSubscriber();
                    await chatPubSub.PublishAsync("genre.recommendations", jsonMessage);

                    return newSeries.ID;

                }
                catch (Exception e)
                {
                    return -1;
                }

            }
            else return -1;
        }

        public async Task<bool> Put(Series series, int id)
        {
            try
            {
                var res = _client.Cypher
                                .Match("(s:Series)")
                                .Where((Series s) => s.ID == id)
                                .Set("s = $series")
                                .WithParam("series", series);

                await res.ExecuteWithoutResultsAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                Series s = await GetSeries(id);
                RecommendationDTO message = new RecommendationDTO
                {
                    SeriesID = s.ID,
                    Title = s.Title,
                    Genre = s.Genre
                };

                IDatabase redisDB = _redisConnection.GetDatabase();
                var result = await redisDB.SetMembersAsync("genre:" + s.Genre + ":subsriber");

                foreach (var r in result)
                {
                    await redisDB.SetRemoveAsync("user:" + Convert.ToInt32(r) + ":recommendations", JsonSerializer.Serialize(message));
                }

                var res = _client.Cypher
                             .Match("(s:Series)")
                             .Where((Series s) => s.ID == id)
                             .OptionalMatch("(s:Series)<-[r]-()")
                             .Where((Series s) => s.ID == id)
                             .Delete("r, s");

                await res.ExecuteWithoutResultsAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<TopRatedDTO>> GetBestRatedSeries()
        {
            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();

                List<TopRatedDTO> arr = new List<TopRatedDTO>();

                if (await redisDB.KeyExistsAsync("best:rated:series"))
                {
                    var result = redisDB.SortedSetRangeByRank(key: "best:rated:series", start: 0, stop: -1, order: Order.Descending);
                    await redisDB.KeyExpireAsync("best:rated:series", new TimeSpan(0, 0, 15));

                    foreach (var res in result)
                        arr.Add(JsonSerializer.Deserialize<TopRatedDTO>(res));
                }
                else
                {
                    var res = await _client.Cypher
                          .Match("(s:Series)")
                          .Where((Series s) => s.Rating > 0.0f)
                          .Return(s => new TopRatedDTO
                          {
                              SeriesID = s.As<Series>().ID,
                              Title = s.As<Series>().Title,
                              Year = s.As<Series>().Year,
                              Genre = s.As<Series>().Genre,
                              Rating = s.As<Series>().Rating
                          })
                          .OrderBy("s.Rating DESC").Limit(10)
                          .ResultsAsync;


                    foreach (TopRatedDTO el in res)
                        await redisDB.SortedSetAddAsync("best:rated:series", JsonSerializer.Serialize(el), el.Rating);

                    arr = res.ToList();
                }

                return arr;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<Series>> GetSeriesFiltered(SeriesFilterDTO filter)
        {
            try
            {
                var res=new List<Series>();
                if(filter.Title!="" && filter.Genre!="All")
                {
                    res = (List<Series>)await _client.Cypher
                    .Match("(s:Series)")
                    .Where((Series s) => s.Title == filter.Title)
                    .AndWhere((Series s) => s.Genre == filter.Genre)
                    .Return(s => s.As<Series>())
                    .ResultsAsync;
                }
                else if (filter.Title != "" && filter.Genre == "All")
                {
                    res = (List<Series>)await _client.Cypher
                   .Match("(s:Series)")
                   .Where((Series s) => s.Title == filter.Title)
                   .Return(s => s.As<Series>())
                   .ResultsAsync;
                }
                else if (filter.Title == "" && filter.Genre != "All")
                {
                    res = (List<Series>)await _client.Cypher
                   .Match("(s:Series)")
                   .Where((Series s) => s.Genre == filter.Genre)
                   .Return(s => s.As<Series>())
                   .ResultsAsync;
                }
                else
                {
                    res = (List<Series>)await _client.Cypher
                   .Match("(s:Series)")
                   .Return(s => s.As<Series>())
                   .ResultsAsync;
                }

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
