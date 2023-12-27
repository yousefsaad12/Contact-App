using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class CountryService : ICountriesService
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountryService (ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
        } 

        public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
        {
            if(request == null)
                throw new ArgumentNullException(nameof(request));

            if(request.CountryName == null)
                throw new ArgumentException(nameof(request.CountryName));

            if(await _countriesRepository.GetCountryByName(request.CountryName) != null)
                throw new ArgumentException("Country name already exists");

            Country country = request.ToCountry();

            country.CountryId = Guid.NewGuid();


            await _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
        }

        public async Task  <CountryResponse?>  GetCountryById (Guid? countryId)
        {
            if(countryId == null)
                 return null;


            Country? country = await _countriesRepository.GetCountryById(countryId.Value);

            if (country == null)
                return null;

            return country.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {

            return (await _countriesRepository.GetAllCountries()).Select(c => c.ToCountryResponse()).ToList();
        }


    }
}