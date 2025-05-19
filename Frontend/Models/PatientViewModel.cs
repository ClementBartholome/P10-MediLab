namespace P10___MédiLabo_Solutions.Models;

public class PatientViewModel
{
    public int Id { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public DateTime DateNaissance { get; set; }
    public int Genre { get; set; }
    public string Adresse { get; set; }
    public string Telephone { get; set; }
}