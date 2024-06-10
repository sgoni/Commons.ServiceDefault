using Commons.ServiceDefaults.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Commons.ServiceDefaults
{
    internal sealed class ConfigureDatabaseConnectionOptions : IConfigureOptions<DatabaseConnectionOptions>
    {
        private readonly IConfiguration _configuration;

        public ConfigureDatabaseConnectionOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(DatabaseConnectionOptions options)
        {
            // Configurar las opciones a partir de la configuración
            _configuration.GetSection("DatabaseConnection").Bind(options);
        }
    }
}
