namespace MIS_GroupProject3.Models;

public class ApprovedMember
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string AssignedRole { get; set; } = "Member"; // Member, Sector Analyst, Organization Lead, Restricted, Admin
    public string AccessLevel { get; set; } = "Full alert access";
    public DateTime ApprovalDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;
    public int AccessRequestId { get; set; } // Link to original access request
}

