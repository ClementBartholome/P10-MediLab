using DiabeteRiskAPI.Models;

namespace DiabeteRiskAPI.Services;

public class DiabetesAssessmentService(
    PatientDataService patientDataService,
    ElasticSearchService elasticSearchService,
    DiabetesRiskService diabetesRiskService,
    ILogger<DiabetesAssessmentService> logger)
{
    public async Task<RiskAssessmentResult?> AssessPatientRiskByIdAsync(int patientId)
    {
        var patient = await patientDataService.GetPatientAsync(patientId);
        if (patient == null)
        {
            logger.LogWarning("Patient non trouvé: {PatientId}", patientId);
            return null;
        }

        var notes = await patientDataService.GetPatientNotesAsync(patientId);
        logger.LogInformation("Récupération de {NoteCount} notes pour le patient {PatientId}", notes.Count, patientId);
        
        var result = await diabetesRiskService.AssessPatientRiskAsync(patient);
        
        result.PatientName = $"{patient.Prenom} {patient.Nom}";
        result.DateOfBirth = patient.DateNaissance;
        result.NoteCount = notes.Count;
        
        return result;
    }
    
    private async Task IndexPatientNotesAsync(List<NoteDocument> notes)
    {
        foreach (var note in notes)
        {
            try
            {
                await elasticSearchService.IndexNoteAsync(note);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'indexation de la note {NoteId}", note.Id);
            }
        }
    }
}