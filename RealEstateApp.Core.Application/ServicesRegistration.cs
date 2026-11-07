using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;
using System.Reflection;

namespace RealEstateApp.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            // Configuration automapper
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());

            // Services IOC
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
        }
    }
}