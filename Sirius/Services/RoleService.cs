using Neo4jClient;
using Neo4jClient.Cypher;
using Sirius.DTOs;
using Sirius.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class RoleService
    {
        private readonly IGraphClient _client;
        public RoleService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<Object> GetSeriesRoles(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                       .Where("ID(s) = $seriesID")
                       .WithParam("seriesID", seriesID)
                       .Return((p, r, s) => new
                       {
                           ID=Return.As<int>("ID(r)"),
                           ActorID = Return.As<int>("ID(p)"),
                           Name = p.As<Person>().Name,
                           InRole = r.As<Role>().InRole
                       })
                       .ResultsAsync;

                return res;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public async Task<Object> GetActorRoles(int actorID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                       .Where("ID(p) = $actorID")
                       .WithParam("actorID", actorID)
                       .Return((p, r, s) => new
                       {
                           ID = Return.As<int>("ID(r)"),
                           SeriesID = Return.As<int>("ID(s)"),
                           Title = s.As<Series>().Title,
                           InRole = r.As<Role>().InRole
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }  
        }

        public async Task<Object> GetRole(int id)
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                        .Where("ID(r) = $id")
                        .WithParam("id", id)
                        .Return((p, r, s) => new
                        {
                            ID = Return.As<int>("ID(r)"),
                            ActorID = Return.As<int>("ID(p)"),
                            Name = p.As<Person>().Name,
                            Sex = p.As<Person>().Sex,
                            Birthplace = p.As<Person>().Birthplace,
                            Birthday = p.As<Person>().Birthday,
                            Biography = p.As<Person>().Biography,
                            SeriesID = Return.As<int>("ID(s)"),
                            Title = s.As<Series>().Title,
                            Year = s.As<Series>().Year,
                            Genre = s.As<Series>().Genre,
                            Plot = s.As<Series>().Plot,
                            Seasons = s.As<Series>().Seasons,
                            Rating = s.As<Series>().Rating,
                            InRole =  r.As<Role>().InRole
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }   
        }
        public async Task<bool> AddRole(int actorID, string role, int seriesID)
        {
            try
            {
                var res = _client.Cypher
                   .Match("(person:Person)", "(series:Series)")
                   .Where("ID(person) = $actorID")
                   .WithParam("actorID", actorID)
                   .AndWhere("ID(series) = $seriesID")
                   .WithParam("seriesID", seriesID)
                   .Create("(person)-[:IN_ROLE { InRole: $role }]->(series)")
                   .WithParam("role", role);

                    await res.ExecuteWithoutResultsAsync();

                    return true;               
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Put(string role, int id)
        {
            try
            {
                var res = _client.Cypher
                        .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                        .Where("ID(r) = $id")
                        .WithParam("id", id)
                        .Set("r.InRole = $role")
                        .WithParam("role", role);

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
                var res = _client.Cypher
                             .Match("(p:Person)-[r:IN_ROLE]->(s:Series)")
                             .Where("ID(r) = $id")
                             .WithParam("id", id)
                             .Delete("r");

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
