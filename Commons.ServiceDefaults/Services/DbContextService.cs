using Commons.ServiceDefaults.Options;
using Microsoft.Extensions.Options;

namespace Commons.ServiceDefaults.Services
{
    public class DbContextService
    {
        private readonly DatabaseConnectionOptions _options;

        public DbContextService(IOptions<DatabaseConnectionOptions> options)
        {
            _options = options.Value;
        }
    }
}
