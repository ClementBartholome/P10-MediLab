using DiabeteRiskAPI.Models;
using DiabeteRiskAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiabeteRiskAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/assessment")]
public class DiabetesAssessmentController(
    DiabetesAssessmentService assessmentService,
    ILogger<DiabetesAssessmentController> logger)
    : ControllerBase
{
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> AssessPatientRisk(int patientId)
    {
        try
        {
            var result = await assessmentService.AssessPatientRiskByIdAsync(patientId);
            if (result == null)
            {
                return NotFound($"Patient avec l'ID {patientId} introuvable");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'évaluation du risque pour le patient {PatientId}", patientId);
            return StatusCode(500, "Erreur lors de l'évaluation du risque");
        }
    }
}