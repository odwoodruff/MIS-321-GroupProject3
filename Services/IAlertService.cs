using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

public interface IAlertService
{
    List<Alert> GetAllAlerts();
    List<Alert> GetAlertsBySector(string sector);
    DashboardMetrics GetDashboardMetrics();
    Alert? GetAlertById(int id);
}

