using MIS_GroupProject3.Models;
using MIS_GroupProject3.Filters;

namespace MIS_GroupProject3.Services;

public class AlertService : IAlertService
{
    private readonly List<Alert> _alerts;
    private readonly IAlertFilterStrategy _filterStrategy;

    public AlertService(IAlertFilterStrategy filterStrategy)
    {
        _filterStrategy = filterStrategy;
        _alerts = GenerateSampleAlerts();
    }

    public List<Alert> GetAllAlerts()
    {
        return _alerts;
    }

    public List<Alert> GetAlertsBySector(string sector)
    {
        return _filterStrategy.Filter(_alerts, sector);
    }

    public DashboardMetrics GetDashboardMetrics()
    {
        return new DashboardMetrics
        {
            TotalAlerts = _alerts.Count,
            FilteredAlerts = _alerts.Count,
            AgricultureAlerts = _alerts.Count(a => a.Sector.Equals("Agriculture", StringComparison.OrdinalIgnoreCase)),
            HospitalAlerts = _alerts.Count(a => a.Sector.Equals("Hospitals", StringComparison.OrdinalIgnoreCase)),
            PharmaAlerts = _alerts.Count(a => a.Sector.Equals("Pharma", StringComparison.OrdinalIgnoreCase)),
            OtherAlerts = _alerts.Count(a => !new[] { "Agriculture", "Hospitals", "Pharma" }
                .Contains(a.Sector, StringComparer.OrdinalIgnoreCase)),
            LastUpdated = DateTime.Now
        };
    }

    public Alert? GetAlertById(int id)
    {
        return _alerts.FirstOrDefault(a => a.Id == id);
    }

    private List<Alert> GenerateSampleAlerts()
    {
        return new List<Alert>
        {
            new Alert
            {
                Id = 1,
                Title = "Ransomware Attack on Agricultural Data Systems",
                Description = "Multiple agricultural facilities reported ransomware attacks targeting crop management systems.",
                Sector = "Agriculture",
                DateCreated = DateTime.Now.AddDays(-2),
                Severity = "High",
                Source = "CISA",
                Tags = new List<string> { "Ransomware", "Data Breach", "Critical" }
            },
            new Alert
            {
                Id = 2,
                Title = "Phishing Campaign Targeting Hospital Staff",
                Description = "Sophisticated phishing emails targeting hospital administrative staff to gain access to patient records.",
                Sector = "Hospitals",
                DateCreated = DateTime.Now.AddDays(-1),
                Severity = "High",
                Source = "FBI",
                Tags = new List<string> { "Phishing", "Social Engineering", "HIPAA" }
            },
            new Alert
            {
                Id = 3,
                Title = "API Vulnerability in Pharmaceutical Research Platforms",
                Description = "Critical API vulnerability discovered in pharmaceutical research data platforms that could expose proprietary research data.",
                Sector = "Pharma",
                DateCreated = DateTime.Now.AddHours(-12),
                Severity = "Critical",
                Source = "NIST",
                Tags = new List<string> { "API", "Vulnerability", "Research Data" }
            },
            new Alert
            {
                Id = 4,
                Title = "Supply Chain Attack on Medical Device Manufacturers",
                Description = "Supply chain compromise affecting multiple medical device manufacturers, potentially impacting device security.",
                Sector = "Hospitals",
                DateCreated = DateTime.Now.AddHours(-6),
                Severity = "High",
                Source = "DHS",
                Tags = new List<string> { "Supply Chain", "Medical Devices", "Infrastructure" }
            },
            new Alert
            {
                Id = 5,
                Title = "IoT Security Flaw in Agricultural Monitoring Systems",
                Description = "Security vulnerability in IoT sensors used for agricultural monitoring, allowing unauthorized access to farm data.",
                Sector = "Agriculture",
                DateCreated = DateTime.Now.AddHours(-3),
                Severity = "Medium",
                Source = "USDA",
                Tags = new List<string> { "IoT", "Sensors", "Data Privacy" }
            },
            new Alert
            {
                Id = 6,
                Title = "Data Exfiltration Attempt on Clinical Trial Systems",
                Description = "Attempted unauthorized access to clinical trial management systems containing sensitive patient data.",
                Sector = "Pharma",
                DateCreated = DateTime.Now.AddHours(-1),
                Severity = "High",
                Source = "FDA",
                Tags = new List<string> { "Data Exfiltration", "Clinical Trials", "Patient Data" }
            }
        };
    }
}

