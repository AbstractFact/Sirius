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
                       .Return(() => new
                       {
                           ID = Return.As<int>("ID(d)"),
                           SeriesID = Return.As<int>("ID(s)"),
                           Title = Return.As<string>("s.Title"),
                           Year = Return.As<int>("s.Year"),
                           Genre = Return.As<string>("s.Genre"),
                           Plot = Return.As<string>("s.Plot"),
                           Seasons = Return.As<int>("s.Seasons"),
                           Rating = Return.As<float>("s.Rating"),
                           DirectorID = Return.As<int>("ID(p)"),
                           Name = Return.As<string>("p.Name"),
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
                           Title = Return.As<string>("s.Title"),
                           Year = Return.As<int>("s.Year"),
                           Genre = Return.As<string>("s.Genre"),
                           Plot = Return.As<string>("s.Plot"),
                           Seasons = Return.As<int>("s.Seasons"),
                           Rating = Return.As<float>("s.Rating")
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
                            Rating = Return.As<float>("s.Rating")
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

        public async Task<bool> Put(DirectedDTO directed, int id)
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
