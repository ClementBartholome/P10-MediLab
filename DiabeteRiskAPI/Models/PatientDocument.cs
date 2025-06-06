using P10___MédiLabo___Patients_API.Enums;

namespace DiabeteRiskAPI.Models;

public class PatientDocument
{
    public int Id { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public DateTime DateNaissance { get; set; }
    public GenreEnum Genre { get; set; }
}