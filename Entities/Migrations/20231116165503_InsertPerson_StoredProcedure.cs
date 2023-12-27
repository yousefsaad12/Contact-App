using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
             CREATE PROCEDURE [dbo].[InsertPerson]
             (@PersonId uniqueidentifier, @Name varchar(40), @Email varchar(120), 
              @DateOfBrith datetime2(7), @Gender varchar(10), @CountryId uniqueidentifier,
              @Address varchar(200), @ReceiveNewsLetters bit)

             AS BEGIN

             INSERT INTO [dbo].[Persons] (PersonId, Name, Email, DateOfBrith, Gender, CountryId, Address, ReceiveNewsLetters) VALUES (@PersonId, @Name, @Email, @DateOfBrith, @Gender, @CountryId, @Address, @ReceiveNewsLetters)
             
            END
        ";

            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
             DROP PROCEDURE [dbo].[InsertPerson ]
        ";

            migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
