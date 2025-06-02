namespace P10___MédiLabo_Solutions.Models;

public class DeleteConfirmationViewModel
{
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public string ItemTypeDescription { get; set; } = "cet élément";
    public string WarningMessage { get; set; } = "Cette action est irréversible.";
    public string ControllerName { get; set; }
    public string ReturnUrl { get; set; }
    public Dictionary<string, string> AdditionalParameters { get; set; } = new Dictionary<string, string>();
}