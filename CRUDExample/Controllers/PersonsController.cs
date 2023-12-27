using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Services;

namespace CRUDExample.Controllers
{
    public class PersonsController : Controller
    {
        private readonly IPersonService _personService;
        private readonly ICountriesService _countriesService;
        public PersonsController(IPersonService personService, ICountriesService countriesService) 
        {
            _personService = personService;
            _countriesService = countriesService;
        }

        [Route("persons/index")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string ? searchString, string sortBy = nameof(PersonResponse.Name), SortedOrderOptions sortedOrder = SortedOrderOptions.ASC)
        {
            ViewBag.SearchFields = new Dictionary<string, string>()
            {
                 { nameof(PersonResponse.Name), "Person Name" },
                 { nameof(PersonResponse.Email), "Email" },
                 { nameof(PersonResponse.DateOfBrith), "Date of Birth" },
                 { nameof(PersonResponse.Gender), "Gender" },
                 { nameof(PersonResponse.CountryId), "Country" },
                 { nameof(PersonResponse.Address), "Address" }
            };

            List<PersonResponse> response = await _personService.GetFilteredPersons(searchBy,searchString);

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;

            List<PersonResponse> sortedPersons = await _personService.GetSortedPersons(response, sortBy, sortedOrder);

            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortedOrder.ToString(); 

            return View(sortedPersons);
        }

        [Route("persons/create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
            ViewBag.Countries = countryResponses.Select(temp => new SelectListItem () { Text = temp.CountryName, Value = temp.CountryId.ToString()});

           
            return View();
        }

        [Route("persons/create")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if (!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() }); ;

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View();
            }

            //call the service method
            PersonResponse personResponse = await _personService.AddPerson(personAddRequest);

            return RedirectToAction("Index", "Persons");
        }

        [Route("persons/update/{personId}")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid personId) 
        {  
            PersonResponse ? personResponse  = await _personService.GetPersonById(personId);

            if(personResponse == null) 
            {
                return RedirectToAction("Index", "Persons");
            }
            List<CountryResponse> countryResponses = await _countriesService.GetAllCountries();
            ViewBag.Countries = countryResponses.Select(temp => new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
             
            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("persons/update/{personId}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest personUpdateRequest)
        {
           PersonResponse ? personResponse = await _personService.GetPersonById(personUpdateRequest.PersonId);
           
           if(personResponse == null) 
           {
               return RedirectToAction("Index", "Persons");
           }

           if(ModelState.IsValid)
           {
               personResponse = await _personService.UpdatePerson(personUpdateRequest);
               return RedirectToAction("Index", "Persons");
           }

           else
           {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;

                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return View(personResponse.ToPersonUpdateRequest());
           }

        }

        [HttpGet]
        [Route("persons/delete/{personId}")]
        public async Task<IActionResult> Delete(Guid personId) 
        {
            PersonResponse ? personResponse = await _personService.GetPersonById(personId);
            
            if(personResponse == null)
                return RedirectToAction("Index", "Persons");

            return View(personResponse);
        }


        [HttpPost]
        [Route("persons/delete/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse ? personResponse = await _personService.GetPersonById(personUpdateRequest.PersonId);
       
            if(personResponse == null)
                return RedirectToAction("Index", "Persons");

            await _personService.DeletePerson(personUpdateRequest.PersonId);

            return RedirectToAction("Index", "Persons");
        }
    }

    
}
