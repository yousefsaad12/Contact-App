using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repository
{
    public class CountriesRepo : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CountriesRepo(ApplicationDbContext db) {  _db = db; }

        public async Task<Country> AddCountry(Country country)
        {
           await _db.Countries.AddAsync(country);
           await _db.SaveChangesAsync();

           return country; 
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryById(Guid countryId)
        {
            return await _db.Countries.FirstOrDefaultAsync(c => c.CountryId == countryId);
        }

        public async Task<Country?> GetCountryByName(string name)
        {
            return await _db.Countries.FirstOrDefaultAsync(c => c.CountryName == name);
        }
    }
}