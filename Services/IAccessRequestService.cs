using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

public interface IAccessRequestService
{
    void SubmitAccessRequest(AccessRequest request);
    List<AccessRequest> GetAllAccessRequests();
    AccessRequest? GetAccessRequestById(int id);
    void UpdateAccessRequestStatus(int id, string status);
    void UpdateAccessRequestRole(int id, string role);
    void RequestMoreInfo(int id, string notes);
}

