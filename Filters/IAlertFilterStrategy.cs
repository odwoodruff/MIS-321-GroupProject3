using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Filters;

// Strategy Pattern: Different filtering strategies for different sectors
public interface IAlertFilterStrategy
{
    List<Alert> Filter(List<Alert> alerts, string? sector = null);
    bool MatchesSector(Alert alert, string sector);
}

