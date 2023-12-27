using System;
using System.Collections.Generic;
using Xunit;
using ServiceContracts;
using Entities;
using ServiceContracts.DTO;
using Services;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Xunit.Sdk;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //private fields
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly Mock<IPersonsRepository> _personsRepoMock;
        private readonly IPersonsRepository _personsRepository;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;


        //constructor
        public PersonsServiceTest(ITestOutputHelper testOutputHelper)
        {
            List<Country> countries = new List<Country>() { };
            List<Person> people = new List<Person>() { };

            _personsRepoMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepoMock.Object;

            DbContextMock <ApplicationDbContext> dbContextMockCountry = new DbContextMock<ApplicationDbContext> (new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            DbContextMock<ApplicationDbContext> dbContextMockPerson = new DbContextMock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContextCountry = dbContextMockCountry.Object;
            ApplicationDbContext dbContextPerson = dbContextMockPerson.Object;

            dbContextMockCountry.CreateDbSetMock(temp => temp.Countries, countries);
            dbContextMockPerson.CreateDbSetMock(temp => temp.Persons, people);

            _testOutputHelper = testOutputHelper;
            _countriesService = new CountryService(null);

            _personService = new PersonService(_personsRepository);
            _fixture = new Fixture();
           
        }

        #region AddPerson

        //When we supply null value as PersonAddRequest, it should throw ArgumentNullException
        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = null;

            //Act
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await _personService.AddPerson(personAddRequest);
            });
        }


        //When we supply null value as PersonName, it should throw ArgumentException
        [Fact]
        public async Task AddPerson_PersonNameIsNull()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                                                         .With(p => p.Name, null as string)
                                                         .Create();

            Person person = personAddRequest.ToPerson();

            _personsRepoMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                            .ReturnsAsync(person);

            //Act
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
               await _personService.AddPerson(personAddRequest);
            });
        }

        //When we supply proper person details, it should insert the person into the persons list; and it should return an object of PersonResponse, which includes with the newly generated person id
        [Fact]
        public async Task AddPerson_FullPersonDetails_ToBeSuccessful()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                                                         .With(temp => temp.Email,"yousefsaadmohamed1@gmail.com" )
                                                         .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse personResponse = person.ToPersonResponse();

            _personsRepoMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                            .ReturnsAsync(person);
            //Act
            PersonResponse person_response_from_add = await _personService.AddPerson(personAddRequest);


            //Assert
            Assert.True(person_response_from_add.PersonId != Guid.Empty);

            Assert.Equal(person_response_from_add, personResponse);
        }

        #endregion

        #region GetPersonByPersonID

        //If we supply null as PersonID, it should return null as PersonResponse
        [Fact]
        public async Task GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

           
            //Act
            PersonResponse? person_response_from_get = await _personService.GetPersonById(personID);

            //Assert
            Assert.Null(person_response_from_get);
        }


        //If we supply a valid person id, it should return the valid person details as PersonResponse object
        [Fact]
        public async Task GetPersonByPersonID_WithPersonID()
        {
            

            Person person = _fixture.Build<Person>()
                                    .With(temp => temp.Email, "yousefsaadmohamed1@gmail.com")
                                    .Create();

            _personsRepoMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                            .ReturnsAsync(person);

            PersonResponse personResponse = person.ToPersonResponse();

            PersonResponse? person_response_from_get = await _personService.GetPersonById(person.PersonId);

            //Assert
            Assert.Equal(personResponse, person_response_from_get);
        }

        #endregion

        #region GetAllPersons

        //The GetAllPersons() should return an empty list by default
        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            //Act
            _personsRepoMock.Setup(temp => temp.GetAllPersons())
                           .ReturnsAsync(new List<Person>());

            List<PersonResponse> persons_from_get = await _personService.GetAllPersons();

            //Assert
            Assert.Empty(persons_from_get);
        }


        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            //Arrange
           
            List<Person> people = new List<Person>() 
            {
               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed1@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed2@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed3@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

            };


            List<PersonResponse> person_response_list_expected = people.Select(temp => temp.ToPersonResponse()).ToList();

           

            //print 
            _testOutputHelper.WriteLine("Expected : \n");
            foreach(PersonResponse personResponse in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString()); 
            }

            _personsRepoMock.Setup(temp => temp.GetAllPersons())
                            .ReturnsAsync(people);

            //Act
            List<PersonResponse> persons_list_from_get = await _personService.GetAllPersons();


            _testOutputHelper.WriteLine("Result : \n");
            foreach (PersonResponse personResponse in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                Assert.Contains(person_response_from_add, persons_list_from_get);
            }
        }

        #endregion

        #region GetFilteredPersons


        //First, we will add few persons; and then when we call GetAllPersons(), it should return the same persons that were added
        [Fact]
        public async Task GetFilteredPersons_EmptySearchText()
        {
            //Arrange
            List<Person> people = new List<Person>()
            {
               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed1@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed2@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed3@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

            };


            List<PersonResponse> person_response_list_expected = people.Select(temp => temp.ToPersonResponse()).ToList();
            
            _personsRepoMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person,bool>>>()))
                            .ReturnsAsync(people);

            //print 
            _testOutputHelper.WriteLine("Expected : \n");
            foreach (PersonResponse personResponse in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse>  persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.Name), "");


            _testOutputHelper.WriteLine("Result : \n");
            foreach (PersonResponse personResponse in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }

        [Fact]
        public async Task GetFilteredPersons_SearchByName()
        {
            List<Person> people = new List<Person>()
            {
               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed1@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed2@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed3@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

            };


            List<PersonResponse> person_response_list_expected = people.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepoMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
                            .ReturnsAsync(people);

            //print 
            _testOutputHelper.WriteLine("Expected : \n");
            foreach (PersonResponse personResponse in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetFilteredPersons(nameof(Person.Name), "sa");


            _testOutputHelper.WriteLine("Result : \n");
            foreach (PersonResponse personResponse in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }
            //Assert
            foreach (PersonResponse person_response_from_add in person_response_list_expected)
            {
                Assert.Contains(person_response_from_add, persons_list_from_search);
            }
        }


        #endregion

        #region GetSortedPersons
        [Fact]
        public async Task GetSortedPersons_SearchByName()
        {
            //Arrange
            List<Person> people = new List<Person>()
            {
               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed1@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed2@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

               _fixture.Build<Person>()
                       .With(temp => temp.Email,"yousefsaadmohamed3@gmail.com" )
                       .With(temp => temp.Country, null as Country)
                       .Create(),

            };

            List<PersonResponse> person_response_list_expected = people.Select(temp => temp.ToPersonResponse())
                                                                       .ToList();


            _personsRepoMock.Setup(temp => temp.GetAllPersons())
                            .ReturnsAsync(people);
            //print 
            _testOutputHelper.WriteLine("Expected : \n");
            foreach (PersonResponse personResponse in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Act
            List<PersonResponse> persons_list_from_search = await _personService.GetSortedPersons(await _personService.GetAllPersons(),nameof(Person.Name), SortedOrderOptions.DESC);


            _testOutputHelper.WriteLine("Result : \n");
            foreach (PersonResponse personResponse in persons_list_from_search)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            person_response_list_expected = person_response_list_expected.OrderByDescending(p => p.Name).ToList();
            //Assert
            
            for(int i = 0;  i < person_response_list_expected.Count; i++) 
                Assert.Equal(person_response_list_expected[i], persons_list_from_search[i]);
            
        }
        #endregion

        #region UpdatePerson

        [Fact]
        public async Task UpdatePerson_NullPerson()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _personService.UpdatePerson(personUpdateRequest)); 

        }

        [Fact]
        public async Task UpdatePerson_InvalidId()
        {
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

            await Assert.ThrowsAsync<ArgumentException>(async () => await _personService.UpdatePerson(personUpdateRequest));

        }

        [Fact]
        public async Task UpdatePerson_PersonNameIsNull()
        {
            Person person = _fixture.Build<Person>()
                                    .With(p => p.Country, null as Country)
                                    .With(p => p.Name, null as string)
                                    .With(temp => temp.Email, "yousefsaadmohamed1@gmail.com")
                                    .With(temp => temp.Gender, "Male")
                                    .Create();

            PersonResponse personResponse = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = personResponse.ToPersonUpdateRequest();
            


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => {
                //Act
                await _personService.UpdatePerson(person_update_request);
            });

        }


        //First, add a new person and try to update the person name and email
        [Fact]
        public async Task UpdatePerson_PersonFullDetailsUpdation()
        {
           

            Person person = _fixture.Build<Person>()
                                    .With(p => p.Country, null as Country)
                                    .With(temp => temp.Email, "yousefsaadmohamed1@gmail.com")
                                    .With(temp => temp.Gender, "Male")
                                    .Create();

            PersonResponse personResponse = person.ToPersonResponse();

            PersonUpdateRequest person_update_Request = personResponse.ToPersonUpdateRequest();

            _personsRepoMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
                            .ReturnsAsync(person);

            _personsRepoMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                            .ReturnsAsync(person);
            //Act
            PersonResponse person_response_from_update = await _personService.UpdatePerson(person_update_Request);

          

            //Assert
            Assert.Equal(personResponse, person_response_from_update);

        }


        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePerson_ValidId()
        {

            Person person = _fixture.Build<Person>()
                                    .With(p => p.Country, null as Country)
                                    .With(p => p.Name, "Mohamed")
                                    .With(temp => temp.Gender, "Male")
                                    .With(temp => temp.Email, "yousefsaadmohamed1@gmail.com")
                                    .Create();


            _personsRepoMock.Setup(temp => temp.DeletePersonById(It.IsAny<Guid>()))
                            .ReturnsAsync(true);

            _personsRepoMock.Setup(temp => temp.GetPersonById(It.IsAny<Guid>()))
                            .ReturnsAsync(person);

            bool isDeleted = await _personService.DeletePerson(person.PersonId);

            Assert.True(isDeleted);
        }

        [Fact]
        public async Task DeletePerson_InvalidId()
        {

            bool isDeleted = await _personService.DeletePerson(Guid.NewGuid());

            Assert.False(isDeleted);
        }

        #endregion
    }
}
