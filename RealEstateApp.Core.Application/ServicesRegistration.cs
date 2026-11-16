using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Core.Domain.Interfaces;
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
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IAgentManagementService, AgentManagementService>();
            services.AddScoped<IAdministratorService, AdministratorService>();
            services.AddScoped<IDeveloperService, DeveloperService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
            services.AddScoped<IAgentProfileService, AgentProfileService>();
            services.AddScoped<IAgentService, AgentService>();
            services.AddScoped<IPropertyTypeService, PropertyTypeService>();
            services.AddScoped<ISaleTypeService, SaleTypeService>();
            services.AddScoped<IPropertyImprovementService, ImprovementService>();
        }
    }
}