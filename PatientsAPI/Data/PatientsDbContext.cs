using Microsoft.EntityFrameworkCore;
using P10___MédiLabo___Patients_API.Enums;
using P10___MédiLabo___Patients_API.Models;

namespace P10___MédiLabo___Patients_API.Data;

public class PatientsDbContext(DbContextOptions<PatientsDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data
        modelBuilder.Entity<Patient>().HasData(
            new Patient
            {
                Id = 1, Nom = "TestNone", Prenom = "Test", DateNaissance = new DateTime(1966, 12, 31),
                Genre = GenreEnum.F, Adresse = "1 Brookside St", Telephone = "100-222-3333"
            },
            new Patient
            {
                Id = 2, Nom = "TestBorderline", Prenom = "Test", DateNaissance = new DateTime(1945, 6, 24),
                Genre = GenreEnum.M, Adresse = "2 High St", Telephone = "200-333-4444"
            },
            new Patient
            {
                Id = 3, Nom = "TestInDanger", Prenom = "Test", DateNaissance = new DateTime(2004, 6, 18),
                Genre = GenreEnum.M, Adresse = "3 Club Road", Telephone = "300-444-5555"
            },
            new Patient
            {
                Id = 4, Nom = "TestEarlyOnset", Prenom = "Test", DateNaissance = new DateTime(2002, 6, 28),
                Genre = GenreEnum.F, Adresse = "4 Valley Dr", Telephone = "400-555-6666"
            }
        );
    }
}