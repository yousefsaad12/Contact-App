using Entities;
using ServiceContracts.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Person ID feild must be applied")]
        public Guid PersonId { get; set; }

        [Required(ErrorMessage = "Name feild must be applied")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email feild must be applied")]
        [EmailAddress(ErrorMessage = "Email must be in gmail or yahoo")]
        public string? Email { get; set; }
        public DateTime? DateOfBrith { get; set; }

        public GenderOptions? Gender { get; set; }

        public Guid? CountryId { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson()
        {
            return new Person
            {
                PersonId = PersonId,
                Name = Name,
                Email = Email,
                Address = Address,
                CountryId = CountryId,
                DateOfBrith = DateOfBrith,
                Gender = Gender.ToString(),
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }
}
