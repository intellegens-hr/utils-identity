using IdentityUtils.Api.Models.Authentication;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Demos.IdentityServer4.SingleTenant.Configuration;
using IdentityUtils.Demos.IdentityServer4.SingleTenant.DbContext;
using IdentityUtils.Demos.IdentityServer4.SingleTenant.Models;
using IdentityUtils.IS4Extensions.IdentityServerBuilder;
using IdentityUtils.IS4Extensions.ServicesCollection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace IdentityUtils.Demos.IdentityServer4.SingleTenant
{
    public class Startup
    {
        //This is used only to seed data
        private IServiceCollection services;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity utils demo API");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            this.services = services;
            services
                .AddControllersWithViews()
                .RemoveIdentityUtilsAuthenticationControllerAssemblyPart();

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

            services
                .AddScoped<IConfigurationRoot>(x => Configuration)
                .AddSingleton<IIdentityUtilsAuthenticationConfig, IdentityUtilsConfiguration>()
                .AddScoped<DbConfig>()
                .AddDbContext<Is4DemoDbContext>()
                .AddAutoMapper(typeof(Is4ModelsMapperProfile));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity utils demo API", Version = "v1" });
            });

            services
                .AddIdentityUtilsIs4Extensions((builder) =>
                {
                    builder
                        .AddIdentity<IdentityManagerUser, IdentityManagerRole, Is4DemoDbContext>()
                        .AddDataStore<IdentityManagerUser, IdentityManagerRole, Is4DemoDbContext>()
                        //This will add authentication to all API calls, first argument is authority - in this case
                        //it's the URL of this instance. Second parameter is Audience for JWT bearer token
                        .AddAuthentication(Configuration["Is4Host"], Configuration["Is4AuthManagementAudience"], JwtBearerOptionsCallback)
                        .AddDefaultUserStore<IdentityManagerUser, UserDto, IdentityManagerRole>()
                        .AddDefaultRolesStore<IdentityManagerUser, IdentityManagerRole, RoleDto>()
                        .AddIntellegensProfileClaimsService<IdentityManagerUser, UserDto, IdentityManagerRole>();
                });

            services.AddAuthorization(opt =>
            {
                //Management controllers have defined Authorize attribute
                //This is policy definition they use. Every client calling APIs, needs to have
                //demo-is4-management-api resource as one of allowed scopes
                opt.AddPolicy("Is4ManagementApi", builder =>
                {
                    builder.RequireScope(Configuration["Is4AuthManagementAudience"], "jsapp");
                });
                opt.AddPolicy("AuthenticatedUser", builder => builder.RequireAuthenticatedUser());
            });

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
                    //other one will be used to authorize client apps calls to API apps. Client "jsapp" has no defined
                    //redirect urls and is used for login via AJAX calls
                    .AddDefaultClientConfiguration()
                    //Profile service will properly load roles data per tenant to tokens provided by IS4
                    .AddIdentityAndProfileService<IdentityManagerUser, UserDto, IdentityManagerRole>()
                    ;
            })
            .AddOperationalStore((options) =>
            {
                var dbConfiguration = new DbConfig(Configuration);
                options.ConfigureDbContext = builder =>
                    builder.UseSqlite($@"Data Source={dbConfiguration.DatabaseName};");

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
                options.TokenCleanupInterval = 30;
            });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }

        protected virtual void JwtBearerOptionsCallback(JwtBearerOptions options)
        {
        }
    }
}