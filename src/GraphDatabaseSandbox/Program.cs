using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureServices((services) =>
                {
                    var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Microsoft.Extensions.Hosting.Environments.Development;
                    var configurationBuilder = new ConfigurationBuilder()
                        .AddJsonFile(@"appsettings.json", false);

                    if (isDevelopment)
                        configurationBuilder.AddUserSecrets<Program>();

                    var configuration = configurationBuilder.Build();

                    services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                        .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"));
                    // Add all controllers
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