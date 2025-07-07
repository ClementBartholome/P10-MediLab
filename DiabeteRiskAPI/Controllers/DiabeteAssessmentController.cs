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
    ElasticSearchService elasticSearchService,
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
    
    [HttpPost("notes")]
    public async Task<IActionResult> IndexNote([FromBody] NoteDocument note)
    {
        try
        {
            await elasticSearchService.IndexNoteAsync(note);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de l'indexation de la note");
            return StatusCode(500, "Erreur lors de l'indexation de la note");
        }
    }
    
    [HttpDelete("notes/{noteId}")]
    public async Task<IActionResult> DeleteNote(string noteId)
    {
        try
        {
            var result = await elasticSearchService.DeleteNoteAsync(noteId);
            if (result)
            {
                return Ok();
            }
            return NotFound($"Note avec l'ID {noteId} introuvable");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la suppression de la note {NoteId}", noteId);
            return StatusCode(500, "Erreur lors de la suppression de la note");
        }
    }

    [HttpDelete("notes/patient/{patientId}")]
    public async Task<IActionResult> DeleteAllNotesByPatientId(string patientId)
    {
        try
        {
            var result = await elasticSearchService.DeleteAllNotesByPatientIdAsync(patientId);
            if (result)
            {
                return Ok();
            }
            return NotFound($"Aucune note trouvée pour le patient avec l'ID {patientId}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erreur lors de la suppression des notes pour le patient {PatientId}", patientId);
            return StatusCode(500, "Erreur lors de la suppression des notes");
        }
    }
}