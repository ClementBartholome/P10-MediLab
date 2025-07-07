using System.ComponentModel.DataAnnotations;

namespace P10___MédiLabo_Solutions.Models;

public class NoteViewModel
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le contenu de la note est obligatoire.")]
    public string Note { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
    public int PatientId { get; set; }
}