using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Services;

// Decorator Pattern: Enhance alerts with additional functionality
public interface IAlertDecorator
{
    Alert Decorate(Alert alert);
    List<Alert> DecorateAll(List<Alert> alerts);
}

