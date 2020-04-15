using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IdentityUtils.Demos.Client
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app)
        {
            WriteJsConfigurationFile();

            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

        private void WriteJsConfigurationFile()
        {
            StringBuilder tsContent = new StringBuilder();
            StringBuilder jsContent = new StringBuilder();

            string is4Host = Configuration["Is4Host"];
            string apiHost = Configuration["ApiHost"];

            Dictionary<string, string> configParams = new Dictionary<string, string>
            {
                { "AuthorizationAuthority", is4Host},
                { "AuthorizationClientId", Configuration["AuthorizationClientId"]},
                { "AuthorizationClientScope", Configuration["AuthorizationClientScope"]},

                { "Is4AuthorizationEndpoint", $"{is4Host}{Configuration["AuthorizationRoute"]}"},
                { "Is4TokenEndpoint", $"{is4Host}{Configuration["Is4TokenRoute"]}"},
                { "Is4EndSessionEndpoint", $"{is4Host}{Configuration["Is4EndSessionRoute"]}"},
                { "Is4JwksEndpoint", $"{is4Host}{Configuration["Is4JwksRoute"]}"},

                { "ApiLoginEndpoint", $"{apiHost}{Configuration["ApiLoginRoute"]}" },
                { "ApiLogoutEndpoint", $"{apiHost}{Configuration["ApiLogoutRoute"]}" },
                { "ApiUserProfileEndpoint", $"{apiHost}{Configuration["ApiUserProfileRoute"]}" }
            };

            foreach (var configParam in configParams)
            {
                jsContent.AppendLine($"const Config{configParam.Key} = \"{configParam.Value}\";");
                tsContent.AppendLine($"declare const Config{configParam.Key}: string;");
            }

            File.WriteAllText("wwwroot/Configuration/ApiConfiguration.js", jsContent.ToString());
            File.WriteAllText("wwwroot/Configuration/ApiConfigurationDeclarations.ts", tsContent.ToString());
        }
    }
}