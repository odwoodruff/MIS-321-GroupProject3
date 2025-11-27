namespace MIS_GroupProject3.Models;

public class Organization
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty; // Email domain
    public string Sector { get; set; } = string.Empty; // Agriculture, Hospitals, Pharma, etc.
    public string Type { get; set; } = string.Empty; // Government, Academic, Private, etc.
    public int MemberCount { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsVerified { get; set; } = false;
    public string VerificationStatus { get; set; } = "Pending"; // Pending, Verified, Rejected
    public string? ContactEmail { get; set; }
}

