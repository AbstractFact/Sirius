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
                           Name = Return.As<string>("person.Name"),
                           Sex = Return.As<string>("person.Sex"),
                           Birthplace = Return.As<string>("person.Birthplace"),
                           Birthday = Return.As<string>("person.Birthday"),
                           Biography = Return.As<string>("person.Biography")
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
                           Name = Return.As<string>("actor.Name"),
                           Sex = Return.As<string>("actor.Sex"),
                           Birthplace = Return.As<string>("actor.Birthplace"),
                           Birthday = Return.As<string>("actor.Birthday"),
                           Biography = Return.As<string>("actor.Biography")
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
                           Name = Return.As<string>("director.Name"),
                           Sex = Return.As<string>("director.Sex"),
                           Birthplace = Return.As<string>("director.Birthplace"),
                           Birthday = Return.As<string>("director.Birthday"),
                           Biography = Return.As<string>("director.Biography")
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
                          Name = Return.As<string>("p.Name"),
                          Sex = Return.As<string>("p.Sex"),
                          Birthplace = Return.As<string>("p.Birthplace"),
                          Birthday = Return.As<string>("p.Birthday"),
                          Biography = Return.As<string>("p.Biography")
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
                       .Where((PersonDTO p) => p.Name == filter.Name)
                       .AndWhere((PersonDTO p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as actor")
                       .Return((actor) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<PersonDTO>().Name,
                           Sex = actor.As<PersonDTO>().Sex,
                           Birthplace = actor.As<PersonDTO>().Birthplace,
                           Birthday = actor.As<PersonDTO>().Birthday,
                           Biography = actor.As<PersonDTO>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name != "" && filter.Sex == "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:IN_ROLE]-(s:Series)")
                       .Where((PersonDTO p) => p.Name == filter.Name)
                       .With("DISTINCT p as actor")
                       .Return((actor) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<PersonDTO>().Name,
                           Sex = actor.As<PersonDTO>().Sex,
                           Birthplace = actor.As<PersonDTO>().Birthplace,
                           Birthday = actor.As<PersonDTO>().Birthday,
                           Biography = actor.As<PersonDTO>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name == "" && filter.Sex != "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:IN_ROLE]-(s:Series)")
                       .Where((PersonDTO p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as actor")
                       .Return((actor) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(actor)"),
                           Name = actor.As<PersonDTO>().Name,
                           Sex = actor.As<PersonDTO>().Sex,
                           Birthplace = actor.As<PersonDTO>().Birthplace,
                           Birthday = actor.As<PersonDTO>().Birthday,
                           Biography = actor.As<PersonDTO>().Biography
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
                           Name = actor.As<PersonDTO>().Name,
                           Sex = actor.As<PersonDTO>().Sex,
                           Birthplace = actor.As<PersonDTO>().Birthplace,
                           Birthday = actor.As<PersonDTO>().Birthday,
                           Biography = actor.As<PersonDTO>().Biography
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
                       .Where((PersonDTO p) => p.Name.Contains(filter.Name))
                       .AndWhere((PersonDTO p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as director")
                       .Return((director) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<PersonDTO>().Name,
                           Sex = director.As<PersonDTO>().Sex,
                           Birthplace = director.As<PersonDTO>().Birthplace,
                           Birthday = director.As<PersonDTO>().Birthday,
                           Biography = director.As<PersonDTO>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name != "" && filter.Sex == "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .Where((PersonDTO p) => p.Name.Contains(filter.Name))
                       .With("DISTINCT p as director")
                       .Return((director) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<PersonDTO>().Name,
                           Sex = director.As<PersonDTO>().Sex,
                           Birthplace = director.As<PersonDTO>().Birthplace,
                           Birthday = director.As<PersonDTO>().Birthday,
                           Biography = director.As<PersonDTO>().Biography
                       })
                       .ResultsAsync;
                }
                else if (filter.Name == "" && filter.Sex != "All")
                {
                    res = (List<PersonDTO>)await _client.Cypher
                       .Match("(p:Person)-[d:DIRECTED]-(s:Series)")
                       .Where((PersonDTO p) => p.Sex == filter.Sex)
                       .With("DISTINCT p as director")
                       .Return((director) => new PersonDTO
                       {
                           ID = Return.As<int>("ID(director)"),
                           Name = director.As<PersonDTO>().Name,
                           Sex = director.As<PersonDTO>().Sex,
                           Birthplace = director.As<PersonDTO>().Birthplace,
                           Birthday = director.As<PersonDTO>().Birthday,
                           Biography = director.As<PersonDTO>().Biography
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
                           Name = director.As<PersonDTO>().Name,
                           Sex = director.As<PersonDTO>().Sex,
                           Birthplace = director.As<PersonDTO>().Birthplace,
                           Birthday = director.As<PersonDTO>().Birthday,
                           Biography = director.As<PersonDTO>().Biography
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
