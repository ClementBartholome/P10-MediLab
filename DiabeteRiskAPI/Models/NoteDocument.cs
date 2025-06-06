namespace DiabeteRiskAPI.Models;

public class NoteDocument
{
    public string Id { get; set; }
    public string Note { get; set; }
    public int PatientId { get; set; } 
    public DateTime Date { get; set; }
}