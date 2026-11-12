using Microsoft.Extensions.FileProviders;
using RealEstateApp.Core.Application;
using RealEstateApp.Infrastructure.Identity;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Registrar capas
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddIdentityForWebApp(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddApplicationLayer();

var app = builder.Build();

// Ejecutar Migraciones y Seeds
await app.Services.RunPersistenceSeedAsync();
await app.Services.RunIdentitySeedAsync();

app.UseHttpsRedirection();
app.UseStaticFiles();
var uploadsPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath,
                 builder.Configuration["Storage:UploadsPath"] ?? "../Uploads"));
Directory.CreateDirectory(uploadsPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();