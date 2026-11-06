using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Infrastructure.Identity.Contexts;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Identity.Services;


namespace RealEstateApp.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        private static void AddContext(IServiceCollection services, IConfiguration config) =>
            services.AddDbContext<IdentityContext>(opts =>
                opts.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                    m => m.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

        public static void AddIdentityForWebApp(this IServiceCollection services, IConfiguration config)
        {
            AddContext(services, config);

            services.AddIdentity<AppUser, IdentityRole>(opts =>
            {
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 6;
                opts.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opts =>
            {
                opts.LoginPath = "/Account/Login";
                opts.AccessDeniedPath = "/Account/AccessDenied";
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWebAccountService, AccountServiceForWebApp>();
        }

        public static void AddIdentityForWebApi(this IServiceCollection services, IConfiguration config)
        {
            AddContext(services, config);

            services.AddIdentityCore<AppUser>(opts =>
            {
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequiredLength = 6;
                opts.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(config.GetSection("JWTSettings"));
            var jwtSettings = config.GetSection("JWTSettings").Get<JwtSettings>()!;

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.RequireHttpsMetadata = false;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IApiAccountService, AccountServiceForWebApi>();
        }

        public static async Task RunIdentitySeedAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityContext>();
            await context.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            // 1. Crear Roles
            foreach (var role in new[] { Roles.Admin, Roles.Developer, Roles.Agent, Roles.Client })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Crear Usuarios
            async Task SeedUser(string userName, string firstName, string lastName, string idCard, string role)
            {
                if (await userManager.FindByNameAsync(userName) == null)
                {
                    var user = new AppUser
                    {
                        UserName = userName,
                        Email = $"{userName}@realestate.com",
                        FirstName = firstName,
                        LastName = lastName,
                        IdCard = idCard,
                        IsActive = true,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, "Pa$$word123");
                    if (result.Succeeded) await userManager.AddToRoleAsync(user, role);
                }
            }

            await SeedUser("admin", "Admin", "Sistema", "001-0000000-1", Roles.Admin);
            await SeedUser("developer", "Dev", "Api", "001-0000000-2", Roles.Developer);
            await SeedUser("agent", "Agente", "Demo", "001-0000000-3", Roles.Agent);
            await SeedUser("client", "Cliente", "Demo", "001-0000000-4", Roles.Client);
        }
    }
}