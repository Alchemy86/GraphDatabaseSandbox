using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Serilog;

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
                //.UseEnvironment(Microsoft.Extensions.Hosting.Environments.Development)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(true);
                    webBuilder.UseUrls("https://localhost:44321/"); 
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

                        app.UseAuthentication();
                        app.UseAuthorization();

                        app.UseMiddleware<CustomMiddleware>();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                            name: "default",
                            pattern: "{controller=Home}/{action=Index}/{id?}");
                            endpoints.MapControllers();
                        });
                    });
                })
                .ConfigureHostConfiguration((configHost) => {
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configHost) => {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile(@"appsettings.json", false);

                    if(hostContext.HostingEnvironment.IsDevelopment()) {
                        configHost.AddUserSecrets<Program>();
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                        .AddMicrosoftIdentityWebApp(hostContext.Configuration.GetSection("AzureAd"));
                        
                    services.AddControllers(options =>
                    {
                        options.RespectBrowserAcceptHeader = true; // false by default
                    })
                    //support application/xml - Erroring
                    //.AddXmlSerializerFormatters()
                    //.AddXmlDataContractSerializerFormatters()
                    .AddJsonOptions(options => 
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

                    // Running Basic hosted services
                    services.AddHostedService<HostedService>();
                    services.AddHostedService<LifetimeEventsHostedService>();

                    // Configure a cosmodDB connection
                    services.AddSingleton(s => {
                        var costmosDB = hostContext.Configuration.GetSection("CosmosDB");
                        if (string.IsNullOrEmpty(costmosDB["PrimaryKey"]))
                        {
                            throw new InvalidOperationException(
                                "Please specify a valid CosmosDBConnection in the appSettings.json. The PrimaryKey is required");
                        }

                        return new CosmosClientBuilder(costmosDB["EndpointUri"], costmosDB["PrimaryKey"]).Build();
                    });

                    // Register a request filter at startup
                    services.AddTransient<IStartupFilter, ExampleStartupFilter>();

                }).UseSerilog((hostingContext, loggerConfiguration) => 
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration);
                });
    }
}
