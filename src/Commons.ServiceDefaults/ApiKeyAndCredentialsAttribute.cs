using Microsoft.AspNetCore.Mvc;

namespace Commons.ServiceDefaults
{
    public class ApiKeyAndCredentialsAttribute : TypeFilterAttribute
    {
        public ApiKeyAndCredentialsAttribute() : base(typeof(ApiKeyAndCredentialsFilter)) { }
    }
}
