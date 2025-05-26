using Microsoft.EntityFrameworkCore;
using P10___MédiLabo___Patients_API.Data;
using P10___MédiLabo___Patients_API.Models;

namespace P10___MédiLabo___Patients_API.Services;

public class PatientsService(PatientsDbContext context)
{
    public async Task<List<Patient>> GetAllPatientsAsync()
    {
        return await context.Patients.ToListAsync();
    }

    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        return await context.Patients.FindAsync(id);
    }

    public async Task AddPatientAsync(Patient patient)
    {
        await context.Patients.AddAsync(patient);
        await context.SaveChangesAsync();
    }

    public async Task UpdatePatientAsync(Patient patient)
    {
        var existingPatient = await context.Patients.FindAsync(patient.Id);
        if (existingPatient != null)
        {
            context.Entry(existingPatient).CurrentValues.SetValues(patient);
            await context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("Patient introuvable pour la mise à jour.");
        }
    }

    public async Task DeletePatientAsync(int id)
    {
        var patient = await GetPatientByIdAsync(id);
        if (patient != null)
        {
            context.Patients.Remove(patient);
            await context.SaveChangesAsync();
        }
    }
}