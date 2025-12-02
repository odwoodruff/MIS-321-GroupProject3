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
        _layoutPath = Path.Combine(contentRoot, "frontend", "layout.html");
        _htmlPath = Path.Combine(contentRoot, "frontend");
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

        var fullPage = File.ReadAllText(pagePath);

        // Extract scripts from content if any
        var scripts = ExtractScripts(fullPage);
        if (!string.IsNullOrEmpty(additionalScripts))
        {
            scripts = additionalScripts + "\n" + scripts;
        }

        // Extract inline styles from content
        var inlineStyles = ExtractInlineStyles(fullPage);

        // Extract only the main content (between <main> tags or body content)
        var content = ExtractMainContent(fullPage);

        // Replace placeholders
        var html = layout
            .Replace("{{TITLE}}", title)
            .Replace("{{CONTENT}}", content)
            .Replace("{{SCRIPTS}}", scripts)
            .Replace("{{INLINE_STYLES}}", inlineStyles);

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
            // Convert relative paths to absolute paths (if not already absolute or external)
            if (!src.StartsWith("http://") && !src.StartsWith("https://") && !src.StartsWith("/"))
            {
                if (src.StartsWith("js/") || src.StartsWith("css/"))
                {
                    src = "/" + src;
                }
            }
            scripts.AppendLine($"<script src=\"{src}\"></script>");
        }

        return scripts.ToString();
    }

    private string ExtractMainContent(string html)
    {
        // Try to extract content from <main> tag first
        var mainPattern = @"<main[^>]*>(.*?)</main>";
        var mainMatch = System.Text.RegularExpressions.Regex.Match(html, mainPattern, System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        if (mainMatch.Success)
        {
            return mainMatch.Groups[1].Value.Trim();
        }

        // If no <main> tag, try to extract content from <body> tag (excluding nav and footer)
        var bodyPattern = @"<body[^>]*>(.*?)</body>";
        var bodyMatch = System.Text.RegularExpressions.Regex.Match(html, bodyPattern, System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        if (bodyMatch.Success)
        {
            var bodyContent = bodyMatch.Groups[1].Value;
            // Remove nav, footer, script tags, and style tags (styles are extracted separately)
            bodyContent = System.Text.RegularExpressions.Regex.Replace(bodyContent, @"<nav[^>]*>.*?</nav>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            bodyContent = System.Text.RegularExpressions.Regex.Replace(bodyContent, @"<footer[^>]*>.*?</footer>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            bodyContent = RemoveScriptTags(bodyContent);
            bodyContent = System.Text.RegularExpressions.Regex.Replace(bodyContent, @"<style[^>]*>.*?</style>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return bodyContent.Trim();
        }

        // If no body tag, remove HTML structure and return inner content
        var cleaned = html;
        // Remove DOCTYPE, html, head tags
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"<!DOCTYPE[^>]*>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"<html[^>]*>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"</html>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"<head[^>]*>.*?</head>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"<body[^>]*>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"</body>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"<nav[^>]*>.*?</nav>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"<footer[^>]*>.*?</footer>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        cleaned = RemoveScriptTags(cleaned);
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"<style[^>]*>.*?</style>", "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        
        return cleaned.Trim();
    }

    private string ExtractInlineStyles(string html)
    {
        var styles = new System.Text.StringBuilder();
        // Extract <style> tags
        var stylePattern = @"<style[^>]*>(.*?)</style>";
        var matches = System.Text.RegularExpressions.Regex.Matches(html, stylePattern, System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var styleContent = match.Groups[1].Value;
            styles.AppendLine($"<style>{styleContent}</style>");
        }

        return styles.ToString();
    }

    private string RemoveScriptTags(string html)
    {
        var scriptPattern = @"<script[^>]*>.*?</script>";
        return System.Text.RegularExpressions.Regex.Replace(html, scriptPattern, "", System.Text.RegularExpressions.RegexOptions.Singleline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}

