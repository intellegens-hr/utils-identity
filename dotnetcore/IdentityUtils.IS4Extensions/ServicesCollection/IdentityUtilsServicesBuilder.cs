using IdentityServer4.Services;
using IdentityUtils.Api.Controllers.Authentication.Services;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.IS4Extensions.ProfileServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

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

        public IdentityUtilsServicesBuilder AddAuthentication(string authority, string managementApiAudience)
        {
            services.AddAuthentication(authority, managementApiAudience);
            return this;
        }

        public IdentityUtilsServicesBuilder AddDataStore<TUser, TRole, TDbContext>()
            where TUser : IdentityManagerUser
            where TRole : IdentityManagerRole
            where TDbContext : IdentityManagerDbContext<TUser, TRole>
        {
            services.AddScoped<IIdentityManagerUserContext<TUser>, TDbContext>();
            services.AddScoped<IdentityManagerDbContext<TUser, TRole>, TDbContext>();

            return this;
        }

        public IdentityUtilsServicesBuilder AddDefaultRolesStore<TUser, TRole, TRoleDto>()
                    where TUser : IdentityManagerUser
            where TRole : IdentityManagerRole
            where TRoleDto : class, IIdentityManagerRoleDto
        {
            services.AddScoped<IIdentityManagerRolesService<TRoleDto>, IdentityManagerRolesService<TRole, TRoleDto>>();
            return this;
        }

        public IdentityUtilsServicesBuilder AddDefaultUserStore<TUser, TUserDto, TRole>()
            where TUser : IdentityManagerUser
            where TUserDto : class, IIdentityManagerUserDto
            where TRole : IdentityManagerRole
        {
            services.AddScoped<IIdentityManagerUserService<TUser, TUserDto>, IdentityManagerUserService<TUser, TRole, TUserDto>>();
            return this;
        }

        public IdentityUtilsServicesBuilder AddIdentity<TUser, TRole, TDbContext>()
            where TUser : IdentityManagerUser
            where TRole : IdentityManagerRole
            where TDbContext : IdentityManagerDbContext<TUser, TRole>
        {
            services.AddIdentity<TUser, TRole>()
             .AddEntityFrameworkStores<TDbContext>()
             .AddDefaultTokenProviders();

            return this;
        }

        public IdentityUtilsServicesBuilder AddIntellegensProfileClaimsService<TUser, TUserDto, TRole>()
            where TUser : IdentityManagerUser
            where TUserDto : class, IIdentityManagerUserDto
            where TRole : IdentityManagerRole
        {
            services.AddTransient<IProfileService, IdentityUtilsProfileService<TUser, TUserDto>>();
            return this;
        }

        private IServiceCollection LoadDefaults(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IIdentityUtilsAuthService, IdentityUtilsAuthService>();
            return services;
        }
    }
}