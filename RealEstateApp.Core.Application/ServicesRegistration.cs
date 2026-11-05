using Microsoft.Extensions.DependencyInjection;
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

        }
    }
}