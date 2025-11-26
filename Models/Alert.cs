namespace MIS_GroupProject3.Models;

public class Alert
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string Severity { get; set; } = "Medium";
    public string Source { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

