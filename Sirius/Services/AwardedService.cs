using Neo4jClient;
using Sirius.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class AwardedService
    {
        private readonly IGraphClient _client;
        private int maxID;

        public AwardedService(IGraphClient client)
        {
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            var query = await _client.Cypher
                        .Match("(a:Award)-[aw:AWARDED]-(s:Series)")
                        .Return(aw => aw.As<Awarded>().ID)
                        .OrderByDescending("aw.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

            return query.FirstOrDefault();
        }

        public async Task<Object> GetSeriesAwards(int seriesID)
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(a:Award)-[aw:AWARDED]-(s:Series)")
                       .Where((Series s) => s.ID == seriesID)
                       .Return((a, aw, s) => new
                       {
                           aw.As<Awarded>().ID,
                           Award = a.As<Award>(),
                           Series = s.As<Series>(),
                           aw.As<Awarded>().Year
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception e)
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
                       .Where((Award a) => a.ID == awardID)
                       .Return((aw, s) => new
                       {
                           Series = s.As<Series>(),
                           Year = aw.As<Awarded>().Year
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception e)
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
                        .Where((Awarded aw) => aw.ID == id)
                        .Return((a, aw, s) => new
                        {
                            aw.As<Awarded>().ID,
                            Award = a.As<Award>(),
                            Series = s.CollectAs<Series>(),
                            aw.As<Awarded>().Year
                        })
                        .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task<bool> AddAwardSeries(int awardID, int year, int seriesID)
        {
            try
            {
                maxID = await MaxID();

                var res = _client.Cypher
                        .Match("(award:Award)", "(series:Series)")
                        .Where((Award award) => award.ID == awardID)
                        .AndWhere((Series series) => series.ID == seriesID)
                        .Create("(award)-[:AWARDED { ID: $id, Year: $year }]->(series)")
                        .WithParam("year", year)
                        .WithParam("id", maxID + 1);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception e)
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
                        .Where((Awarded aw) => aw.ID == id)
                        .Set("aw.Year = $year")
                        .WithParam("year", year);

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
                             .Match("(a:Award)-[aw:AWARDED]->(s:Series)")
                             .Where((Awarded aw) => aw.ID == id)
                             .Delete("aw");

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
