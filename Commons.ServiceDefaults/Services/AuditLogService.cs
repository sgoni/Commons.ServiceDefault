using Commons.ServiceDefaults.Options;
using Microsoft.Extensions.Options;

namespace Commons.ServiceDefaults.Services
{
    public class AuditLogService
    {
        private readonly AuditLogStoreOptions _options;

        public AuditLogService(IOptions<AuditLogStoreOptions> options)
        {
            _options = options.Value;
        }
    }
}
