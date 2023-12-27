using System;
using Entities;

namespace ServiceContracts.DTO
{   
    /// <summary>
    /// DTO class use to return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string ? CountryName { get; set; }

        public override bool Equals(object ? obj)
        {
            if (obj == null)
                return false;

            if(obj.GetType() != typeof(CountryResponse))
                return false;

            CountryResponse countryResponse = (CountryResponse)obj;

            return countryResponse.CountryId == this.CountryId 
                && countryResponse.CountryName == this.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse() { CountryId = country.CountryId, CountryName = country.CountryName };
        }
    }
}
