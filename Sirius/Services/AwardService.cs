using Neo4jClient;
using Neo4jClient.Cypher;
using Sirius.DTOs;
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
        public AwardService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<Object> GetAll()
        {
            try
            {
                var res = await _client.Cypher
                      .Match("(award:Award)")
                      .Return((award) => new AwardDTO
                      {
                           ID = Return.As<int>("ID(award)"),
                           Name = award.As<Award>().Name,
                           Description = award.As<Award>().Description
                      })
                      .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<AwardDTO> GetAward(int awardID)
        {
            try
            {
                var res = await _client.Cypher
                     .Match("(a:Award)")
                     .Where("ID(a) = $awardID")
                     .WithParam("awardID", awardID)
                     .Return((a) => new AwardDTO
                     {
                          ID = Return.As<int>("ID(a)"),
                          Name = a.As<Award>().Name,
                          Description = a.As<Award>().Description
                     })
                     .ResultsAsync;


                return res.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Post(Award a)
        {
            try
            {
                var res = _client.Cypher
                                .Create("(award:Award $a)")
                                .WithParam("a", a);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Put(AwardDTO award, int id)
        {
            try
            {
                var res = _client.Cypher
                              .Match("(a:Award)")
                              .Where("ID(a) = $id")
                              .WithParam("id", id)
                              .Set("a = $award")
                              .WithParam("award", award);

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
                             .Match("(a:Award)")
                             .Where("ID(a) = $id")
                             .WithParam("id", id)
                             .DetachDelete("a");

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<AwardDTO>> GetAwardsFiltered(string filter)
        {
            try
            {
                var res = new List<AwardDTO>();
                if (filter != "All")
                {
                    res = (List<AwardDTO>)await _client.Cypher
                     .Match("(a:Award)")
                     .Where((AwardDTO a) => a.Name.Contains(filter))
                     .Return((a) => new AwardDTO
                     {
                         ID = Return.As<int>("ID(a)"),
                         Name = a.As<Award>().Name,
                         Description = a.As<Award>().Description
                     })
                     .ResultsAsync;
                }
                else
                {
                    res = (List<AwardDTO>)await _client.Cypher
                     .Match("(a:Award)")
                     .Return((a) => new AwardDTO
                     {
                         ID = Return.As<int>("ID(a)"),
                         Name = a.As<Award>().Name,
                         Description = a.As<Award>().Description
                     })
                     .ResultsAsync;
                }

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
