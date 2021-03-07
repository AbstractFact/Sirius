using Neo4jClient;
using Neo4jClient.Cypher;
using Sirius.DTOs;
using Sirius.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class DirectedService
    {
        private readonly IGraphClient _client;
        public DirectedService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<Object> GetSeriesWithDirector(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]->(s:Series)")
                       .Where("ID(s) = $seriesID")
                       .WithParam("seriesID", seriesID)
                       .Return((p, d, s) => new
                       {
                           ID = Return.As<int>("ID(d)"),
                           SeriesID = Return.As<int>("ID(s)"),
                           Title = s.As<Series>().Title,
                           Year = s.As<Series>().Year,
                           Genre = s.As<Series>().Genre,
                           Plot = s.As<Series>().Plot,
                           Seasons = s.As<Series>().Seasons,
                           Rating = s.As<Series>().Rating,
                           DirectorID = Return.As<int>("ID(p)"),
                           Name = p.As<Person>().Name,
                       })
                       .ResultsAsync;

                return res.FirstOrDefault();
            }
            catch (Exception)
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
                       .Where("ID(p) = $directorID")
                       .WithParam("directorID", directorID)
                       .Return((p, d, s) => new
                       {
                           SeriesID = Return.As<int>("ID(s)"),
                           Title = s.As<Series>().Title,
                           Year = s.As<Series>().Year,
                           Genre = s.As<Series>().Genre,
                           Plot = s.As<Series>().Plot,
                           Seasons = s.As<Series>().Seasons,
                           Rating = s.As<Series>().Rating,
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception)
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
                        .Where("ID(d) = $id")
                        .WithParam("id", id)
                        .Return((p, d, s) => new
                        {
                            ID = Return.As<int>("ID(d)"),
                            DirectorID = Return.As<int>("ID(p)"),
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
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> AddDirected(int directorID, int seriesID)
        {
            try
            {
                var res = _client.Cypher
                        .Match("(person:Person)", "(series:Series)")
                        .Where("ID(person) = $directorID")
                        .WithParam("directorID", directorID)
                        .AndWhere("ID(series) = $seriesID")
                        .WithParam("seriesID", seriesID)
                        .Create("(person)-[:DIRECTED]->(series)");

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
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
                        .Where("ID(d) = $id")
                        .WithParam("id", id)
                        .Set("d = $directed")
                        .WithParam("directed", directed);

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
                             .Match("(p:Person)-[d:DIRECTED]->(s:Series)")
                             .Where("ID(d) = $id")
                             .WithParam("id", id)
                             .Delete("d");

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
