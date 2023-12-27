using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts.DTO;
using ServiceContracts;
using Services;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using EntityFrameworkCoreMock;
using Moq;


namespace CRUDTests
{
    public  class CountryServiceTest
    {
        private readonly ICountriesService  _countriesService;

        // Constructor
        public CountryServiceTest() 
        {
            List<Country> countries = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countries);

            _countriesService = new CountryService(dbContext);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            CountryAddRequest ? request = null;

          await Assert.ThrowsAsync<ArgumentNullException>(async () => await _countriesService.AddCountry(request));

        }

        [Fact]
        public async Task AddCountry_CountryNameIsNull()
        {
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null};

            await Assert.ThrowsAsync<ArgumentException>(async () => await _countriesService.AddCountry(request));

        }

        [Fact]
        public async Task AddCountry_CountryNameIsDuplicate()
        {
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };

           await Assert.ThrowsAsync<ArgumentException>(async () => 
            {
               await _countriesService.AddCountry(request1);
               await _countriesService.AddCountry(request2);
                
            });

        }

        [Fact]
        public async Task AddCountry_AccaptedCountryDetails()
        {
            CountryAddRequest? request = new CountryAddRequest () { CountryName = "USA"};

            CountryResponse response = await _countriesService.AddCountry(request);

            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();   

            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countryResponses);

        }

        #endregion


        #region GetAllCountry


        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
           List<CountryResponse> list = await _countriesService.GetAllCountries();

           Assert.Empty(list);
        }

        [Fact]

        public async Task GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> countryAddRequests = new List<CountryAddRequest>()
            {
               new CountryAddRequest(){CountryName = "TOKYO"},
               new CountryAddRequest(){CountryName = "USA"},
               new CountryAddRequest(){CountryName = "UK"},
            };

            List<CountryResponse> countryResponses = new List<CountryResponse>();

            foreach (CountryAddRequest countryAddRequest in countryAddRequests)
               countryResponses.Add(await _countriesService.AddCountry(countryAddRequest));

            
            List<CountryResponse>list = await _countriesService.GetAllCountries();

            foreach(CountryResponse countryResponse in countryResponses)
            {
                Assert.Contains(countryResponse, list);
            }


        }



        #endregion


        #region GetCountryById

        [Fact]
        public async Task GetCountryById_NullCountryID()
        {
            Guid ? countryId = null;

            CountryResponse  ? countryResponse =  await _countriesService.GetCountryById(countryId);

            Assert.Null(countryResponse);   
        }


        [Fact]
        public async Task GetCountryById_ValidCountryID()
        {
            CountryAddRequest ? countryAddRequest = new CountryAddRequest() { CountryName = "China"};

            CountryResponse countryResponse = await _countriesService.AddCountry(countryAddRequest);

            CountryResponse ? countryR = await _countriesService.GetCountryById(countryResponse.CountryId);

            Assert.Equal(countryResponse, countryR); 
        }

        #endregion

    }
}
