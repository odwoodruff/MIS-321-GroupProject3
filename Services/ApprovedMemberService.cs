using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

public interface IApprovedMemberService
{
    void AddApprovedMember(ApprovedMember member);
    List<ApprovedMember> GetAllApprovedMembers();
    ApprovedMember? GetApprovedMemberById(int id);
    ApprovedMember? GetApprovedMemberByEmail(string email);
    void UpdateMemberRole(int id, string role);
    void DeactivateMember(int id);
    void UpdateLastLogin(string email);
}

public class ApprovedMemberService : IApprovedMemberService
{
    private readonly List<ApprovedMember> _approvedMembers;
    private int _nextId = 1;

    public ApprovedMemberService()
    {
        _approvedMembers = new List<ApprovedMember>();
    }

    public void AddApprovedMember(ApprovedMember member)
    {
        member.Id = _nextId++;
        member.ApprovalDate = DateTime.Now;
        member.IsActive = true;
        _approvedMembers.Add(member);
    }

    public List<ApprovedMember> GetAllApprovedMembers()
    {
        return _approvedMembers.OrderByDescending(m => m.ApprovalDate).ToList();
    }

    public ApprovedMember? GetApprovedMemberById(int id)
    {
        return _approvedMembers.FirstOrDefault(m => m.Id == id);
    }

    public ApprovedMember? GetApprovedMemberByEmail(string email)
    {
        return _approvedMembers.FirstOrDefault(m => m.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public void UpdateMemberRole(int id, string role)
    {
        var member = GetApprovedMemberById(id);
        if (member != null)
        {
            member.AssignedRole = role;
        }
    }

    public void DeactivateMember(int id)
    {
        var member = GetApprovedMemberById(id);
        if (member != null)
        {
            member.IsActive = false;
        }
    }

    public void UpdateLastLogin(string email)
    {
        var member = GetApprovedMemberByEmail(email);
        if (member != null)
        {
            member.LastLoginDate = DateTime.Now;
        }
    }
}

