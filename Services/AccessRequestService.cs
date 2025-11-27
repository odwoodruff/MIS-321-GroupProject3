using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

public class AccessRequestService : IAccessRequestService
{
    private readonly List<AccessRequest> _accessRequests;
    private int _nextId = 1;

    public AccessRequestService()
    {
        _accessRequests = new List<AccessRequest>();
    }

    public void SubmitAccessRequest(AccessRequest request)
    {
        request.Id = _nextId++;
        request.RequestDate = DateTime.Now;
        request.Status = "Pending";
        
        // Calculate domain trust score
        request.DomainTrustScore = CalculateDomainTrustScore(request.Email);
        
        // Set email confirmation status
        request.EmailConfirmationStatus = "Pending";
        
        _accessRequests.Add(request);
    }

    public List<AccessRequest> GetAllAccessRequests()
    {
        return _accessRequests.OrderByDescending(r => r.RequestDate).ToList();
    }

    public AccessRequest? GetAccessRequestById(int id)
    {
        return _accessRequests.FirstOrDefault(r => r.Id == id);
    }

    public void UpdateAccessRequestStatus(int id, string status)
    {
        var request = GetAccessRequestById(id);
        if (request != null)
        {
            request.Status = status;
        }
    }

    public void UpdateAccessRequestRole(int id, string role)
    {
        var request = GetAccessRequestById(id);
        if (request != null)
        {
            request.AssignedRole = role;
        }
    }

    public void RequestMoreInfo(int id, string notes)
    {
        var request = GetAccessRequestById(id);
        if (request != null)
        {
            request.Status = "MoreInfoRequested";
            request.VerificationNotes = notes;
        }
    }

    private string CalculateDomainTrustScore(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            return "Unknown";

        var domain = email.Split('@').LastOrDefault()?.ToLower() ?? "";
        
        // High trust domains
        var highTrustDomains = new[] { ".gov", ".edu", ".mil" };
        if (highTrustDomains.Any(d => domain.EndsWith(d)))
            return "High";

        // Medium trust - known organization domains
        var mediumTrustPatterns = new[] { ".org", ".com" };
        if (mediumTrustPatterns.Any(p => domain.EndsWith(p)) && !IsPersonalEmailDomain(domain))
            return "Medium";

        // Low trust - personal email domains
        if (IsPersonalEmailDomain(domain))
            return "Low";

        return "Unknown";
    }

    private bool IsPersonalEmailDomain(string domain)
    {
        var personalDomains = new[] { "gmail.com", "hotmail.com", "yahoo.com", "outlook.com", "icloud.com", "aol.com" };
        return personalDomains.Contains(domain);
    }
}

