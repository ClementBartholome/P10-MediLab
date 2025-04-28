using P10___MédiLabo___Patients_API.Enums;

namespace P10___MédiLabo___Patients_API.Models;

public class Patient
{
    public int Id { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public DateTime DateNaissance { get; set; }
    public GenreEnum Genre { get; set; }
    public string? Adresse { get; set; }
    public string? Telephone { get; set; }
}