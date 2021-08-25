using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace GraphDatabaseSandbox
{
    public class ExampleMiddleware
    {
        private readonly RequestDelegate _next;

        public ExampleMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Test with https://localhost:5001/***/?option=Hello
        public async Task Invoke(HttpContext httpContext)
        {
            var option = httpContext.Request.Query["option"];

            if (!string.IsNullOrWhiteSpace(option))
            {
                httpContext.Items["option"] = WebUtility.HtmlEncode(option);
            }

            await _next(httpContext);
        }
    }
}
