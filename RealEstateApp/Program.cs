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

var app = builder.Build();

// Ejecutar Migraciones y Seeds
await app.Services.RunPersistenceSeedAsync();
await app.Services.RunIdentitySeedAsync();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();