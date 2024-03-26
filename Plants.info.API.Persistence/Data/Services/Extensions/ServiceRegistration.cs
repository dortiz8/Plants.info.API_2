using System;
using Microsoft.Extensions.DependencyInjection;
using Plants.info.API.Data;
using Plants.info.API.Data.Repository;
using Plants.info.API.Data.Services;
using Plants.info.API.Persistence.Data.Repositories;
using Plants.info.API.Persistence.Data.Repositories.Interfaces;

namespace Plants.info.API.Persistence.Data.Services.Extensions
{
	public static class ServiceRegistration
	{
        public static void AddPersistenceInfrastructure(
             this IServiceCollection services)
        {
            services.AddScoped<IPlantsRepository, PlantsRepository>();
            services.AddScoped<IUserRepository, UserRepository>(); // one instance per request
            services.AddScoped<IMenusRepository, MenusRepository>(); // one instance per request
            services.AddScoped<IAppAuditRepository, AppAuditRepository>(); // one instance per request

            services.AddTransient<UserSeeder>(); // new instance is created per request
        }     
    }
}

