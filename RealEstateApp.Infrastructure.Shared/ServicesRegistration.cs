using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;

namespace RealEstateApp.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar MailSettings
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));

            // Registrar Servicios
            services.AddTransient<IEmailService, Services.EmailService>();
            services.AddSingleton<IFileStorage, Services.LocalFileStorage>();
        }
    }
}