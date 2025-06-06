using DiabeteRiskAPI.Services;

namespace DiabeteRiskAPI.Models;

public class RiskAssessmentResult
{
    public string PatientId { get; set; }
    public DiabetesRiskLevel RiskLevel { get; set; }
    public Dictionary<string, int> TriggerTermsFound { get; set; }
    public int Age { get; set; }
    public string Sex { get; set; }
    
    public string PatientName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int NoteCount { get; set; }
    
    public string RiskLevelLabel => RiskLevel switch
    {
        DiabetesRiskLevel.None => "Aucun risque",
        DiabetesRiskLevel.Borderline => "Risque limité",
        DiabetesRiskLevel.InDanger => "Danger",
        DiabetesRiskLevel.EarlyOnset => "Apparition précoce",
        _ => "Indéterminé"
    };
}