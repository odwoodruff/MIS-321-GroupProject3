using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

public class AlertDecorator : IAlertDecorator
{
    public Alert Decorate(Alert alert)
    {
        // Add additional metadata or formatting
        var decorated = new Alert
        {
            Id = alert.Id,
            Title = alert.Title,
            Description = alert.Description,
            Sector = alert.Sector,
            DateCreated = alert.DateCreated,
            Severity = alert.Severity,
            Source = alert.Source,
            Tags = new List<string>(alert.Tags)
        };

        // Add time-based decorations
        var timeSinceCreation = DateTime.Now - alert.DateCreated;
        if (timeSinceCreation.TotalHours < 24)
        {
            decorated.Tags.Add("Recent");
        }

        // Add severity-based decorations
        if (alert.Severity == "Critical" || alert.Severity == "High")
        {
            decorated.Tags.Add("Priority");
        }

        return decorated;
    }

    public List<Alert> DecorateAll(List<Alert> alerts)
    {
        return alerts.Select(Decorate).ToList();
    }
}

