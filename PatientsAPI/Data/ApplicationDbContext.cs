using Microsoft.EntityFrameworkCore;
using P10___MédiLabo___Patients_API.Models;

namespace P10___MédiLabo___Patients_API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }
}