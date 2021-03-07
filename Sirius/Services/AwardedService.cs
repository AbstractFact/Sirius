using Neo4jClient;
using Neo4jClient.Cypher;
using Sirius.DTOs;
using Sirius.Entities;
using System;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class AwardedService
    {
        private readonly IGraphClient _client;

        public AwardedService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<Object> GetSeriesAwards(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(a:Award)-[aw:AWARDED]-(s:Series)")
                       .Where("ID(s) = $seriesID")
                       .WithParam("seriesID", seriesID)
                       .Return((a, aw, s) => new
                       {
                           ID = Return.As<int>("ID(aw)"),
                           AwardID = Return.As<int>("ID(a)"),
                           Name = a.As<Award>().Name,
                           Year = aw.As<Awarded>().Year
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Object> GetAwardedSeries(int awardID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(a:Award)-[aw:AWARDED]-(s:Series)")
                       .Where("ID(a) = $awardID")
                       .WithParam("awardID", awardID)
                       .Return((aw, s) => new
                       {
                           ID = Return.As<int>("ID(s)"),
                           Title = s.As<Series>().Title,
                           Year = aw.As<Awarded>().Year
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Object> GetAwarded(int id)
        {
            try
            {
                var res = await _client.Cypher
                        .Match("(a:Award)-[aw:AWARDED]-(s:Series)")
                        .Where("ID(aw) = $id")
                        .WithParam("id", id)
                        .Return((a, aw, s) => new
                        {
                            ID = Return.As<int>("ID(aw)"),
                            AwardID = Return.As<int>("ID(a)"),
                            Name = a.As<Award>().Name,
                            Description = a.As<Award>().Description,
                            SeriesID = Return.As<int>("ID(s)"),
                            Title = s.As<Series>().Title,
                            SeriesYear = s.As<Series>().Year,
                            Genre = s.As<Series>().Genre,
                            Plot = s.As<Series>().Plot,
                            Seasons = s.As<Series>().Seasons,
                            Rating = s.As<Series>().Rating,
                            aw.As<Awarded>().Year
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<bool> AddAwardSeries(int awardID, int year, int seriesID)
        {
            try
            {
                var res = _client.Cypher
                   .Match("(award:Award)", "(series:Series)")
                   .Where("ID(award) = $awardID")
                   .WithParam("awardID", awardID)
                   .AndWhere("ID(series) = $seriesID")
                   .WithParam("seriesID", seriesID)
                   .Create("(award)-[:AWARDED { Year: $year }]->(series)")
                   .WithParam("year", year);

                await res.ExecuteWithoutResultsAsync();

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Put(int year, int id)
        {
            try
            {
                var res = _client.Cypher
                        .Match("(a:Award)-[aw:AWARDED]-(s:Series)")
                        .Where("ID(aw) = $id")
                        .WithParam("id", id)
                        .Set("aw.Year = $year")
                        .WithParam("year", year);

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
                             .Match("(a:Award)-[aw:AWARDED]->(s:Series)")
                             .Where("ID(aw) = $id")
                             .WithParam("id", id)
                             .Delete("aw");

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
