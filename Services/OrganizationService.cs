using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

public interface IOrganizationService
{
    void AddOrganization(Organization org);
    List<Organization> GetAllOrganizations();
    Organization? GetOrganizationById(int id);
    Organization? GetOrganizationByDomain(string domain);
    void UpdateOrganizationVerification(int id, bool isVerified, string status);
    void IncrementMemberCount(string domain);
}

public class OrganizationService : IOrganizationService
{
    private readonly List<Organization> _organizations;
    private int _nextId = 1;

    public OrganizationService()
    {
        _organizations = new List<Organization>();
    }

    public void AddOrganization(Organization org)
    {
        org.Id = _nextId++;
        org.RegistrationDate = DateTime.Now;
        _organizations.Add(org);
    }

    public List<Organization> GetAllOrganizations()
    {
        return _organizations.OrderBy(o => o.Name).ToList();
    }

    public Organization? GetOrganizationById(int id)
    {
        return _organizations.FirstOrDefault(o => o.Id == id);
    }

    public Organization? GetOrganizationByDomain(string domain)
    {
        return _organizations.FirstOrDefault(o => o.Domain.Equals(domain, StringComparison.OrdinalIgnoreCase));
    }

    public void UpdateOrganizationVerification(int id, bool isVerified, string status)
    {
        var org = GetOrganizationById(id);
        if (org != null)
        {
            org.IsVerified = isVerified;
            org.VerificationStatus = status;
        }
    }

    public void IncrementMemberCount(string domain)
    {
        var org = GetOrganizationByDomain(domain);
        if (org != null)
        {
            org.MemberCount++;
        }
    }
}

