using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class PersonsRepo : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonsRepo (ApplicationDbContext db) {  _db = db; }

        public async Task<Person> AddPerson(Person person)
        {
           await _db.Persons.AddAsync(person);
           await _db.SaveChangesAsync();
           
           return person;
        }

        public async Task<bool> DeletePersonById(Guid personId)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(p => p.PersonId == personId));
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<List<Person>> GetAllPersons()
        {
           return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country")
                                    .Where(predicate)
                                    .ToListAsync();
        }

        public async Task<Person?> GetPersonById(Guid personId)
        {
            return await _db.Persons
                           .Include("Country")
                           .FirstOrDefaultAsync(p => p.PersonId == personId); 
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            Person ? matchingPerson = await _db.Persons.FirstOrDefaultAsync(p => p.PersonId == person.PersonId);

            if (matchingPerson == null) 
                return person; 

            matchingPerson.Gender = person.Gender;
            matchingPerson.Address = person.Address;
            matchingPerson.Email = person.Email;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;
            matchingPerson.CountryId = person.CountryId;
            matchingPerson.DateOfBrith = person.DateOfBrith;
            matchingPerson.Name = person.Name;   

            _db.Update(matchingPerson);
            
            int countUpdated = await _db.SaveChangesAsync();
            
            return matchingPerson;
        }
    }
}
