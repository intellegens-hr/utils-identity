using AutoMapper;
using IdentityUtils.Commons.Mailing;
using IdentityUtils.Commons.Mailing.Google;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Demos.IdentityServer4.Configuration;
using IdentityUtils.Demos.IdentityServer4.DbContext;
using IdentityUtils.Demos.IdentityServer4.Models;
using IdentityUtils.IS4Extensions.IdentityServerBuilder;
using IdentityUtils.IS4Extensions.ServicesCollection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityUtils.Demos.IdentityServer4
{
    public class Startup
    {
        //This is used only to seed data
        private IServiceCollection services;

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
            this.services = services;
            services.AddControllersWithViews();

            // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            // configures IIS in-proc settings
            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            services.AddDbContext<Is4DemoDbContext>();
            services.AddAutoMapper(typeof(Is4ModelsMapperProfile));

            services
                .AddIdentityUtilsIs4Extensions((builder) =>
                {
                    builder
                        .AddIdentity<IdentityManagerUser, IdentityManagerRole, IdentityManagerTenant, Is4DemoDbContext>()
                        //This will add authentication to all API calls, first argument is authority - in this case
                        //it's the URL of this instance. Second parameter is Audience for JWT bearer token
                        .AddAuthentication(Configuration["Is4Host"], Configuration["Is4AuthManagementAudience"])
                        .AddTenantStore<IdentityManagerTenant, TenantDto>()
                        .AddTenantUserStore<IdentityManagerUser, UserDto, IdentityManagerRole>()
                        .AddRolesStore<IdentityManagerUser, IdentityManagerRole, RoleDto>()
                        .AddIntellegensProfileClaimsService<IdentityManagerUser, UserDto, IdentityManagerRole>();
                });

            services.AddAuthorization(opt =>
            {
                //Management controllers have defined Authorize attribute
                //This is policy definition they use. Every client calling APIs, needs to have
                //demo-is4-management-api resource as one of allowed scopes
                opt.AddPolicy("Is4ManagementApi", builder =>
                {
                    builder.RequireScope(Configuration["Is4AuthManagementAudience"]);
                });
            });

            //User management requires Mailing provider to send e-mails for password reset
            services.AddSingleton<IGoogleMailingProviderConfig, MasterConfig>();
            services.AddSingleton<IMailingProvider, GoogleMailingProvider>();

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

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .LoadIdentityUtilsIdentityServerSettings((builder) =>
                {
                    builder
                        //This will add default clients: is4management and jsapp. One will be used to authorize calls to IS4,
                        //other one will be used to authorize client apps calls to API apps
                        .AddDefaultClientConfiguration()
                        //Profile service will properly load roles data per tenant to tokens provided by IS4
                        .AddIdentityAndProfileService<IdentityManagerUser, UserDto, IdentityManagerRole>();
                });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseAuthentication();
            SeedData.EnsureSeedData(services).Wait();

            app.UseCors("default");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseIdentityServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}