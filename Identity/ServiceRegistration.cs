using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RealEstateApp.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        public static void AddIdentityLayerIocForWebApp(this IServiceCollection services, IConfiguration config) {}
        public static void AddIdentityLayerIocForWebApi(this IServiceCollection services, IConfiguration config) { }
        public static async Task IdentitySeedAsync(this IServiceCollection services) { }
    }
}