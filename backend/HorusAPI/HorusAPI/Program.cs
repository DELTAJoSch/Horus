using HorusAPI.Database;
using HorusAPI.Entities;
using HorusAPI.Service;
using HorusAPI.Service.Implementations;
using HorusAPI.Settings;
using HorusAPI.Startup;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;

namespace HorusAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var allowOriginsPolicy = "_allowOrigins";

            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Info("Building application...");

            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Horus API", Version = "v1" });
            });

            builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("MongoDb"));

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(name: allowOriginsPolicy,
                    policy =>
                    {
                        policy.AllowAnyHeader();
                        policy.AllowAnyMethod();
                        policy.AllowCredentials();
                        policy.SetIsOriginAllowed(hostName => true);
                    });
            });

            AddSingletons(builder.Services);

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = (context) =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    options.Cookie.Name = "session-identifier";
                    options.Cookie.HttpOnly= true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.IsEssential = true;
                });

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();
            logger.Info("Application built.");

            // Ensure startup constraints
            EnsurerExecutor.Ensure(app);
            logger.Info("Ensured all startup constraints.");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Remove for production!
            app.UseCors(allowOriginsPolicy);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            logger.Info("Launching.");
            app.Run();

            logger.Info("Shutting down.");
            NLog.LogManager.Shutdown();
        }

        /// <summary>
        /// Adds all singletons to the app
        /// </summary>
        /// <param name="services">The service list to add the singletons to</param>
        private static void AddSingletons(IServiceCollection services)
        {
            services.AddSingleton<ICrud<User>, UserDatabase>();
            services.AddSingleton<IUserService, UserService>();
        }
    }
}