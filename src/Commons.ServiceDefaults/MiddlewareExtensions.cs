using Audit.Core;
using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Reflection;

namespace Commons.ServiceDefaults
{
    public static partial class MiddlewareExtensions
    {
        public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // Default health checks assume the event bus and self health checks
            builder.AddBasicServiceDefaults();

            builder.Services.ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                //http.AddServiceDiscovery();
            });

            return builder;
        }

        public static IHostApplicationBuilder AddBasicServiceDefaults(this IHostApplicationBuilder builder)
        {
            builder.AddDefaulCulture();
            builder.AddDefaultHealthChecks();
            builder.AddDefaultSerilog();
            builder.AddDefaultCors();
            return builder;
        }

        public static IHostApplicationBuilder AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.AddDefaultAuditLogStore();
            builder.AddDefaultDataProtection();
            return builder;
        }

        private static IHostApplicationBuilder AddDefaulCulture(this IHostApplicationBuilder builder)
        {
            CustomRequestCultureProvider Provider =
                new CustomRequestCultureProvider(async (HttpContext) =>
                {
                    await Task.Yield();
                    CultureInfo CI = CultureInfo.InvariantCulture;  //new CultureInfo("en-US");
                    return new ProviderCultureResult(CI.Name);
                });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
                options.SupportedUICultures = options.SupportedCultures;

                //options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Insert(0, Provider);
            });

            return builder;
        }

        public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;

            services.Configure<HealthChecksOptions>(options =>
            {
                // Configuración adicional opcional
            });

            services.AddSingleton<IConfigureOptions<HealthChecksOptions>, ConfigureHealthChecksOptions>();

            services
                .AddHealthChecks()
                .AddApplicationStatus(name: "api_status", tags: new[] { "api" })
                // Add a default liveness check to ensure app is responsive
                .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"])
                .AddSqlServer(
                    connectionString: builder.Configuration.GetConnectionString("DefaultConnection")!,
                    healthQuery: "select 1",
                    name: "sql",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "db", "sql" }); ;

            return builder;
        }

        private static IHostApplicationBuilder AddDefaultSerilog(this IHostApplicationBuilder builder)
        {
            builder.Services
                .AddSerilog(lc => lc
                .ReadFrom.Configuration(builder.Configuration.GetSection("Serilog")));

            return builder;
        }

        private static IHostApplicationBuilder AddDefaultAuditLogStore(this IHostApplicationBuilder builder)
        {
            //Configuration.Setup()
            //    .UseAzureStorageBlobs(config => config
            //        .WithConnectionString(builder.Configuration["AuditLogs:ConnectionString"])
            //        .ContainerName(ev => $"mediatrlogs{DateTime.Today:yyyyMMdd}")
            //        .BlobName(ev =>
            //        {
            //            //var currentUser = ""; //ev.CustomFields["User"] as CurrentUser;
            //            //return $"{ev.EventType}/{currentUser?.Id}_{DateTime.UtcNow.Ticks}.json";
            //        })
            //    );

            return builder;
        }

        private static IHostApplicationBuilder AddDefaultDataProtection(this IHostApplicationBuilder builder)
        {
            builder.Services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration.GetValue<string>("DataProtection:DirectoryInfo")));

            return builder;
        }

        private static IHostApplicationBuilder AddDefaultCors(this IHostApplicationBuilder builder, string policy = "AllowAngularOrigins")
        {
            var configuration = builder.Configuration;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowAngularOrigins",
                    builder =>
                    {
                        builder.WithOrigins(configuration.GetValue<string>("Cors:Uri"))
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            return builder;
        }

        public static WebApplication MapDefaultEndpoints(this WebApplication app)
        {
            // Uncomment the following line to enable the Prometheus endpoint (requires the OpenTelemetry.Exporter.Prometheus.AspNetCore package)
            // app.MapPrometheusScrapingEndpoint();

            var healthChecksOptions = app.Services.GetService<IOptions<HealthChecksOptions>>()!.Value;

            // Adding health checks endpoints to applications in non-development environments has security implications.
            // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
            if (app.Environment.IsDevelopment())
            {
                // All health checks must pass for app to be considered ready to accept traffic after starting
                app.MapHealthChecks(healthChecksOptions.Path, new HealthCheckOptions
                {
                    Predicate = healthChecksOptions.Predicate,
                    ResponseWriter = healthChecksOptions.ResponseWriter,
                    AllowCachingResponses = healthChecksOptions.AllowCachingResponses,
                    //Timeout = healthChecksOptions.Timeout
                });

                // Only health checks tagged with the "live" tag must pass for app to be considered alive
                app.MapHealthChecks("/alive", new HealthCheckOptions
                {
                    Predicate = r => r.Tags.Contains("live")
                });

                app.MapHealthChecksUI();
            }

            return app;
        }
    }
}
