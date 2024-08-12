using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using StackExchange.Redis;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using API.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.interfaces;
using Infrastructure.Services;
using Core.DTOS.Email;
using Core.DTOS.Basket;
using CorePush.Apple;
using CorePush.Google;
using X.Paymob.CashIn;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("Default"));
            });
            services.AddEndpointsApiExplorer();
            
            services.AddSwaggerGen();

            
            services.AddScoped<IJwtServices, JwtServices>();

            // you must make it dynamic
            services.AddScoped<IRedisService<UserTokenVerificationEmail>
                , RedisService>();


            services.AddScoped<IRedisService<CustomerBasket>,
                RedisBasket>();

            services.AddTransient<INotificationService, 
                NotificationService>();
            
            services.AddHttpClient<FcmSender>();
            

            services.AddHttpClient<ApnSender>();



            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ActionContext =>  // determine what happens when model state is invalid
                {
                    var errors = ActionContext.ModelState
                                .Where(x => x.Value.Errors.Count > 0)
                                .SelectMany(x => x.Value.Errors)
                                .Select(x => x.ErrorMessage)
                                .ToList();

                    var response = new ApiValidationException { Errors = errors };  // automatic call constrcutor of apiValidation

                    return new BadRequestObjectResult(response);   // return as json
                                                                   // BadRequestObjectResult allow us to return an object as response with the statusCode 400

                };
            });

            services.AddSingleton<IConnectionMultiplexer>(c => {   // it's the connection of the redis server

            var options = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"));

            return ConnectionMultiplexer.Connect(options);
                
            });


            services.AddIdentity<ApplicationUser, ApplicationRole>
                (options =>
            {
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                var Key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]!);
                o.RequireHttpsMetadata = false;  //disabled in development
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ClockSkew = TimeSpan.Zero // the library itself has 5 minutes delay for expiration date for stop that we use this line
                };


            }).AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = configuration["Facebook:AppId"];
                facebookOptions.AppSecret = configuration["Facebook:AppSecret"];
                facebookOptions.AccessDeniedPath = "/api/users/access-denied";
                facebookOptions.Scope.Add("email"); // Include 'email' in the scope
                facebookOptions.SaveTokens = true;
            });


            services.AddScoped<ICacheService , CacheService>();


            services.AddAuthorization();



            services.AddLogging();

            services.AddHttpClient();

            services.AddSwaggerDocumentation();

            return services;
        }
    }
}
