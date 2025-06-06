namespace P10___MédiLabo_Solutions.Models;

public class RiskAssessmentViewModel
{
    public string PatientId { get; set; }
    public string RiskLevel { get; set; }
    public string RiskLevelLabel { get; set; }
    public Dictionary<string, int> TriggerTermsFound { get; set; } = [];
    public int Age { get; set; }
    public string Sex { get; set; }
    public string PatientName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public int NoteCount { get; set; }
    
    public string BadgeClass => RiskLevel switch
    {
        "None" => "bg-success",
        "Borderline" => "bg-warning", 
        "InDanger" => "bg-danger",
        "EarlyOnset" => "bg-dark",
        _ => "bg-secondary"
    };
    
    public string Icon => RiskLevel switch
    {
        "None" => "fa-check-circle",
        "Borderline" => "fa-exclamation",
        "InDanger" => "fa-exclamation-triangle",
        "EarlyOnset" => "fa-exclamation-circle",
        _ => "fa-question-circle"
    };
}