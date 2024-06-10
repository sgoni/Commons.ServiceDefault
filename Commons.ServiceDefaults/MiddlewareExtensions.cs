using Commons.ServiceDefaults.Options;
using Commons.ServiceDefaults.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Commons.ServiceDefaults
{

    public static partial class MiddlewareExtensions
    {
        public static IServiceCollection AddDefaulCulture(this IServiceCollection services)
        {
            CustomRequestCultureProvider Provider =
                new CustomRequestCultureProvider(async (HttpContext) =>
                {
                    await Task.Yield();
                    CultureInfo CI = CultureInfo.InvariantCulture;  //new CultureInfo("en-US");
                    return new ProviderCultureResult(CI.Name);
                });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
                options.SupportedUICultures = options.SupportedCultures;

                //options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Insert(0, Provider);
            });

            return services;
        }

        public static IServiceCollection AddDefaultAuditLogStore(this IServiceCollection services)
        {
            services.Configure<AuditLogStoreOptions>(options =>
            {
                // Configuración opcional adicional
                options.TableName = "AuditLogs";
            });

            services.AddSingleton<IConfigureOptions<AuditLogStoreOptions>, ConfigureAuditLogStoreOptions>();
            services.AddSingleton<AuditLogService>(); // Registrar el servicio

            return services;
        }

        public static IServiceCollection AddDefaultDatabase(this IServiceCollection services)
        {
            services.Configure<DatabaseConnectionOptions>(options =>
            {
                // Configuración adicional opcional
            });

            services.AddSingleton<IConfigureOptions<DatabaseConnectionOptions>, ConfigureDatabaseConnectionOptions>();

            return services;
        }
    }
}
