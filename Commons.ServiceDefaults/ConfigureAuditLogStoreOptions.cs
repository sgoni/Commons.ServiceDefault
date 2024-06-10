using Commons.ServiceDefaults.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Commons.ServiceDefaults
{
    internal sealed class ConfigureAuditLogStoreOptions : IConfigureOptions<AuditLogStoreOptions>
    {
        private readonly IConfiguration _configuration;

        public ConfigureAuditLogStoreOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(AuditLogStoreOptions options)
        {
            // Configurar las opciones a partir de la configuración
            _configuration.GetSection(AuditLogStoreOptions.Option).Bind(options);
        }
    }
}
