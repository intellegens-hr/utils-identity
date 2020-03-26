using IdentityUtils.Demos.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityUtils.Demos.Api.ControllersApi
{
    [Route("/api/profile")]
    [Authorize]
    public class UserProfileControllerApi : ControllerBase
    {
        private readonly ApiUser apiUser;

        public UserProfileControllerApi(ApiUser apiUser)
        {
            this.apiUser = apiUser;
        }

        [HttpGet("/api/userprofile")]
        public async Task<UserProfile> Status()
        {
            var profile = new UserProfile
            {
                IsAuthenticated = apiUser.IsAuthenticated
            };

            if (profile.IsAuthenticated)
            {
                profile.UserProfileData = apiUser;
            };

            return profile;
        }
    }
}