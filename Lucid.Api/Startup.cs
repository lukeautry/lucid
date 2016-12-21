using System.Data;
using Lucid.Core;
using Lucid.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Lucid.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
			// Add framework services
			services.AddApplicationInsightsTelemetry(Configuration);
			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials());
			});

			services.AddMvc();

			services.AddScoped<IRedisProvider, RedisProvider>();
			services.AddTransient(typeof(IDbConnection), p =>
			{
				var connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;User Id = postgres;Password=admin;Database=lucid");
				connection.Open();

				return connection;
			});

			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IAreaRepository, AreaRepository>();
			services.AddScoped<IRoomRepository, RoomRepository>();
			services.AddSwaggerGen();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
			app.UseCors("CorsPolicy");
			app.UseMvc();
			app.UseSwagger();
        }
    }
}
