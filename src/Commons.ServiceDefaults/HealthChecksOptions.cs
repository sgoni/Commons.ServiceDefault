using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Commons.ServiceDefaults
{
    public class HealthChecksOptions
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);
        public string Path { get; set; }
        public Func<HttpContext, HealthReport, Task> ResponseWriter { get; set; }
        public Func<HealthCheckRegistration, bool> Predicate { get; set; }
        public bool AllowCachingResponses { get; set; } = false;
    }
}
