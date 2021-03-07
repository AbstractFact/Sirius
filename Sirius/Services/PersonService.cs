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
    public class PersonService
    {
        private readonly IGraphClient _client;
        public PersonService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<Object> GetAll()
        {
            try
            {
                var res = await _client.Cypher
                       .Match("(person:Person)")
                       .Return((person) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(person)"),
                           Name = person.As<Person>().Name,
                           Sex = person.As<Person>().Sex,
                           Birthplace = person.As<Person>().Birthplace,
                           Birthday = person.As<Person>().Birthday,
                           Biography = person.As<Person>().Biography,
                       })
                      .ResultsAsync;

                return res;
            }
            catch (Exception)
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
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<Person>().Name,
                           Sex = actor.As<Person>().Sex,
                           Birthplace = actor.As<Person>().Birthplace,
                           Birthday = actor.As<Person>().Birthday,
                           Biography = actor.As<Person>().Biography,
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception)
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
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<Person>().Name,
                           Sex = director.As<Person>().Sex,
                           Birthplace = director.As<Person>().Birthplace,
                           Birthday = director.As<Person>().Birthday,
                           Biography = director.As<Person>().Biography,
                       })
                       .ResultsAsync;

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<PersonDTO> GetPerson(int personID)
        {
            try
            {
                var res = await _client.Cypher
                     .Match("(p:Person)")
                     .Where("ID(p) = $personID")
                     .WithParam("personID", personID)
                     .Return((p) => new PersonDTO
                      {
                         ID = Return.As<int>("ID(p)"),
                         Name = p.As<Person>().Name,
                         Sex = p.As<Person>().Sex,
                         Birthplace = p.As<Person>().Birthplace,
                         Birthday = p.As<Person>().Birthday,
                         Biography = p.As<Person>().Biography,
                     })
                     .ResultsAsync;

              
                return res.FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Post(Person p)
        {
            try
            {
                var res = _client.Cypher
                            .Create("(person:Person $p)")
                            .WithParam("p", p);

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }             
        }

        public async Task<bool> Put(Person person, int id)
        {
            try
            {
                var res = _client.Cypher
                              .Match("(p:Person)")
                              .Where("ID(p) = $id")
                              .WithParam("id", id)
                              .Set("p = $person")
                              .WithParam("person", person);

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
                             .Match("(p:Person)")
                             .Where("ID(p) = $id")
                             .WithParam("id", id)
                             .DetachDelete("p");

                await res.ExecuteWithoutResultsAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<PersonDTO>> GetActorsFiltered(PersonFilterDTO filter)
        {
            try
            {
                var res = new List<PersonDTO>();
                if (filter.Name != "" && filter.Sex != "All")
                {
                    res = (List<PersonDTO>) await _client.Cypher
                       .Match("(p:Person)-[d:IN_ROLE]-(s:Series)")
                       .Where((Person p) => p.Name == filter.Name)
                       .AndWhere((Person p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as actor")
                       .Return((actor) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<Person>().Name,
                           Sex = actor.As<Person>().Sex,
                           Birthplace = actor.As<Person>().Birthplace,
                           Birthday = actor.As<Person>().Birthday,
                           Biography = actor.As<Person>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name != "" && filter.Sex == "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:IN_ROLE]-(s:Series)")
                       .Where((Person p) => p.Name == filter.Name)
                       .With("DISTINCT p as actor")
                       .Return((actor) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<Person>().Name,
                           Sex = actor.As<Person>().Sex,
                           Birthplace = actor.As<Person>().Birthplace,
                           Birthday = actor.As<Person>().Birthday,
                           Biography = actor.As<Person>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name == "" && filter.Sex != "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:IN_ROLE]-(s:Series)")
                       .Where((Person p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as actor")
                       .Return((actor) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<Person>().Name,
                           Sex = actor.As<Person>().Sex,
                           Birthplace = actor.As<Person>().Birthplace,
                           Birthday = actor.As<Person>().Birthday,
                           Biography = actor.As<Person>().Biography
                       })
                       .ResultsAsync;
                }
                else
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:IN_ROLE]-(s:Series)")
                       .With("DISTINCT p as actor")
                       .Return((actor) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<Person>().Name,
                           Sex = actor.As<Person>().Sex,
                           Birthplace = actor.As<Person>().Birthplace,
                           Birthday = actor.As<Person>().Birthday,
                           Biography = actor.As<Person>().Biography
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

        public async Task<List<PersonDTO>> GetDirectorsFiltered(PersonFilterDTO filter)
        {
            try
            {
                var res = new List<PersonDTO>();
                if (filter.Name != "" && filter.Sex != "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .Where((Person p) => p.Name.Contains(filter.Name))
                       .AndWhere((Person p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as director")
                       .Return((director) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<Person>().Name,
                           Sex = director.As<Person>().Sex,
                           Birthplace = director.As<Person>().Birthplace,
                           Birthday = director.As<Person>().Birthday,
                           Biography = director.As<Person>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name != "" && filter.Sex == "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .Where((Person p) => p.Name.Contains(filter.Name))
                       .With("DISTINCT p as director")
                       .Return((director) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<Person>().Name,
                           Sex = director.As<Person>().Sex,
                           Birthplace = director.As<Person>().Birthplace,
                           Birthday = director.As<Person>().Birthday,
                           Biography = director.As<Person>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name == "" && filter.Sex != "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .Where((Person p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as director")
                       .Return((director) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<Person>().Name,
                           Sex = director.As<Person>().Sex,
                           Birthplace = director.As<Person>().Birthplace,
                           Birthday = director.As<Person>().Birthday,
                           Biography = director.As<Person>().Biography
                       })
                       .ResultsAsync;
                }
                else
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .With("DISTINCT p as director")
                       .Return((director) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<Person>().Name,
                           Sex = director.As<Person>().Sex,
                           Birthplace = director.As<Person>().Birthplace,
                           Birthday = director.As<Person>().Birthday,
                           Biography = director.As<Person>().Biography
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
