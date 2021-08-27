
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GraphDatabaseSandbox
{

    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<CustomMiddleware> _logger;

        public CustomMiddleware(RequestDelegate next, ILogger<CustomMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // IMyScopedService is injected into Invoke
        public async Task Invoke(HttpContext httpContext)
        {
 
            _logger.LogInformation("WE ARE HERE &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&& {0}", httpContext.User.Identity.IsAuthenticated);
            await _next(httpContext);
        }
    }
}