using AutoFixture;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTests
{
    public class PersonsControllerTest
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        private readonly Mock<IPersonService> _personServiceMock;
        private readonly Mock<ICountriesService> _countriesServiceMock;
        private readonly Fixture _fixture;

        public PersonsControllerTest ()
        {
            _fixture = new Fixture ();
            _countriesServiceMock = new Mock<ICountriesService>();
            _personServiceMock = new Mock<IPersonService> ();
            _countriesService =_countriesServiceMock.Object;
            _personService = _personServiceMock.Object;
        }


        #region Index
        [Fact]
        
        public async Task Index_ViewWithPersonsList()
        {

            List<PersonResponse> personResponses = _fixture.Create<List<PersonResponse>> ();

            PersonsController personsController = new PersonsController(_personService,_countriesService);

            _personServiceMock.Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
                              .ReturnsAsync(personResponses);

            _personServiceMock.Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortedOrderOptions>()))
                              .ReturnsAsync(personResponses);


            IActionResult result =  await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortedOrderOptions>());


            ViewResult viewResult =  Assert.IsType<ViewResult>(result);


        }
        #endregion

    }
}
