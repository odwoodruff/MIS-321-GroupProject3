using MIS_GroupProject3.Services;
using MIS_GroupProject3.Models;
using MIS_GroupProject3.Filters;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization to use camelCase to match JavaScript expectations
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true; // Pretty print for debugging
    });
builder.Services.AddSingleton<IAlertService, AlertService>();
builder.Services.AddSingleton<IAlertFilterStrategy, SectorFilterStrategy>();
builder.Services.AddSingleton<IAlertAdapter, AlertAdapter>();
builder.Services.AddSingleton<IAlertDecorator, AlertDecorator>();
builder.Services.AddSingleton<IAccessRequestService, AccessRequestService>();
builder.Services.AddSingleton<IApprovedMemberService, ApprovedMemberService>();
builder.Services.AddSingleton<IOrganizationService, OrganizationService>();
builder.Services.AddSingleton<IAuditLogService, AuditLogService>();
builder.Services.AddSingleton<HtmlRenderer>();

// Add CORS to allow API calls from the webpage
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Serve static files from frontend folder
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "frontend")),
    RequestPath = "/frontend"
});

app.UseCors();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
