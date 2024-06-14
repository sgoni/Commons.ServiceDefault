using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Commons.ServiceDefaults
{
    public class ApiKeyAndCredentialsFilter : IAuthorizationFilter
    {
        private const string APIKEYNAME = "X-Custom-Header";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiKeyAndCredentialsFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult("Api Key was not provided. (Using ApiKeyMiddleware)");
                return;
            }

            var appSettings = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var apiKey = appSettings.GetValue<string>(APIKEYNAME);

            if (!apiKey.Equals(extractedApiKey))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult("Unauthorized client. (Using ApiKeyMiddleware)");
                return;
            }
        }
    }
}
