using IdentityServer4.Services;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.IS4Extensions.ProfileServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityUtils.IS4Extensions.ServicesCollection
{
    public class IdentityUtilsServicesBuilderMultitenant
    {
        private readonly IServiceCollection services;

        public IdentityUtilsServicesBuilderMultitenant(IServiceCollection services)
        {
            this.services = services;
            LoadDefaults(services);
        }

        public IdentityUtilsServicesBuilderMultitenant AddAuthentication(string authority, string managementApiAudience)
        {
            services.AddAuthentication(authority, managementApiAudience);
            return this;
        }

        public IdentityUtilsServicesBuilderMultitenant AddDataStore<TUser, TRole, TTenant, TDbContext>()
            where TUser : IdentityManagerUser
            where TRole : IdentityManagerRole
            where TTenant : IdentityManagerTenant
            where TDbContext : IdentityManagerTenantDbContext<TUser, TRole, TTenant>
        {
            services.AddScoped<IIdentityManagerTenantContext<TTenant>, TDbContext>();
            services.AddScoped<IIdentityManagerUserContext<TUser>, TDbContext>();
            services.AddScoped<IdentityManagerTenantDbContext<TUser, TRole, TTenant>, TDbContext>();

            return this;
        }

        public IdentityUtilsServicesBuilderMultitenant AddIdentity<TUser, TRole, TTenant, TDbContext>()
            where TUser : IdentityManagerUser
            where TRole : IdentityManagerRole
            where TTenant : IdentityManagerTenant
            where TDbContext : IdentityManagerTenantDbContext<TUser, TRole, TTenant>
        {
            services.AddIdentity<TUser, TRole>()
             .AddEntityFrameworkStores<TDbContext>()
             .AddDefaultTokenProviders();

            return this;
        }

        public IdentityUtilsServicesBuilderMultitenant AddIntellegensProfileClaimsService<TUser, TUserDto, TRole>()
            where TUser : IdentityManagerUser
            where TUserDto : class, IIdentityManagerUserDto
            where TRole : IdentityManagerRole
        {
            services.AddTransient<IProfileService, IdentityUtilsMultitenantProfileService<TUser, TUserDto>>();
            return this;
        }

        public IdentityUtilsServicesBuilderMultitenant AddDefaultRolesStore<TRole, TRoleDto>()
            where TRole : IdentityManagerRole
            where TRoleDto : class, IIdentityManagerRoleDto
        {
            services.AddScoped<IIdentityManagerRolesService<TRoleDto>, IdentityManagerRolesService<TRole, TRoleDto>>();
            return this;
        }

        public IdentityUtilsServicesBuilderMultitenant AddDefaultTenantStore<TTenant, TTenantDto>()
            where TTenant : IdentityManagerTenant
            where TTenantDto : class, IIdentityManagerTenantDto
        {
            services.AddScoped<IIdentityManagerTenantService<TTenantDto>, IdentityManagerTenantService<TTenant, TTenantDto>>();

            return this;
        }

        public IdentityUtilsServicesBuilderMultitenant AddDefaultTenantUserStore<TUser, TUserDto, TRole>()
            where TUser : IdentityManagerUser
            where TUserDto : class, IIdentityManagerUserDto
            where TRole : IdentityManagerRole
        {
            services.AddScoped<IIdentityManagerUserTenantService<TUser, TUserDto>, IdentityManagerUserTenantService<TUser, TUserDto, TRole>>();
            return this;
        }

        private IServiceCollection LoadDefaults(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            return services;
        }
    }
}