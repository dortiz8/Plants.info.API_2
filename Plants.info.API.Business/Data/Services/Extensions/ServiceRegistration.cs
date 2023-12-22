using System;
using Microsoft.Extensions.DependencyInjection; 
using Plants.info.API.Data.Services.PlantServices;
using Plants.info.API.Data.Services.PlantServices.Interfaces;
using Plants.info.API.Data.Services.UserServices;

namespace Plants.info.API.Data.Services.Extensions
{
	public static class ServiceRegistration
	{

		public static void  AddBusinessInfrastructure(
			 this IServiceCollection services)
		{
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<IPlantsService, PlantsService>();
            services.AddScoped<IUserService, UserService>();
        }
	}
}; 

