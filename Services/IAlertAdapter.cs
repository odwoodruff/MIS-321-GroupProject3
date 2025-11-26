using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

// Adapter Pattern: Adapt different alert sources to a common format
public interface IAlertAdapter
{
    Alert AdaptExternalAlert(object externalAlert);
    List<Alert> AdaptExternalAlerts(List<object> externalAlerts);
}

