using Microsoft.AspNetCore.Mvc;
using MIS_GroupProject3.Services;
using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    private readonly IAlertService _alertService;
    private readonly IAlertDecorator _alertDecorator;
    private readonly IAccessRequestService _accessRequestService;
    private readonly IApprovedMemberService _approvedMemberService;
    private readonly IOrganizationService _organizationService;
    private readonly IAuditLogService _auditLogService;

    public ApiController(
        IAlertService alertService,
        IAlertDecorator alertDecorator,
        IAccessRequestService accessRequestService,
        IApprovedMemberService approvedMemberService,
        IOrganizationService organizationService,
        IAuditLogService auditLogService)
    {
        _alertService = alertService;
        _alertDecorator = alertDecorator;
        _accessRequestService = accessRequestService;
        _approvedMemberService = approvedMemberService;
        _organizationService = organizationService;
        _auditLogService = auditLogService;
    }

    [HttpGet("alerts")]
    public IActionResult GetAlerts([FromQuery] string? sector = null)
    {
        var alerts = string.IsNullOrWhiteSpace(sector) || sector == "All"
            ? _alertService.GetAllAlerts()
            : _alertService.GetAlertsBySector(sector);

        var decoratedAlerts = _alertDecorator.DecorateAll(alerts);
        return Ok(decoratedAlerts);
    }

    [HttpGet("metrics")]
    public IActionResult GetMetrics()
    {
        var metrics = _alertService.GetDashboardMetrics();
        return Ok(metrics);
    }

    [HttpGet("alerts/{id}")]
    public IActionResult GetAlert(int id)
    {
        var alert = _alertService.GetAlertById(id);
        if (alert == null)
        {
            return NotFound();
        }

        var decoratedAlert = _alertDecorator.Decorate(alert);
        return Ok(decoratedAlert);
    }

    [HttpPost("access-requests")]
    public IActionResult SubmitAccessRequest([FromBody] AccessRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName) ||
            string.IsNullOrWhiteSpace(request.OrganizationName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Role) ||
            string.IsNullOrWhiteSpace(request.RequestedAccessType))
        {
            return BadRequest("All required fields must be provided.");
        }

        // Validate email is not a personal email
        var personalEmailDomains = new[] { "gmail.com", "hotmail.com", "yahoo.com", "outlook.com", "icloud.com", "aol.com" };
        var emailDomain = request.Email.Split('@').LastOrDefault()?.ToLower();
        if (emailDomain != null && personalEmailDomains.Contains(emailDomain))
        {
            return BadRequest("Personal email addresses are not allowed. Please use your organization email.");
        }

        // Set default if not provided
        if (string.IsNullOrWhiteSpace(request.RequestedAccessType))
        {
            request.RequestedAccessType = "Full alert access";
        }

        _accessRequestService.SubmitAccessRequest(request);

        // Log the action
        _auditLogService.LogAction(
            "AccessRequestSubmitted",
            request.Email,
            "system",
            $"New access request submitted by {request.FullName} from {request.OrganizationName}",
            "Info"
        );

        return Ok(new { message = "Access request submitted successfully. Awaiting admin approval." });
    }

    [HttpGet("access-requests")]
    public IActionResult GetAllAccessRequests()
    {
        var requests = _accessRequestService.GetAllAccessRequests();
        return Ok(requests);
    }

    [HttpPut("access-requests/{id}/status")]
    public IActionResult UpdateAccessRequestStatus(int id, [FromBody] UpdateStatusRequest statusRequest)
    {
        var request = _accessRequestService.GetAccessRequestById(id);
        if (request == null)
        {
            return NotFound();
        }

        _accessRequestService.UpdateAccessRequestStatus(id, statusRequest.Status);

        // If approved, create approved member
        if (statusRequest.Status == "Approved")
        {
            var member = new ApprovedMember
            {
                FullName = request.FullName,
                OrganizationName = request.OrganizationName,
                Department = request.Department,
                Email = request.Email,
                Role = request.Role,
                AssignedRole = request.AssignedRole ?? "Member",
                AccessLevel = request.RequestedAccessType,
                AccessRequestId = request.Id
            };
            _approvedMemberService.AddApprovedMember(member);

            // Update or create organization
            var domain = request.Email.Split('@').LastOrDefault() ?? "";
            var org = _organizationService.GetOrganizationByDomain(domain);
            if (org == null)
            {
                org = new Organization
                {
                    Name = request.OrganizationName,
                    Domain = domain,
                    Sector = "Unknown",
                    Type = "Unknown"
                };
                _organizationService.AddOrganization(org);
            }
            _organizationService.IncrementMemberCount(domain);
        }

        // Log the action
        _auditLogService.LogAction(
            $"AccessRequest{statusRequest.Status}",
            request.Email,
            "admin@bio-isac.org", // In real app, get from auth context
            $"Access request {statusRequest.Status.ToLower()} for {request.FullName}",
            statusRequest.Status == "Approved" ? "Info" : "Warning"
        );

        return Ok(new { message = "Status updated successfully." });
    }

    [HttpPut("access-requests/{id}/role")]
    public IActionResult UpdateAccessRequestRole(int id, [FromBody] UpdateRoleRequest roleRequest)
    {
        var request = _accessRequestService.GetAccessRequestById(id);
        if (request == null)
        {
            return NotFound();
        }

        _accessRequestService.UpdateAccessRequestRole(id, roleRequest.Role);
        _auditLogService.LogAction(
            "AccessRequestRoleUpdated",
            request.Email,
            "admin@bio-isac.org",
            $"Role updated to {roleRequest.Role} for {request.FullName}",
            "Info"
        );

        return Ok(new { message = "Role updated successfully." });
    }

    [HttpPost("access-requests/{id}/request-info")]
    public IActionResult RequestMoreInfo(int id, [FromBody] RequestInfoRequest infoRequest)
    {
        var request = _accessRequestService.GetAccessRequestById(id);
        if (request == null)
        {
            return NotFound();
        }

        _accessRequestService.RequestMoreInfo(id, infoRequest.Notes);
        _auditLogService.LogAction(
            "AccessRequestMoreInfoRequested",
            request.Email,
            "admin@bio-isac.org",
            $"More information requested for {request.FullName}: {infoRequest.Notes}",
            "Info"
        );

        return Ok(new { message = "More information request sent successfully." });
    }

    [HttpGet("approved-members")]
    public IActionResult GetApprovedMembers()
    {
        var members = _approvedMemberService.GetAllApprovedMembers();
        return Ok(members);
    }

    [HttpPut("approved-members/{id}/role")]
    public IActionResult UpdateMemberRole(int id, [FromBody] UpdateRoleRequest roleRequest)
    {
        var member = _approvedMemberService.GetApprovedMemberById(id);
        if (member == null)
        {
            return NotFound();
        }

        _approvedMemberService.UpdateMemberRole(id, roleRequest.Role);
        _auditLogService.LogAction(
            "MemberRoleUpdated",
            member.Email,
            "admin@bio-isac.org",
            $"Member role updated to {roleRequest.Role} for {member.FullName}",
            "Info"
        );

        return Ok(new { message = "Member role updated successfully." });
    }

    [HttpPut("approved-members/{id}/deactivate")]
    public IActionResult DeactivateMember(int id)
    {
        var member = _approvedMemberService.GetApprovedMemberById(id);
        if (member == null)
        {
            return NotFound();
        }

        _approvedMemberService.DeactivateMember(id);
        _auditLogService.LogAction(
            "MemberDeactivated",
            member.Email,
            "admin@bio-isac.org",
            $"Member deactivated: {member.FullName}",
            "Warning"
        );

        return Ok(new { message = "Member deactivated successfully." });
    }

    [HttpGet("organizations")]
    public IActionResult GetOrganizations()
    {
        var organizations = _organizationService.GetAllOrganizations();
        return Ok(organizations);
    }

    [HttpPut("organizations/{id}/verification")]
    public IActionResult UpdateOrganizationVerification(int id, [FromBody] VerificationRequest verificationRequest)
    {
        var org = _organizationService.GetOrganizationById(id);
        if (org == null)
        {
            return NotFound();
        }

        _organizationService.UpdateOrganizationVerification(id, verificationRequest.IsVerified, verificationRequest.Status);
        _auditLogService.LogAction(
            "OrganizationVerificationUpdated",
            org.ContactEmail ?? "",
            "admin@bio-isac.org",
            $"Organization verification updated for {org.Name}: {verificationRequest.Status}",
            "Info"
        );

        return Ok(new { message = "Organization verification updated successfully." });
    }

    [HttpGet("audit-logs")]
    public IActionResult GetAuditLogs([FromQuery] string? severity = null, [FromQuery] string? userEmail = null)
    {
        List<AuditLog> logs;
        if (!string.IsNullOrWhiteSpace(severity))
        {
            logs = _auditLogService.GetAuditLogsBySeverity(severity);
        }
        else if (!string.IsNullOrWhiteSpace(userEmail))
        {
            logs = _auditLogService.GetAuditLogsByUser(userEmail);
        }
        else
        {
            logs = _auditLogService.GetAllAuditLogs();
        }

        return Ok(logs);
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class UpdateRoleRequest
{
    public string Role { get; set; } = string.Empty;
}

public class RequestInfoRequest
{
    public string Notes { get; set; } = string.Empty;
}

public class VerificationRequest
{
    public bool IsVerified { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

