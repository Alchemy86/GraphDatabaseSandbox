using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GraphDatabaseSandbox
{
    public class HostedService : IHostedService
    {
        private readonly ILogger<HostedService> _logger;
        public HostedService(ILogger<HostedService> logger)
        {
            _logger = logger;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, "Started the console app");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}