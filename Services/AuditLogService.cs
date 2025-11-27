using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

public interface IAuditLogService
{
    void LogAction(string action, string userEmail, string adminEmail, string details, string severity = "Info", string? ipAddress = null);
    List<AuditLog> GetAllAuditLogs();
    List<AuditLog> GetAuditLogsByUser(string email);
    List<AuditLog> GetAuditLogsBySeverity(string severity);
}

public class AuditLogService : IAuditLogService
{
    private readonly List<AuditLog> _auditLogs;
    private int _nextId = 1;

    public AuditLogService()
    {
        _auditLogs = new List<AuditLog>();
    }

    public void LogAction(string action, string userEmail, string adminEmail, string details, string severity = "Info", string? ipAddress = null)
    {
        var log = new AuditLog
        {
            Id = _nextId++,
            Timestamp = DateTime.Now,
            Action = action,
            UserEmail = userEmail,
            AdminEmail = adminEmail,
            Details = details,
            Severity = severity,
            IpAddress = ipAddress
        };
        _auditLogs.Add(log);
    }

    public List<AuditLog> GetAllAuditLogs()
    {
        return _auditLogs.OrderByDescending(l => l.Timestamp).ToList();
    }

    public List<AuditLog> GetAuditLogsByUser(string email)
    {
        return _auditLogs
            .Where(l => l.UserEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.Timestamp)
            .ToList();
    }

    public List<AuditLog> GetAuditLogsBySeverity(string severity)
    {
        return _auditLogs
            .Where(l => l.Severity.Equals(severity, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(l => l.Timestamp)
            .ToList();
    }
}

