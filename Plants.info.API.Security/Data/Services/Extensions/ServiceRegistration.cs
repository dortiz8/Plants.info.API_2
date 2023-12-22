using System;
using Microsoft.Extensions.DependencyInjection;
using Plants.info.API.Data.Services.JwtFeatures;
using Plants.info.API.Data.Services.JwtFeatures.Interfaces;

namespace Plants.info.API.Security.Data.Services.Extensions
{
	public static class ServiceRegistration
	{
        public static void AddSecurityInfrastructure(
             this IServiceCollection services)
        {
         
            services.AddScoped<IJwtHandler, JwtHandler>();
           
            services.AddScoped<IJwtService, JwtService>(); 
        }
    }
}

