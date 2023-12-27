using ServiceContracts.DTO;

namespace ServiceContracts
{
    /// <summary>
    /// Represent Business logic
    /// </summary>
    public interface ICountriesService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest ? request);
        Task<List<CountryResponse>>GetAllCountries();

        Task<CountryResponse?> ? GetCountryById(Guid ? countryId);
    }
}