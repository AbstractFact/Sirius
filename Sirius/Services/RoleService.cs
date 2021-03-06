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
                           Name = Return.As<string>("p.Name"),
                           r.As<RoleDTO>().InRole
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
                           r.As<RoleDTO>().ID,
                           SeriesID = Return.As<int>("ID(s)"),
                           Title = Return.As<string>("s.Title"),
                           r.As<RoleDTO>().InRole
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
                            Name = Return.As<string>("p.Name"),
                            Sex = Return.As<string>("p.Sex"),
                            Birthplace = Return.As<string>("p.Birthplace"),
                            Birthday = Return.As<string>("p.Birthday"),
                            Biography = Return.As<string>("p.Biography"),
                            SeriesID = Return.As<int>("ID(s)"),
                            Title = Return.As<string>("s.Title"),
                            Year = Return.As<int>("s.Year"),
                            Genre = Return.As<string>("s.Genre"),
                            Plot = Return.As<string>("s.Plot"),
                            Seasons = Return.As<int>("s.Seasons"),
                            Rating = Return.As<float>("s.Rating"),
                            r.As<RoleDTO>().InRole
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
