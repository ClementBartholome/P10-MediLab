using System.ComponentModel;

namespace P10___MédiLabo_Solutions.Models;

public class DetailPatientViewModel
{
    public PatientViewModel Patient { get; set; }
    public List<NoteViewModel> Notes { get; set; }
}