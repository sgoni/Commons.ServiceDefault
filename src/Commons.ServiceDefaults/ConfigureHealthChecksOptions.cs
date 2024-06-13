using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Commons.ServiceDefaults
{
    internal sealed class ConfigureHealthChecksOptions : IConfigureOptions<HealthChecksOptions>
    {
        private readonly IConfiguration _configuration;

        public ConfigureHealthChecksOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(HealthChecksOptions options)
        {
            _configuration.GetSection("HealthChecks").Bind(options);

            // Configuración adicional opcional si se requiere
            options.ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    results = report.Entries.Select(e => new { key = e.Key, value = e.Value.Status.ToString() })
                });
                await context.Response.WriteAsync(result);
            };

            options.Predicate = check => true; // Ejecutar todos los Health Checks por defecto
        }
    }
}
