using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace P10___MédiLabo_Solutions.Models;

public class PatientViewModel
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Le prénom est obligatoire.")]
    public string Prenom { get; set; }
    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string Nom { get; set; }
    [DisplayName ("Date de naissance")]
    [Required(ErrorMessage = "La date de naissance est obligatoire.")]
    public DateTime DateNaissance { get; set; }
    [Required(ErrorMessage = "Le genre est obligatoire.")]
    public int Genre { get; set; }
    public string? Adresse { get; set; }
    public string? Telephone { get; set; }
}