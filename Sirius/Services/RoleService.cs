using Neo4jClient;
using Sirius.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class RoleService
    {
        private readonly IGraphClient _client;
        private int maxID;

        public RoleService(IGraphClient client)
        {
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Return(r => r.As<Role>().ID)
                        .OrderByDescending("r.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        public async Task<Object> GetSeriesRoles(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                       .Where((Series s) => s.ID == seriesID)
                       .Return((a, r, s) => new
                       {
                           r.As<Role>().ID,
                           Actor = a.As<Actor>(),
                           Series = s.As<Series>(),
                           r.As<Role>().InRole
                       })
                       .ResultsAsync;

                return res;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<Object> GetActorRoles(int actorID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                       .Where((Actor a) => a.ID == actorID)
                       .Return((a, r, s) => new
                       {
                           r.As<Role>().ID,
                           Actor = a.As<Actor>(),
                           Series = s.As<Series>(),
                           r.As<Role>().InRole
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }  
        }

        public async Task<Object> GetRole(int id)
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Where((Role r) => r.ID == id)
                        .Return((a, r, s) => new
                        {
                            r.As<Role>().ID,
                            Actor = a.As<Actor>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Role>().InRole
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }   
        }
        public async Task<bool> AddRole(int actorID, string role, int seriesID)
        {
            try
            {
                maxID = await MaxID();

                var res = _client.Cypher
                        .Match("(actor:Actor)", "(series:Series)")
                        .Where((Actor actor) => actor.ID == actorID)
                        .AndWhere((Series series) => series.ID == seriesID)
                        .Create("(actor)-[:IN_ROLE { ID: $id, InRole: $role }]->(series)")
                        .WithParam("role", role)
                        .WithParam("id", maxID + 1);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Put(string role, int id)
        {
            try
            {
                var res = _client.Cypher
                        .Match("(a:Actor)-[r:IN_ROLE]-(s:Series)")
                        .Where((Role r) => r.ID == id)
                        .Set("r.InRole = $role")
                        .WithParam("role", role);

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
                             .Match("(a:Actor)-[r:IN_ROLE]->(s:Series)")
                             .Where((Role r) => r.ID == id)
                             .Delete("r");

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
