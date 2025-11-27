namespace MIS_GroupProject3.Models;

public class AccessRequest
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string RequestedAccessType { get; set; } = "Full alert access"; // Full alert access, High-Level IOC Access, etc.
    public DateTime RequestDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Pending"; // Pending, Approved, Denied, MoreInfoRequested
    public string EmailConfirmationStatus { get; set; } = "Pending"; // Pending, Confirmed, Failed
    public string DomainTrustScore { get; set; } = "Unknown"; // High, Medium, Low, Unknown
    public bool HasVerificationUpload { get; set; } = false;
    public string? VerificationNotes { get; set; }
    public string? AssignedRole { get; set; } // Member, Sector Analyst, Organization Lead, Restricted, Admin
}

