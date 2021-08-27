using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GraphDatabaseSandbox
{
    public class ExampleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExampleMiddleware> _logger;

        public ExampleMiddleware(RequestDelegate next, ILogger<ExampleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Test with https://localhost:5001/***/?option=Hello
        public async Task Invoke(HttpContext httpContext)
        {
            _logger.LogDebug("******* Middleware hit debug *******");
            _logger.LogInformation("******* Middleware hit info *******");
            var option = httpContext.Request.Query["option"];
            var moo = httpContext.User.Identity.Name;
            _logger.LogInformation("User is Auth?: {0}", moo);

            if (!string.IsNullOrWhiteSpace(option))
            {
                httpContext.Items["option"] = WebUtility.HtmlEncode(option);
            }

            await _next(httpContext);
        }
    }
}
