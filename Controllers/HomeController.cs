using Microsoft.AspNetCore.Mvc;
using MIS_GroupProject3.Services;

namespace MIS_GroupProject3.Controllers;

public class HomeController : Controller
{
    private readonly IAlertService _alertService;
    private readonly IAlertDecorator _alertDecorator;
    private readonly HtmlRenderer _htmlRenderer;

    public HomeController(IAlertService alertService, IAlertDecorator alertDecorator, HtmlRenderer htmlRenderer)
    {
        _alertService = alertService;
        _alertDecorator = alertDecorator;
        _htmlRenderer = htmlRenderer;
    }

    public IActionResult Index()
    {
        var html = _htmlRenderer.RenderPage("index", "Home");
        return Content(html, "text/html");
    }

    public IActionResult Dashboard()
    {
        var html = _htmlRenderer.RenderPage("dashboard", "Dashboard");
        return Content(html, "text/html");
    }

    public IActionResult SectorFilters()
    {
        var html = _htmlRenderer.RenderPage("sectorFilters", "Sector Filters");
        return Content(html, "text/html");
    }

    public IActionResult AlertRepository()
    {
        var html = _htmlRenderer.RenderPage("alertRepository", "Alert Repository");
        return Content(html, "text/html");
    }

    public IActionResult AdminReviewTools()
    {
        var html = _htmlRenderer.RenderPage("adminReviewTools", "Admin Review Tools");
        return Content(html, "text/html");
    }

    public IActionResult SecurityCheck()
    {
        var html = _htmlRenderer.RenderPage("securityCheck", "Security Check");
        return Content(html, "text/html");
    }

    public IActionResult AdminPanel()
    {
        var html = _htmlRenderer.RenderPage("adminPanel", "Admin Panel");
        return Content(html, "text/html");
    }
}

