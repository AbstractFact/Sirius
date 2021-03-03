using Neo4jClient;
using Sirius.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class DirectedService
    {
        private readonly IGraphClient _client;
        private int maxID;

        public DirectedService(IGraphClient client)
        {
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                        .Return(d => d.As<Directed>().ID)
                        .OrderByDescending("d.ID")
                        //.Return<int>("ID(d)")
                        //.OrderByDescending("ID(d)")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        public async Task<Object> GetSeriesWithDirector(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .Where((Series s) => s.ID == seriesID)
                       .Return((p, d, s) => new
                       {
                           d.As<Directed>().ID,
                           Director = p.As<Person>(),
                           Series = s.As<Series>(),
                       })
                       .ResultsAsync;

                return res.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Object> GetDirectorSeries(int directorID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .Where((Person p) => p.ID == directorID)
                       .Return((p, d, s) => new
                       {
                           d.As<Directed>().ID,
                           Director = p.As<Person>(),
                           Series = s.As<Series>(),
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Object> GetDirected(int id)
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                        .Where((Directed d) => d.ID == id)
                        .Return((p, d, s) => new
                        {
                            d.As<Directed>().ID,
                            Director = p.As<Person>(),
                            Series = s.CollectAs<Series>(),
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<bool> AddDirected(int directorID, int seriesID)
        {
            try
            {
                maxID = await MaxID();

                var res = _client.Cypher
                        .Match("(person:Person)", "(series:Series)")
                        .Where((Person person) => person.ID == directorID)
                        .AndWhere((Series series) => series.ID == seriesID)
                        .Create("(person)-[:DIRECTED]->(series)")
                        .WithParam("id", maxID + 1);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> Put(Directed directed, int id)
        {
            try
            {
                var res = _client.Cypher
                        .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                        .Where((Directed d) => d.ID == id)
                        .Set("d = $directed")
                        .WithParam("directed", directed);

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
                             .Match("(p:Person)-[d:DIRECTED]->(s:Series)")
                             .Where((Directed d) => d.ID == id)
                             .Delete("d");

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
