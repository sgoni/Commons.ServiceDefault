using Commons.ServiceDefaults.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Commons.ServiceDefaults.Middleware
{
    public class DatabaseConnectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DatabaseConnectionOptions _options;

        public DatabaseConnectionMiddleware(RequestDelegate next, IOptions<DatabaseConnectionOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Usar _options.ConnectionString y _options.Timeout
            // para configurar tu conexión a la base de datos

            // Ejemplo: Conectar a la base de datos
            // using var connection = new SqlConnection(_options.ConnectionString);
            // connection.Open();

            // Continuar con el siguiente middleware en la tubería
            await _next(context);
        }
    }
}
