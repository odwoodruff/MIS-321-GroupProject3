namespace MIS_GroupProject3.Models;

public class DashboardMetrics
{
    public int TotalAlerts { get; set; }
    public int FilteredAlerts { get; set; }
    public int AgricultureAlerts { get; set; }
    public int HospitalAlerts { get; set; }
    public int PharmaAlerts { get; set; }
    public int OtherAlerts { get; set; }
    public DateTime LastUpdated { get; set; }
}

