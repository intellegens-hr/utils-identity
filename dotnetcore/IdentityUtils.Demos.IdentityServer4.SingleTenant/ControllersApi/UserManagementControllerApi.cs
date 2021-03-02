using AutoMapper;
using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Demos.IdentityServer4.SingleTenant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.SingleTenant.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/users")]
    public class UserManagementControllerApi
        : UsersControllerApiAbstract<IdentityManagerUser, UserDto>
    {
        public UserManagementControllerApi(
            IIdentityManagerUserService<IdentityManagerUser, UserDto> userManager,
            IMapper mapper)
        : base(userManager, mapper)
        {
        }
    }
}