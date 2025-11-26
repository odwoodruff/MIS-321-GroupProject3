using Microsoft.AspNetCore.Hosting;

namespace MIS_GroupProject3.Services;

public class HtmlRenderer
{
    private readonly IWebHostEnvironment _environment;
    private readonly string _layoutPath;
    private readonly string _htmlPath;

    public HtmlRenderer(IWebHostEnvironment environment)
    {
        _environment = environment;
        var contentRoot = _environment.ContentRootPath;
        _layoutPath = Path.Combine(contentRoot, "Views", "Shared", "_Layout.html");
        _htmlPath = Path.Combine(contentRoot, "Views", "Home");
    }

    public string RenderPage(string pageName, string title, string? additionalScripts = null)
    {
        // Read layout template
        var layout = File.ReadAllText(_layoutPath);

        // Read page content
        var pagePath = Path.Combine(_htmlPath, $"{pageName}.html");
        if (!File.Exists(pagePath))
        {
            throw new FileNotFoundException($"Page {pageName}.html not found");
        }

        var content = File.ReadAllText(pagePath);

        // Extract scripts from content if any
        var scripts = ExtractScripts(content);
        if (!string.IsNullOrEmpty(additionalScripts))
        {
            scripts = additionalScripts + "\n" + scripts;
        }

        // Remove script tags from content
        content = RemoveScriptTags(content);

        // Replace placeholders
        var html = layout
            .Replace("{{TITLE}}", title)
            .Replace("{{CONTENT}}", content)
            .Replace("{{SCRIPTS}}", scripts);

        return html;
    }

    private string ExtractScripts(string html)
    {
        var scripts = new System.Text.StringBuilder();
        var scriptPattern = @"<script[^>]*src=[""']([^""']+)[""'][^>]*></script>";
        var matches = System.Text.RegularExpressions.Regex.Matches(html, scriptPattern);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var src = match.Groups[1].Value;
            scripts.AppendLine($"<script src=\"{src}\"></script>");
        }

        return scripts.ToString();
    }

    private string RemoveScriptTags(string html)
    {
        var scriptPattern = @"<script[^>]*>.*?</script>";
        return System.Text.RegularExpressions.Regex.Replace(html, scriptPattern, "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}

