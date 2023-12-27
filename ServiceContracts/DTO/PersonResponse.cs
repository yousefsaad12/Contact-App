using Entities;
using ServiceContracts.Enums;
using System;


namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBrith { get; set; }

        public string? Gender { get; set; }

        public Guid? CountryId { get; set; }

        public string? Country { get; set; }

        public string? Address { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public double ? Age { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(PersonResponse))
                return false;

            PersonResponse other = (PersonResponse)obj;

            return PersonId == other.PersonId &&
                   Name == other.Name &&
                   Email == other.Email &&
                   DateOfBrith == other.DateOfBrith &&
                   Gender == other.Gender &&
                   CountryId == other.CountryId &&
                   Address == other.Address &&
                   ReceiveNewsLetters == other.ReceiveNewsLetters;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Person ID : {PersonId}, Person Name : {Name}" +
                $"Email :  {Email}, Date Of Brith : {DateOfBrith}, Gender : {Gender}" +
                $"Address : {Address}, Receive News Letters : {ReceiveNewsLetters}, Age : {Age} \n"; 
                
        }

        public PersonUpdateRequest ToPersonUpdateRequest ()
        {
            return new PersonUpdateRequest()
            {
                PersonId = PersonId,
                Name = Name,
                Email = Email,
                Address = Address,
                CountryId = CountryId,
                DateOfBrith = DateOfBrith,
                Gender = (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true),
                ReceiveNewsLetters = ReceiveNewsLetters
            };
        }
    }

    public static class PersonExtensions
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonId = person.PersonId,
                Name = person.Name,
                Email = person.Email,
                Address = person.Address,
                CountryId = person.CountryId,
                DateOfBrith = person.DateOfBrith,
                Gender = person.Gender,
                ReceiveNewsLetters = person.ReceiveNewsLetters,
                Age = (person.DateOfBrith != null) ? Math.Round((DateTime.Now - person.DateOfBrith.Value).TotalDays/365.25) : null,
                Country = person.Country?.CountryName
            };
        }

    }
}
