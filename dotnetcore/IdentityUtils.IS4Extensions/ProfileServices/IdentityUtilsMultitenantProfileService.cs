using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Configuration;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.IS4Extensions.ProfileServices
{
    public class IdentityUtilsMultitenantProfileService<TUser, TUserDto, TTenantDto> : IProfileService
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
        where TTenantDto : class, IIdentityManagerTenantDto
    {
        private readonly IUserClaimsPrincipalFactory<TUser> claimsFactory;
        private readonly IIdentityUtilsMultitenantConfiguration configuration;
        private readonly IIdentityManagerTenantService<TTenantDto> tenantService;
        private readonly IIdentityManagerUserTenantService<TUser, TUserDto> tenantUserService;

        public IdentityUtilsMultitenantProfileService(
            IIdentityManagerUserTenantService<TUser, TUserDto> tenantUserService,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IIdentityManagerTenantService<TTenantDto> tenantService,
            IIdentityUtilsMultitenantConfiguration configuration)
        {
            this.tenantUserService = tenantUserService;
            this.claimsFactory = claimsFactory;
            this.tenantService = tenantService;
            this.configuration = configuration;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = Guid.Parse(context.Subject.GetSubjectId());

            var (userResult, user) = await tenantUserService.FindByIdAsync(userId).UnpackSingleOrDefault();
            var principal = await claimsFactory.CreateAsync(user);

            var claims = principal.Claims
                .Where(x => context.RequestedClaimTypes.Contains(x.Type));

            var tenants = await tenantService.Search(new TenantSearch { Hostname = configuration.Hostname });
            if (!tenants.Any())
            {
                throw new Exception("Host not recognized!");
            }

            var tenantId = tenants.First().TenantId;
            var roles = await tenantUserService.GetRolesAsync(userId, tenantId);
            var tenantRoles = roles.Select(x => new Claim(ClaimTypes.Role, x.NormalizedName));

            context.IssuedClaims = claims.Union(tenantRoles).ToList();
            context.IssuedClaims.Add(new Claim("userId", userId.ToString()));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = Guid.Parse(context.Subject.GetSubjectId());
            var user = await tenantUserService.FindByIdAsync(sub);
            context.IsActive = user.Success;
        }
    }
}