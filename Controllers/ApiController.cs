using Microsoft.AspNetCore.Mvc;
using MIS_GroupProject3.Services;
using MIS_GroupProject3.Models;

namespace MIS_GroupProject3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    private readonly IAlertService _alertService;
    private readonly IAlertDecorator _alertDecorator;

    public ApiController(IAlertService alertService, IAlertDecorator alertDecorator)
    {
        _alertService = alertService;
        _alertDecorator = alertDecorator;
    }

    [HttpGet("alerts")]
    public IActionResult GetAlerts([FromQuery] string? sector = null)
    {
        var alerts = string.IsNullOrWhiteSpace(sector) || sector == "All"
            ? _alertService.GetAllAlerts()
            : _alertService.GetAlertsBySector(sector);

        var decoratedAlerts = _alertDecorator.DecorateAll(alerts);
        return Ok(decoratedAlerts);
    }

    [HttpGet("metrics")]
    public IActionResult GetMetrics()
    {
        var metrics = _alertService.GetDashboardMetrics();
        return Ok(metrics);
    }

    [HttpGet("alerts/{id}")]
    public IActionResult GetAlert(int id)
    {
        var alert = _alertService.GetAlertById(id);
        if (alert == null)
        {
            return NotFound();
        }

        var decoratedAlert = _alertDecorator.Decorate(alert);
        return Ok(decoratedAlert);
    }
}

