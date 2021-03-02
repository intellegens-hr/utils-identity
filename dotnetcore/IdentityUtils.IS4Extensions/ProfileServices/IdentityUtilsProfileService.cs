using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.IS4Extensions.ProfileServices
{
    public class IdentityUtilsProfileService<TUser, TUserDto> : IProfileService
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IUserClaimsPrincipalFactory<TUser> claimsFactory;
        private readonly IIdentityManagerUserService<TUser, TUserDto> userService;

        public IdentityUtilsProfileService(
            IIdentityManagerUserService<TUser, TUserDto> userService,
            IUserClaimsPrincipalFactory<TUser> claimsFactory)
        {
            this.userService = userService;
            this.claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = Guid.Parse(context.Subject.GetSubjectId());

            var (userResult, user) = await userService.FindByIdAsync(userId).UnpackSingleOrDefault();
            var principal = await claimsFactory.CreateAsync(user);

            var claims = principal.Claims
                .Where(x => context.RequestedClaimTypes.Contains(x.Type));

            context.IssuedClaims.Add(new Claim("userId", userId.ToString()));
            context.IssuedClaims.AddRange(claims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = Guid.Parse(context.Subject.GetSubjectId());
            var user = await userService.FindByIdAsync(sub);
            context.IsActive = user.Success;
        }
    }
}