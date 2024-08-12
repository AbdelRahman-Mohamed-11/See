using Application.Interfaces;
using Application.Services;
using Application.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IProductService, ProductServices>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<DeliveryCostPlanInterface, 
                DeliveryCostPlanService>();

            services.AddScoped<ICityService , CityService>();

            services.AddScoped<IReviewService , ReviewService>();

            services.AddScoped<IPrescriptionService , 
                PrescriptionService>();

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });

            return services;
        }
    }
}
