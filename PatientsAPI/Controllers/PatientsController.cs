using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P10___MédiLabo___Patients_API.Models;
using P10___MédiLabo___Patients_API.Services;

namespace P10___MédiLabo___Patients_API.Controllers;

[ApiController]
[Authorize]
[Route("api/patients")]
public class PatientsController(PatientsService patientsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPatients()
    {
        var patients = await patientsService.GetAllPatientsAsync();
        return Ok(patients);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientById(int id)
    {
        var patient = await patientsService.GetPatientByIdAsync(id);
        if (patient == null)
        {
            return NotFound();
        }
        return Ok(patient);
    }

    [HttpPost]
    public async Task<IActionResult> AddPatient([FromBody] Patient patient)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await patientsService.AddPatientAsync(patient);
        return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patient)
    {
        if (id != patient.Id || !ModelState.IsValid)
        {
            return BadRequest();
        }

        var existingPatient = await patientsService.GetPatientByIdAsync(id);
        if (existingPatient == null)
        {
            return NotFound();
        }

        await patientsService.UpdatePatientAsync(patient);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatient(int id)
    {
        var patient = await patientsService.GetPatientByIdAsync(id);
        if (patient == null)
        {
            return NotFound();
        }

        await patientsService.DeletePatientAsync(id);
        return NoContent();
    }
}