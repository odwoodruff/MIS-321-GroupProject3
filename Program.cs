using MIS_GroupProject3.Services;
using MIS_GroupProject3.Models;
using MIS_GroupProject3.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSingleton<IAlertService, AlertService>();
builder.Services.AddSingleton<IAlertFilterStrategy, SectorFilterStrategy>();
builder.Services.AddSingleton<IAlertAdapter, AlertAdapter>();
builder.Services.AddSingleton<IAlertDecorator, AlertDecorator>();
builder.Services.AddSingleton<HtmlRenderer>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
