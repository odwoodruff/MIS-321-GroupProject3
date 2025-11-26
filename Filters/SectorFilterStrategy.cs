using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Filters;

public class SectorFilterStrategy : IAlertFilterStrategy
{
    public List<Alert> Filter(List<Alert> alerts, string? sector = null)
    {
        if (string.IsNullOrWhiteSpace(sector) || sector == "All")
        {
            return alerts;
        }

        return alerts.Where(a => MatchesSector(a, sector)).ToList();
    }

    public bool MatchesSector(Alert alert, string sector)
    {
        return alert.Sector.Equals(sector, StringComparison.OrdinalIgnoreCase);
    }
}

