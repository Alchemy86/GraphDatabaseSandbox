using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

[assembly: UserSecretsId("aspnet-TestApp-4bdb4f05-4dce-4136-9db4-ee487faf451d")]
namespace GraphDatabaseSandbox
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(true);
                    webBuilder.Configure(app => {
                        // Configure services without a startup
                        var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                        if (env.IsDevelopment())
                        {
                            app.UseDeveloperExceptionPage();
                        }

                        app.UseStaticFiles();
                        app.UseRouting();
                        app.UseCors();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                })
                .ConfigureHostConfiguration((configHost) => {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureServices((services) =>
                {
                    // Add all controllers
                    services.AddControllers(options =>
                    {
                        options.RespectBrowserAcceptHeader = true; // false by default
                    })
                    //support application/xml
                    .AddXmlSerializerFormatters()
                    .AddXmlDataContractSerializerFormatters()
                    .AddJsonOptions(options => 
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

                    // Running Basic hosted services
                    services.AddHostedService<HostedService>();
                    services.AddHostedService<LifetimeEventsHostedService>();

                    // Register a request filter at startup
                    services.AddTransient<IStartupFilter, ExampleStartupFilter>();

                }).ConfigureLogging((hostBuilder, configureLogging) =>
                {
                    var configuration = new ConfigurationBuilder()
                        .AddJsonFile(@"Config/Serilog.json", true);

                    if (hostBuilder.HostingEnvironment.IsDevelopment())
                        configuration.AddUserSecrets<Program>();

                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(configuration.Build())
                        .CreateLogger();

                    configureLogging.AddSerilog();
                });
    }
}