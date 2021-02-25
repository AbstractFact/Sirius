using Neo4jClient;
using Sirius.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class ActorService
    {
        private readonly IGraphClient _client;
        private int maxID;
        public ActorService(IGraphClient client)
        {
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            try
            {
                var query = await _client.Cypher
                        .Match("(a:Actor)")
                        .Return<int>(a => a.As<Actor>().ID)
                        .OrderByDescending("a.ID")
                        //.Return<int>("ID(s)")
                        //.OrderByDescending("ID(s)")
                        .ResultsAsync;

                return query.FirstOrDefault();
            }
            catch(Exception e)
            {
                return -1;
            }
            
        }

        public async Task<Object> GetAll()
        {
            try
            {
                var res = await _client.Cypher
                      .Match("(actor:Actor)")
                      .Return(actor => actor.As<Actor>())
                      .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Actor> GetActor(int actorID)
        {
            try
            {
                var res = await _client.Cypher
                     .Match("(a:Actor)")
                     .Where((Actor a) => a.ID == actorID)
                     .Return(a => a.As<Actor>())
                     .ResultsAsync;

              
                return res.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> Post(Actor a)
        {
            maxID = await MaxID();

            if (maxID != -1)
            {
                try
                {
                    var newActor = new Actor { ID = maxID + 1, Name = a.Name, Sex = a.Sex, Birthplace = a.Birthplace, Birthday = a.Birthday, Biography = a.Biography };
                    var res = _client.Cypher
                                .Create("(actor:Actor $newActor)")
                                .WithParam("newActor", newActor);

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

        public async Task<bool> Put(Actor actor, int id)
        {
            try
            {
                var res = _client.Cypher
                              .Match("(a:Actor)")
                              .Where((Actor a) => a.ID == id)
                              .Set("a = $actor")
                              .WithParam("actor", actor);

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
                             .Match("(a:Actor)")
                             .Where((Actor a) => a.ID == id)
                             .DetachDelete("a");

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
