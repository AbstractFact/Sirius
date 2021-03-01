using Neo4jClient;
using Neo4jClient.Cypher;
using Sirius.DTOs;
using Sirius.Entities;
using Sirius.Services.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class UserSeriesListService
    {
        private readonly IGraphClient _client;
        private readonly IConnectionMultiplexer _redisConnection;
        private int maxID;

        public UserSeriesListService(IGraphClient client, IRedisService builder)
        {
            _client = client;
            _redisConnection = builder.Connection;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            try
            {
                var query = await _client.Cypher
                       .Match("(u:User)-[l:LISTED]-(s:Series)")
                       .Return(l => l.As<UserSeriesList>().ID)
                       .OrderByDescending("l.ID")
                       //.Return<int>("ID(s)")
                       //.OrderByDescending("ID(s)")
                       .ResultsAsync;

                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public async Task<Object> GetUserSeriesList(int userID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(u:User)-[l:LISTED]-(s:Series)")
                       .Where((User u) => u.ID == userID)
                       .Return((u, l, s) => new
                       {
                           l.As<UserSeriesList>().ID,
                           Series = s.As<Series>(),
                           l.As<UserSeriesList>().Status,
                           l.As<UserSeriesList>().Stars,
                           l.As<UserSeriesList>().Comment,
                           l.As<UserSeriesList>().Favourite
                       })
                       .ResultsAsync;

                return res;

            }
            catch(Exception e)
            {
                return null;
            }    
        }

        public async Task<Object> GetUserFavourites(int userID)
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((User u) => u.ID == userID)
                        .AndWhere((UserSeriesList l) => l.Favourite == true)
                        .Return((s, u) => new
                        {
                            Series = s.As<Series>(),
                            u.As<User>().Username
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<Object>> GetRecommendations(int userID)
        {
            try
            {
                var frs = await _client.Cypher
                       .OptionalMatch("(user:User)-[FRIENDS]-(friend:User)")
                       .Where((User user) => user.ID == userID)
                       .Return(friend => friend.As<User>())
                       .ResultsAsync;

                var friends = frs.ToList();
                List<Object> res = new List<Object>();
                foreach (User fr in friends)
                {
                    if (fr != null)
                    {
                        var rs = await _client.Cypher
                            .Match("(u:User)-[l:LISTED]-(s:Series)")
                            .Where((User u) => u.ID == fr.ID)
                            .AndWhere((UserSeriesList l) => l.Favourite == true)
                            .Return((s, u) => new
                            {
                                Series = s.As<Series>(),
                                u.As<User>().Username
                            })
                            .ResultsAsync;

                        rs.ToList().ForEach(rec =>
                        {
                            res.Add(rec);
                        });
                    }
                };

                return res;
            }
            catch (Exception e)
            {
                return null;
            } 
        }

        public async Task<Object> GetSeriesPopularity(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                      .Match("(u:User)-[l:LISTED]-(s:Series)")
                      .Where((Series s) => s.ID == seriesID)
                      .Return(s => new
                      {
                          Popularity = All.Count()
                      })
                      .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
          
        }

        public async Task<List<TopPopularDTO>> GetMostPopularSeries()
        {
            try
            {
                IDatabase redisDB = _redisConnection.GetDatabase();

                List<TopPopularDTO> arr = new List<TopPopularDTO>();

                if (await redisDB.KeyExistsAsync("most:popular:series"))
                {
                    var result = redisDB.SortedSetScan("most:popular:series");
                    await redisDB.KeyExpireAsync("most:popular:series", new TimeSpan(0,0,15));

                    foreach (var res in result)
                        arr.Add(JsonSerializer.Deserialize<TopPopularDTO>(res.Element));

                    arr = arr.OrderByDescending(order => order.Popularity).ToList();
                }
                else
                {
                    var res = await _client.Cypher
                          .Match("(u:User)-[l:LISTED]-(s:Series)")
                          .Return((s, l) => new TopPopularDTO
                          {
                              SeriesID = s.As<Series>().ID,
                              Title = s.As<Series>().Title,
                              Year = s.As<Series>().Year,
                              Genre = s.As<Series>().Genre,
                              Rating = s.As<Series>().Rating,
                              Popularity = (int) l.Count()
                          })
                          .OrderBy("COUNT(l) DESC").Limit(10)
                          .ResultsAsync;


                    foreach (TopPopularDTO el in res)
                        await redisDB.SortedSetAddAsync("most:popular:series", JsonSerializer.Serialize(el), el.Popularity);

                    arr = res.ToList();
                }

                return arr;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Object> GetListed(int id)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(u:User)-[l:LISTED]-(s:Series)")
                       .Where((UserSeriesList l) => l.ID == id)
                       .Return((u, l, s) => new
                       {
                           l.As<UserSeriesList>().ID,
                           User = u.As<User>(),
                           Series = s.CollectAs<Series>(),
                           l.As<UserSeriesList>().Status,
                           l.As<UserSeriesList>().Stars,
                           l.As<UserSeriesList>().Comment,
                           l.As<UserSeriesList>().Favourite
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            } 
        }

        public async Task<int> GetListedSeries(int userID, int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(u:User)-[l:LISTED]-(s:Series)")
                        .Where((Series s) => s.ID == seriesID)
                        .AndWhere((User u) => u.ID == userID)
                        .Return((u, l, s) => new
                        {
                            l.As<UserSeriesList>().ID,
                        })
                        .ResultsAsync;

                if (res.Count() != 0)
                    return res.FirstOrDefault().ID;
                else
                    return -1;
            }
            catch (Exception e)
            {
                return -1;
            }

        }

        public async Task<bool> AddSeriesToList(string status, bool fav, int userID, int seriesID)
        {
            try
            {
                maxID = await MaxID();
                int tmp = await GetListedSeries(userID, seriesID);

                if (tmp == -1 && maxID!=-1)
                {
                    var res = _client.Cypher
                            .Match("(user:User)", "(series:Series)")
                            .Where((User user) => user.ID == userID)
                            .AndWhere((Series series) => series.ID == seriesID)
                            .Create("(user)-[:LISTED { ID: $id, Status: $status, Stars: 0, Comment: \"\", Favourite: $fav}]->(series)")
                            .WithParam("status", status)
                            .WithParam("fav", fav)
                            .WithParam("id", maxID + 1);

                    await res.ExecuteWithoutResultsAsync();

                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
           
        }

        public async Task<bool> Put(FavouriteSeriesEditDTO data, int id, int seriesID)
        {
            try
            {
                var res = _client.Cypher
                       .Match("(u:User)-[l:LISTED]-(s:Series)")
                       .Where((UserSeriesList l) => l.ID == id)
                       .Set("l.Status = $status")
                       .Set("l.Stars = $stars")
                       .Set("l.Comment = $comment")
                       .Set("l.Favourite = $favourite")
                       .WithParam("status", data.Status)
                       .WithParam("stars", data.Stars)
                       .WithParam("comment", data.Comment)
                       .WithParam("favourite", data.Favourite);

                await res.ExecuteWithoutResultsAsync();

                if (data.Stars != 0)
                {
                    float avgrtng = await GetSeriesAvgRating(seriesID);
                    if(avgrtng!=-1)
                        await UpdateRating(seriesID, avgrtng);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateRating(int id, float rating)
        {
            try
            {
                var res = _client.Cypher
                               .Match("(s:Series)")
                               .Where((Series s) => s.ID == id)
                               .Set("s.Rating = $rating")
                               .WithParam("rating", rating);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<float> GetSeriesAvgRating(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                           .Match("(u:User)-[l:LISTED]-(s:Series)")
                           .Where((Series s) => s.ID == seriesID)
                           .AndWhere((UserSeriesList l) => l.Stars != 0)
                           .Return((l) => Return.As<string>("avg(l.Stars)"))
                           .ResultsAsync;

                if (res.FirstOrDefault() != "null")
                    return float.Parse(res.FirstOrDefault());
                else
                    return 0;
            }
            catch (Exception e)
            {
                return -1;
            }

        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var res = _client.Cypher
                              .Match("(u:User)-[l:LISTED]->(s:Series)")
                              .Where((UserSeriesList l) => l.ID == id)
                              .Delete("l");

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
