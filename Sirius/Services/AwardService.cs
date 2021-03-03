using Neo4jClient;
using Sirius.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class AwardService
    {
        private readonly IGraphClient _client;
        private int maxID;
        public AwardService(IGraphClient client)
        {
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            try
            {
                var query = await _client.Cypher
                        .Match("(a:Award)")
                        .Return<int>(a => a.As<Award>().ID)
                        .OrderByDescending("a.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

                return query.FirstOrDefault();
            }
            catch (Exception e)
            {
                return -1;
            }

        }

        public async Task<Object> GetAll()
        {
            try
            {
                var res = await _client.Cypher
                      .Match("(award:Award)")
                      .Return(award => award.As<Award>())
                      .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Award> GetAward(int awardID)
        {
            try
            {
                var res = await _client.Cypher
                     .Match("(a:Award)")
                     .Where((Award a) => a.ID == awardID)
                     .Return(a => a.As<Award>())
                     .ResultsAsync;


                return res.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> Post(Award a)
        {
            maxID = await MaxID();

            if (maxID != -1)
            {
                try
                {
                    var newAward = new Award { ID = maxID + 1, Name = a.Name, Description = a.Description };
                    var res = _client.Cypher
                                .Create("(award:Award $newAward)")
                                .WithParam("newAward", newAward);

                    await res.ExecuteWithoutResultsAsync();

                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
                return false;

        }

        public async Task<bool> Put(Award award, int id)
        {
            try
            {
                var res = _client.Cypher
                              .Match("(a:Award)")
                              .Where((Award a) => a.ID == id)
                              .Set("a = $award")
                              .WithParam("award", award);

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
                             .Match("(a:Award)")
                             .Where((Award a) => a.ID == id)
                             .DetachDelete("a");

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<List<Award>> GetAwardsFiltered(string filter)
        {
            try
            {
                var res = new List<Award>();
                if (filter != "All")
                {
                    res = (List<Award>)await _client.Cypher
                     .Match("(a:Award)")
                     .Where((Award a) => a.Name.Contains(filter))
                     .Return(a => a.As<Award>())
                     .ResultsAsync;
                }
                else
                {
                    res = (List<Award>)await _client.Cypher
                     .Match("(a:Award)")
                     .Return(a => a.As<Award>())
                     .ResultsAsync;
                }

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
