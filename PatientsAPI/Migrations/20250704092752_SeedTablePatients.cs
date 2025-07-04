using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace P10___MédiLabo___Patients_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedTablePatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Adresse", "DateNaissance", "Genre", "Nom", "Prenom", "Telephone" },
                values: new object[,]
                {
                    { 1, "1 Brookside St", new DateTime(1966, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "TestNone", "Test", "100-222-3333" },
                    { 2, "2 High St", new DateTime(1945, 6, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "TestBorderline", "Test", "200-333-4444" },
                    { 3, "3 Club Road", new DateTime(2004, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "TestInDanger", "Test", "300-444-5555" },
                    { 4, "4 Valley Dr", new DateTime(2002, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "TestEarlyOnset", "Test", "400-555-6666" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
