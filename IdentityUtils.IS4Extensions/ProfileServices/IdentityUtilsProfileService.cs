using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.IS4Extensions.ProfileServices
{
    public class IdentityUtilsProfileService<TUser, TUserDto, TRole> : IProfileService
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
        where TRole : IdentityManagerRole
    {
        private readonly IUserClaimsPrincipalFactory<TUser> claimsFactory;
        private readonly IdentityManagerUserService<TUser, TUserDto, TRole> tenantUserService;

        public IdentityUtilsProfileService(
            IdentityManagerUserService<TUser, TUserDto, TRole> tenantUserService,
            IUserClaimsPrincipalFactory<TUser> claimsFactory)
        {
            this.tenantUserService = tenantUserService;
            this.claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = Guid.Parse(context.Subject.GetSubjectId());

            var userResult = await tenantUserService.FindByIdAsync(userId);
            var user = userResult.Payload;
            var principal = await claimsFactory.CreateAsync(user);

            var claims = principal.Claims
                .Where(x => context.RequestedClaimTypes.Contains(x.Type))
                .ToList();

            var tenantClaims = await tenantUserService.GetUserTenantRolesClaims(userId);

            context.IssuedClaims = claims.Union(tenantClaims).ToList();

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