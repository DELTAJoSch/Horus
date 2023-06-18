using NLog;
using NLog.Web;

namespace HorusAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Info("Building application...");

            var builder = WebApplication.CreateBuilder(args);
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            logger.Info("Application built.");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            logger.Info("Launching.");
            app.Run();

            logger.Info("Shutting down.");
            NLog.LogManager.Shutdown();
        }
    }
}