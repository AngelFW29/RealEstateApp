using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using RealEstateApp.Core.Application;
using RealEstateApp.Infrastructure.Identity;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Shared;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Registrar MVC
builder.Services.AddControllersWithViews();

// Registrar capas
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddIdentityForWebApp(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddApplicationLayer();

// Configurar cookie de Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Tamaño máximo de archivos subidos (20 MB)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 20 * 1024 * 1024;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 20 * 1024 * 1024;
});

// Registro explícito de IAccountService (necesario para StaffUserService y derivados)
builder.Services.AddScoped<RealEstateApp.Core.Application.Interfaces.IAccountService,
                           RealEstateApp.Infrastructure.Identity.Services.UserService>();

var app = builder.Build();

// Configurar cultura es-DO (decimales con punto, precios, etc.)
var supportedCultures = new[] { new CultureInfo("es-DO") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es-DO"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Ejecutar Migraciones y Seeds
await app.Services.RunPersistenceSeedAsync();
await app.Services.RunIdentitySeedAsync();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Servir /uploads desde Uploads/ (fuera de wwwroot)
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