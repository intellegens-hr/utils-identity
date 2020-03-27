using IdentityUtils.Api.Extensions;
using IdentityUtils.Api.Extensions.RestClients;
using IdentityUtils.Demos.Api.Configuration;
using IdentityUtils.Demos.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityUtils.Demos.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private AppSettings AppSettings { get; set; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            AppSettings = new AppSettings(Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var apiExtensionsConfig = new ApiExtensionsConfig(AppSettings);

            services.AddMvcCore()
                .AddAuthorization();

            services.AddSingleton(AppSettings);

            services.AddHttpContextAccessor();
            services.AddScoped<ApiUser>();

            services.AddScoped<TenantManagementApi<TenantDto>>((collection) =>
            {
                return new TenantManagementApi<TenantDto>(new Is4ManagementRestClient(apiExtensionsConfig), apiExtensionsConfig);
            });

            services.AddHealthChecks();

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin()
                          .Build();
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = AppSettings.Is4Host;
                    options.Audience = AppSettings.ApiAuthenticationAudience;
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("default");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}