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
        // Étape 1: Récupérer les informations du patient
        var patient = await patientDataService.GetPatientAsync(patientId);
        if (patient == null)
        {
            logger.LogWarning("Patient non trouvé: {PatientId}", patientId);
            return null;
        }

        // Étape 2: Récupérer les notes du patient et les indexer si nécessaire
        var notes = await patientDataService.GetPatientNotesAsync(patientId);
        logger.LogInformation("Récupération de {NoteCount} notes pour le patient {PatientId}", notes.Count, patientId);
        
        // Étape 3: Indexer les notes dans ElasticSearch
        await IndexPatientNotesAsync(notes);
        
        // Étape 4: Évaluer le risque de diabète
        var result = await diabetesRiskService.AssessPatientRiskAsync(patient);
        
        // Ajouter des informations supplémentaires au résultat
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