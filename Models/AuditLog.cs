namespace MIS_GroupProject3.Models;

public class AuditLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Action { get; set; } = string.Empty; // AccessRequestApproved, AccessRequestDenied, UserRoleChanged, etc.
    public string UserEmail { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info"; // Info, Warning, Critical
    public string? IpAddress { get; set; }
}

