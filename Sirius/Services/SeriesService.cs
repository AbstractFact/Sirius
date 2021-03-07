using Microsoft.AspNetCore.SignalR;
using Neo4jClient;
using Neo4jClient.Cypher;
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
        private readonly IConnectionMultiplexer _redisConnection;

        public SeriesService(IGraphClient client, IRedisService builder)
        {
            _client = client;
            _redisConnection = builder.Connection;
        }

        public async Task<Object> GetAll()
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(series:Series)")
                        .Return((series) => new SeriesDTO
                        {
                             ID = Return.As<int>("ID(series)"),
                             Title = series.As<Series>().Title,
                             Year = series.As<Series>().Year,
                             Genre = series.As<Series>().Genre,
                             Plot = series.As<Series>().Plot,
                             Seasons = series.As<Series>().Seasons,
                             Rating = series.As<Series>().Rating
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<SeriesDTO> GetSeries(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(series:Series)")
                       .Where("ID(series) = $seriesID")
                       .WithParam("seriesID", seriesID)
                       .Return((series) => new SeriesDTO
                       {
                           ID = Return.As<int>("ID(series)"),
                           Title = series.As<Series>().Title,
                           Year = series.As<Series>().Year,
                           Genre = series.As<Series>().Genre,
                           Plot = series.As<Series>().Plot,
                           Seasons = series.As<Series>().Seasons,
                           Rating = series.As<Series>().Rating
                       })
                       .ResultsAsync;
              
               return res.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> Post(Series s)
        {
  
            try
            {
                var res = await _client.Cypher
                            .Create("(series:Series $s)")
                            .WithParam("s", s)
                            .Return((series) => new SeriesDTO
                            {
                                ID = Return.As<int>("ID(series)"),
                                Title = series.As<Series>().Title,
                                Year = series.As<Series>().Year,
                                Genre = series.As<Series>().Genre,
                                Plot = series.As<Series>().Plot,
                                Seasons = series.As<Series>().Seasons,
                                Rating = series.As<Series>().Rating
                            })
                            .ResultsAsync;

                SeriesDTO newS = res.Single();

                IDatabase redisDB = _redisConnection.GetDatabase();
                var result = await redisDB.SetMembersAsync("genre:" + s.Genre + ":subsriber");

                RecommendationDTO message = new RecommendationDTO
                {
                    SeriesID = newS.ID,
                    Title = newS.Title,
                    Genre = newS.Genre
                };

                var msgForSet = JsonSerializer.Serialize(message);

                foreach (var r in result)
                {
                    await redisDB.SetAddAsync("user:" + Convert.ToInt32(r) + ":recommendations", msgForSet);
                }

                var jsonMessage = JsonSerializer.Serialize(message);
                ISubscriber chatPubSub = _redisConnection.GetSubscriber();
                await chatPubSub.PublishAsync("genre.recommendations", jsonMessage);

                return newS.ID;

            }
            catch (Exception)
            {
                return -1;
            }
        }

        public async Task<bool> Put(Series series, int id)
        {
            try
            {
                var res = _client.Cypher
                                .Match("(s:Series)")
                                .Where("ID(s) = $id")
                                .WithParam("id", id)
                                .Set("s = $series")
                                .WithParam("series", series);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                SeriesDTO s = await GetSeries(id);
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
                             .Where("ID(s) = $id")
                             .WithParam("id", id)
                             .DetachDelete("s");

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
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
                          .Where((SeriesDTO s) => s.Rating > 0.0f)
                          .Return(s => new TopRatedDTO
                          {
                              SeriesID = Return.As<int>("ID(s)"),
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
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<SeriesDTO>> GetSeriesFiltered(SeriesFilterDTO filter)
        {
            try
            {
                var res=new List<SeriesDTO>();
                if(filter.Title!="" && filter.Genre!="All")
                {
                    res = (List<SeriesDTO>)await _client.Cypher
                    .Match("(series:Series)")
                    .Where((Series series) => series.Title.Contains(filter.Title))
                    .AndWhere((Series series) => series.Genre == filter.Genre)
                    .Return((series) => new SeriesDTO
                    {
                        ID = Return.As<int>("ID(series)"),
                        Title = series.As<Series>().Title,
                        Year = series.As<Series>().Year,
                        Genre = series.As<Series>().Genre,
                        Plot = series.As<Series>().Plot,
                        Seasons = series.As<Series>().Seasons,
                        Rating = series.As<Series>().Rating
                    })
                    .ResultsAsync;
                }
                else if (filter.Title != "" && filter.Genre == "All")
                {
                    res = (List<SeriesDTO>)await _client.Cypher
                    .Match("(series:Series)")
                    .Where((Series series) => series.Title.Contains(filter.Title))
                    .Return((series) => new SeriesDTO
                    {
                        ID = Return.As<int>("ID(series)"),
                        Title = series.As<Series>().Title,
                        Year = series.As<Series>().Year,
                        Genre = series.As<Series>().Genre,
                        Plot = series.As<Series>().Plot,
                        Seasons = series.As<Series>().Seasons,
                        Rating = series.As<Series>().Rating
                    })
                   .ResultsAsync;
                }
                else if (filter.Title == "" && filter.Genre != "All")
                {
                    res = (List<SeriesDTO>)await _client.Cypher
                    .Match("(series:Series)")
                    .Where((Series series) => series.Genre == filter.Genre)
                    .Return((series) => new SeriesDTO
                    {
                        ID = Return.As<int>("ID(series)"),
                        Title = series.As<Series>().Title,
                        Year = series.As<Series>().Year,
                        Genre = series.As<Series>().Genre,
                        Plot = series.As<Series>().Plot,
                        Seasons = series.As<Series>().Seasons,
                        Rating = series.As<Series>().Rating
                    })
                   .ResultsAsync;
                }
                else
                {
                    res = (List<SeriesDTO>)await _client.Cypher
                   .Match("(series:Series)")
                   .Return((series) => new SeriesDTO
                   {
                       ID = Return.As<int>("ID(series)"),
                       Title = series.As<Series>().Title,
                       Year = series.As<Series>().Year,
                       Genre = series.As<Series>().Genre,
                       Plot = series.As<Series>().Plot,
                       Seasons = series.As<Series>().Seasons,
                       Rating = series.As<Series>().Rating
                   })
                   .ResultsAsync;
                }

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
