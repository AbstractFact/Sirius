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

        public UserSeriesListService(IGraphClient client, IRedisService builder)
        {
            _client = client;
            _redisConnection = builder.Connection;
        }

        public async Task<Object> GetUserSeriesList(int userID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(u:User)-[l:LISTED]-(s:Series)")
                       .Where("ID(u) = $userID")
                       .WithParam("userID", userID)
                       .Return((l, s) => new
                       {
                           ID = Return.As<int>("ID(l)"),
                           Status = Return.As<string>("l.Status"),
                           Stars = Return.As<int>("l.Stars"),
                           Comment = Return.As<string>("l.Comment"),
                           Favourite = Return.As<bool>("l.Favourite"),
                           SeriesID = Return.As<int>("ID(s)"),
                           Title = Return.As<string>("s.Title"),
                           Genre = Return.As<string>("s.Genre"),
                           Seasons = Return.As<int>("s.Seasons"),
                           Rating = Return.As<float>("s.Rating")

                       })
                       .ResultsAsync;

                return res;

            }
            catch(Exception)
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
                        .Where("ID(u) = $userID")
                        .WithParam("userID", userID)
                        .AndWhere((UserSeriesList l) => l.Favourite == true)
                        .Return((s, u) => new
                        {
                            SeriesID = Return.As<int>("ID(s)"),
                            Title = Return.As<string>("s.Title"),
                            Genre = Return.As<string>("s.Genre"),
                            Seasons = Return.As<int>("s.Seasons"),
                            Rating = Return.As<float>("s.Rating"),
                            Username = Return.As<string>("u.Username")
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception)
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
                       .Where("ID(user) = $userID")
                       .WithParam("userID", userID)
                       .Return((friend) => new UserDTO
                       {
                            ID = Return.As<int>("ID(friend)"),
                            Name = Return.As<string>("friend.Name"),
                            Email = Return.As<string>("friend.Email"),
                            Username = Return.As<string>("friend.Username"),
                            Password = Return.As<string>("friend.Password")
                       })
                       .ResultsAsync;

                var friends = frs.ToList();
                List<Object> res = new List<Object>();
                foreach (UserDTO fr in friends)
                {
                    if (fr != null)
                    {
                        var rs = await _client.Cypher
                            .Match("(u:User)-[l:LISTED]-(s:Series)")
                            .Where("ID(u) = $frID")
                            .WithParam("frID", fr.ID)
                            .AndWhere((UserSeriesList l) => l.Favourite == true)
                            .Return((s, u) => new
                            {
                                SeriesID = Return.As<int>("ID(s)"),
                                Title = Return.As<string>("s.Title"),
                                Genre = Return.As<string>("s.Genre"),
                                Seasons = Return.As<int>("s.Seasons"),
                                Rating = Return.As<float>("s.Rating"),
                                Username = Return.As<string>("u.Username")
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
            catch (Exception)
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
                      .Where("ID(s) = $seriesID")
                      .WithParam("seriesID", seriesID)
                      .Return(s => new
                      {
                          Popularity = All.Count()
                      })
                      .ResultsAsync;

                return res;
            }
            catch (Exception)
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
                    var result = redisDB.SortedSetRangeByRank(key: "most:popular:series", start: 0, stop: -1, order: Order.Descending);
                    await redisDB.KeyExpireAsync("most:popular:series", new TimeSpan(0,0,15));

                    foreach (var res in result)
                        arr.Add(JsonSerializer.Deserialize<TopPopularDTO>(res));
                }
                else
                {
                    var res = await _client.Cypher
                          .Match("(u:User)-[l:LISTED]-(s:Series)")
                          .Return((s, l) => new TopPopularDTO
                          {
                              SeriesID = Return.As<int>("ID(s)"),
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
            catch (Exception)
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
                       .Where("ID(l) = $id")
                       .WithParam("id", id)
                       .Return((u, l, s) => new
                       {
                           ID = Return.As<int>("ID(l)"),
                           User = u.As<User>(),
                           Series = s.CollectAs<Series>(),
                           l.As<UserSeriesListDTO>().Status,
                           l.As<UserSeriesListDTO>().Stars,
                           l.As<UserSeriesListDTO>().Comment,
                           l.As<UserSeriesListDTO>().Favourite
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception)
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
                        .Where("ID(s) = $seriesID")
                        .WithParam("seriesID", seriesID)
                        .AndWhere("ID(u) = $userID")
                        .WithParam("userID", userID)
                        .Return((l) => new
                        {
                            ID = Return.As<int>("ID(l)")
                        })
                        .ResultsAsync;

                if (res.Count() != 0)
                    return res.FirstOrDefault().ID;
                else
                    return -1;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        public async Task<bool> AddSeriesToList(string status, bool fav, int userID, int seriesID)
        {
            try
            {
                int tmp = await GetListedSeries(userID, seriesID);

                if (tmp == -1)
                {
                    var res = _client.Cypher
                            .Match("(user:User)", "(series:Series)")
                            .Where("ID(series) = $seriesID")
                            .WithParam("seriesID", seriesID)
                            .AndWhere("ID(user) = $userID")
                            .WithParam("userID", userID)
                            .Create("(user)-[:LISTED {Status: $status, Stars: 0, Comment: \"\", Favourite: $fav}]->(series)")
                            .WithParam("status", status)
                            .WithParam("fav", fav);

                    await res.ExecuteWithoutResultsAsync();

                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
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
                       .Where("ID(l) = $id")
                       .WithParam("id", id)
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
            catch (Exception)
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
                               .Where("ID(s) = $id")
                               .WithParam("id", id)
                               .Set("s.Rating = $rating")
                               .WithParam("rating", rating);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
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
                           .Where("ID(s) = $seriesID")
                           .WithParam("seriesID", seriesID)
                           .AndWhere((UserSeriesList l) => l.Stars != 0)
                           .Return((l) => Return.As<string>("avg(l.Stars)"))
                           .ResultsAsync;

                if (res.FirstOrDefault() != "null")
                    return float.Parse(res.FirstOrDefault());
                else
                    return 0;
            }
            catch (Exception)
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
                              .Where("ID(l) = $id")
                              .WithParam("id", id)
                              .Delete("l");

                await res.ExecuteWithoutResultsAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<UserSeriesListFilteredDTO>> GetUserSeriesFiltered(int userID, UserSeriesListFilterDTO filter)
        {
            try
            {
                var res = new List<UserSeriesListFilteredDTO>();
                if (filter.Status != "All" && filter.Favourite)
                {
                    res = (List<UserSeriesListFilteredDTO>)await _client.Cypher
                       .Match("(u:User)-[l:LISTED]->(s:Series)")
                       .Where("ID(u) = $userID")
                       .WithParam("userID", userID)
                       .AndWhere((UserSeriesList l) => l.Status == filter.Status)
                       .AndWhere((UserSeriesList l) => l.Favourite == filter.Favourite)
                       .Return((u, l, s) => new UserSeriesListFilteredDTO
                       {
                           ID = Return.As<int>("ID(l)"),
                           SeriesID = Return.As<int>("ID(s)"),
                           SeriesTitle = s.As<Series>().Title,
                           SeriesGenre = s.As<Series>().Genre,
                           SeriesSeasons = s.As<Series>().Seasons,
                           SeriesRating = s.As<Series>().Rating,
                           Status = l.As<UserSeriesList>().Status,
                           Stars = l.As<UserSeriesList>().Stars,
                           Comment = l.As<UserSeriesList>().Comment,
                           Favourite = l.As<UserSeriesList>().Favourite
                       })
                       .ResultsAsync;
                }
                else if (filter.Status != "All" && !filter.Favourite)
                {
                    res = (List<UserSeriesListFilteredDTO>)await _client.Cypher
                       .Match("(u:User)-[l:LISTED]->(s:Series)")
                       .Where("ID(u) = $userID")
                       .WithParam("userID", userID)
                       .AndWhere((UserSeriesList l) => l.Status == filter.Status)
                       .Return((u, l, s) => new UserSeriesListFilteredDTO
                       {
                           ID = Return.As<int>("ID(l)"),
                           SeriesID = Return.As<int>("ID(s)"),
                           SeriesTitle = s.As<Series>().Title,
                           SeriesGenre = s.As<Series>().Genre,
                           SeriesSeasons = s.As<Series>().Seasons,
                           SeriesRating = s.As<Series>().Rating,
                           Status = l.As<UserSeriesList>().Status,
                           Stars = l.As<UserSeriesList>().Stars,
                           Comment = l.As<UserSeriesList>().Comment,
                           Favourite = l.As<UserSeriesList>().Favourite
                       })
                       .ResultsAsync;
                }
                else if (filter.Status == "All" && filter.Favourite)
                {
                    res = (List<UserSeriesListFilteredDTO>)await _client.Cypher
                       .Match("(u:User)-[l:LISTED]->(s:Series)")
                       .Where("ID(u) = $userID")
                       .WithParam("userID", userID)
                       .AndWhere((UserSeriesList l) => l.Favourite == filter.Favourite)
                       .Return((u, l, s) => new UserSeriesListFilteredDTO
                       {
                           ID = Return.As<int>("ID(l)"),
                           SeriesID = Return.As<int>("ID(s)"),
                           SeriesTitle = s.As<Series>().Title,
                           SeriesGenre = s.As<Series>().Genre,
                           SeriesSeasons = s.As<Series>().Seasons,
                           SeriesRating = s.As<Series>().Rating,
                           Status = l.As<UserSeriesList>().Status,
                           Stars = l.As<UserSeriesList>().Stars,
                           Comment = l.As<UserSeriesList>().Comment,
                           Favourite = l.As<UserSeriesList>().Favourite
                       })
                       .ResultsAsync;
                }
                else
                {
                    res = (List<UserSeriesListFilteredDTO>)await _client.Cypher
                       .Match("(u:User)-[l:LISTED]->(s:Series)")
                       .Where("ID(u) = $userID")
                       .WithParam("userID", userID)
                       .Return((u, l, s) => new UserSeriesListFilteredDTO
                       {
                           ID = Return.As<int>("ID(l)"),
                           SeriesID = Return.As<int>("ID(s)"),
                           SeriesTitle = s.As<Series>().Title,
                           SeriesGenre = s.As<Series>().Genre,
                           SeriesSeasons = s.As<Series>().Seasons,
                           SeriesRating = s.As<Series>().Rating,
                           Status = l.As<UserSeriesList>().Status,
                           Stars = l.As<UserSeriesList>().Stars,
                           Comment = l.As<UserSeriesList>().Comment,
                           Favourite = l.As<UserSeriesList>().Favourite
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
