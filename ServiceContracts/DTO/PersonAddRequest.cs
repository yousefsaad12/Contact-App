using Entities;
using ServiceContracts.Enums;
using System;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO Requset for new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Name feild must be applied")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email feild must be applied")]
        [EmailAddress(ErrorMessage = "Email must be in gmail or yahoo")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime ? DateOfBrith { get; set; }

        public GenderOptions ? Gender { get; set; }

        public Guid? CountryId { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public Person ToPerson () 
        {
            return new Person { Name = Name, Email = Email, 
                                Address = Address, CountryId = CountryId,
                                DateOfBrith = DateOfBrith, Gender = Gender.ToString(),
                                ReceiveNewsLetters = ReceiveNewsLetters
                              };
        }
    }
}
