using IdentityServer4.Services;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.IS4Extensions.ProfileServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;

namespace IdentityUtils.IS4Extensions.ServicesCollection
{
    public class IdentityUtilsServicesBuilder
    {
        private readonly IServiceCollection services;

        public IdentityUtilsServicesBuilder(IServiceCollection services)
        {
            this.services = services;
            LoadDefaults(services);
        }

        public IdentityUtilsServicesBuilder AddIdentity<TUser, TRole, TTenant, TDbContext>()
            where TUser : IdentityManagerUser
            where TRole : IdentityManagerRole
            where TTenant : IdentityManagerTenant
            where TDbContext : IdentityManagerDbContext<TUser, TRole, TTenant>
        {
            services.AddIdentity<TUser, TRole>()
             .AddEntityFrameworkStores<TDbContext>()
             .AddDefaultTokenProviders();

            services.AddScoped<IIdentityManagerTenantContext<TTenant>, TDbContext>();
            services.AddScoped<IIdentityManagerUserContext<TUser>, TDbContext>();
            services.AddScoped<IdentityManagerDbContext<TUser, TRole, TTenant>, TDbContext>();

            return this;
        }

        public IdentityUtilsServicesBuilder AddTenantStore<TTenant, TTenantDto>()
            where TTenant : IdentityManagerTenant
            where TTenantDto : class, IIdentityManagerTenantDto
        {
            services.AddScoped<IdentityManagerTenantService<TTenant, TTenantDto>>();

            return this;
        }

        public IdentityUtilsServicesBuilder AddTenantUserStore<TUser, TUserDto, TRole>()
            where TUser : IdentityManagerUser
            where TUserDto : class, IIdentityManagerUserDto
            where TRole : IdentityManagerRole
        {
            services.AddScoped<IdentityManagerUserService<TUser, TUserDto, TRole>>();
            return this;
        }

        public IdentityUtilsServicesBuilder AddRolesStore<TUser, TRole, TRoleDto>()
            where TUser : IdentityManagerUser
            where TRole : IdentityManagerRole
            where TRoleDto : class, IIdentityManagerRoleDto
        {
            services.AddScoped<IdentityManagerRolesService<TUser, TRole, TRoleDto>>();
            return this;
        }

        public IdentityUtilsServicesBuilder AddAuthentication(string authority, string managementApiAudience)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = managementApiAudience;

                    options.RequireHttpsMetadata = false;
                });

            return this;
        }

        public IdentityUtilsServicesBuilder AddIntellegensProfileClaimsService<TUser, TUserDto, TRole>()
            where TUser : IdentityManagerUser
            where TUserDto : class, IIdentityManagerUserDto
            where TRole : IdentityManagerRole
        {
            services.AddTransient<IProfileService, IdentityUtilsProfileService<TUser, TUserDto, TRole>>();
            return this;
        }

        private IServiceCollection LoadDefaults(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services;
        }
    }

    public static class IdentityUtilsServicesSetup
    {
        public static IServiceCollection AddIdentityUtilsIs4Extensions(this IServiceCollection services, Action<IdentityUtilsServicesBuilder> builder)
        {
            var identityUtilsServicesBuilder = new IdentityUtilsServicesBuilder(services);
            builder(identityUtilsServicesBuilder);
            return services;
        }
    }
}