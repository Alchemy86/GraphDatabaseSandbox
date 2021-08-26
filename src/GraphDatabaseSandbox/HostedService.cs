using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GraphDatabaseSandbox
{
    public class HostedService : IHostedService
    {
        private readonly ILogger<HostedService> _logger;
        private readonly CosmosClient _cosmosClient;
        public HostedService(ILogger<HostedService> logger, CosmosClient cosmosClient)
        {
            _logger = logger;
            _cosmosClient = cosmosClient;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Log(LogLevel.Information, "Started the console app");
            var response = await _cosmosClient.CreateDatabaseIfNotExistsAsync("MyCosmosDb", cancellationToken: cancellationToken);

            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var mo = _cosmosClient.GetDatabase("");
                _logger.Log(LogLevel.Information, "Database Created***");
                var database = (Database)response;
                var resp = await database.CreateContainerIfNotExistsAsync("items", "/LastName", cancellationToken: cancellationToken);
                _logger.LogInformation("Container ID: {0}\n", resp.Container.Id);

            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}