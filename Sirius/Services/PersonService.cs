using Neo4jClient;
using Sirius.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sirius.Services
{
    public class PersonService
    {
        private readonly IGraphClient _client;
        private int maxID;
        public PersonService(IGraphClient client)
        {
            _client = client;
            maxID = 0;
        }

        private async Task<int> MaxID()
        {
            try
            {
                var query = await _client.Cypher
                        .Match("(p:Person)")
                        .Return<int>(p => p.As<Person>().ID)
                        .OrderByDescending("p.ID")
                        //.Return<int>("ID(p)")
                        //.OrderByDescending("ID(p)")
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
                      .Match("(person:Person)")
                      .Return(person => person.As<Person>())
                      .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Object> GetAllActors()
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[r:IN_ROLE]-(s:Series)")
                       .With("DISTINCT p as actor")
                       .Return((actor) => new
                       {
                           Actor = actor.As<Person>()
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Object> GetAllDirectors()
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .With("DISTINCT p as director")
                       .Return((director) => new
                       {
                           Director = director.As<Person>()
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<Person> GetPerson(int personID)
        {
            try
            {
                var res = await _client.Cypher
                     .Match("(p:Person)")
                     .Where((Person p) => p.ID == personID)
                     .Return(p => p.As<Person>())
                     .ResultsAsync;

              
                return res.FirstOrDefault();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> Post(Person p)
        {
            maxID = await MaxID();

            if (maxID != -1)
            {
                try
                {
                    var newPerson = new Person { ID = maxID + 1, Name = p.Name, Sex = p.Sex, Birthplace = p.Birthplace, Birthday = p.Birthday, Biography = p.Biography };
                    var res = _client.Cypher
                                .Create("(person:Person $newPerson)")
                                .WithParam("newPerson", newPerson);

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

        public async Task<bool> Put(Person person, int id)
        {
            try
            {
                var res = _client.Cypher
                              .Match("(p:Person)")
                              .Where((Person p) => p.ID == id)
                              .Set("p = $person")
                              .WithParam("person", person);

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
                             .Match("(p:Person)")
                             .Where((Person p) => p.ID == id)
                             .DetachDelete("p");

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
