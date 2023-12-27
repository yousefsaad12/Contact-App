using ServiceContracts.DTO;
using ServiceContracts.Enums;
using System;

namespace ServiceContracts
{
    public interface IPersonService
    {
        Task <PersonResponse> AddPerson(PersonAddRequest ? request);
        Task < List<PersonResponse> > GetAllPersons();
        Task <PersonResponse ? > GetPersonById(Guid ? PersonId);
        Task < List<PersonResponse> >  GetFilteredPersons(string searchBy, string ? searchString);
        Task < List<PersonResponse> > GetSortedPersons(List<PersonResponse> allPersons, string sortedBy, SortedOrderOptions sortedOrder);
        Task <PersonResponse> UpdatePerson(PersonUpdateRequest ? personUpdateRequest);
        Task <bool> DeletePerson(Guid ? personId);
    }
}
