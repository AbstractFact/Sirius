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
            try
            {
                var query = await _client.Cypher
                       .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                       .Return(r => r.As<Role>().ID)
                       .OrderByDescending("r.ID")
                       //.Return<int>("ID(r)")
                       //.OrderByDescending("ID(r)")
                       .ResultsAsync;

                return query.FirstOrDefault();
            }
            catch(Exception)
            {
                return -1;
            }  
        }

        public async Task<Object> GetSeriesRoles(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                       .Where((Series s) => s.ID == seriesID)
                       .Return((p, r, s) => new
                       {
                           r.As<Role>().ID,
                           Actor = p.As<Person>(),
                           Series = s.As<Series>(),
                           r.As<Role>().InRole
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
                       .Where((Person p) => p.ID == actorID)
                       .Return((p, r, s) => new
                       {
                           r.As<Role>().ID,
                           Actor = p.As<Person>(),
                           Series = s.As<Series>(),
                           r.As<Role>().InRole
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
                        .Where((Role r) => r.ID == id)
                        .Return((p, r, s) => new
                        {
                            r.As<Role>().ID,
                            Actor = p.As<Person>(),
                            Series = s.CollectAs<Series>(),
                            r.As<Role>().InRole
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
                maxID = await MaxID();

                if (maxID != -1)
                {
                    var res = _client.Cypher
                       .Match("(person:Person)", "(series:Series)")
                       .Where((Person person) => person.ID == actorID)
                       .AndWhere((Series series) => series.ID == seriesID)
                       .Create("(person)-[:IN_ROLE { ID: $id, InRole: $role }]->(series)")
                       .WithParam("role", role)
                       .WithParam("id", maxID + 1);

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

        public async Task<bool> Put(string role, int id)
        {
            try
            {
                var res = _client.Cypher
                        .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                        .Where((Role r) => r.ID == id)
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
                             .Where((Role r) => r.ID == id)
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
