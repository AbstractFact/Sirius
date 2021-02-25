using Microsoft.AspNetCore.SignalR;
using Neo4jClient;
using Sirius.Entities;
using Sirius.Hubs;
using Sirius.Services.Redis;
using StackExchange.Redis;
using System;
using System.Linq;
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

        public async Task<bool> Post(Series s)
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





                    //string channelName = $"messages:{userID}:recommendations";
                    //var values = new NameValueEntry[]
                    //    {
                    //    new NameValueEntry("series_id", s.ID),
                    //    new NameValueEntry("series_title", s.Title)
                    //    };

                    //IDatabase redisDB = _redisConnection.GetDatabase();
                    //var messageId = await redisDB.StreamAddAsync(channelName, values);

                    //NewSeriesNotificationDTO message = new NewSeriesNotificationDTO
                    //{
                    //    ID = s.ID,
                    //    SeriesID = s.ID,
                    //    Title = s.Title,
                    //    Genre = s.Genre
                    //};


                    //var jsonMessage = JsonSerializer.Serialize(message);
                    //ISubscriber chatPubSub = _redisConnection.GetSubscriber();
                    //await chatPubSub.PublishAsync("genre.recommendations", jsonMessage);






                    return true;

                }
                catch (Exception e)
                {
                    return false;
                }

            }
            else return false;
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
    }
}
