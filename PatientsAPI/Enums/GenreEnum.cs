using System.ComponentModel.DataAnnotations;

namespace P10___MédiLabo___Patients_API.Enums;

public enum GenreEnum
{
    [Display(Name = "Masculin")]
    M = 1,
    [Display(Name = "Féminin")]
    F = 2,
    [Display(Name = "Autre")]
    Autre = 3
}