namespace MIS_GroupProject3.Models;

public class SystemSettings
{
    public int Id { get; set; }
    public bool RequireMfa { get; set; } = false;
    public int SessionTimeoutMinutes { get; set; } = 60;
    public bool EmailVerificationRequired { get; set; } = true;
    public bool DomainValidationEnabled { get; set; } = true;
    public List<string> TrustedDomains { get; set; } = new(); // .gov, .edu, etc.
    public List<string> RestrictedDomains { get; set; } = new(); // Personal email domains
    public Dictionary<string, string> RolePermissions { get; set; } = new();
}

