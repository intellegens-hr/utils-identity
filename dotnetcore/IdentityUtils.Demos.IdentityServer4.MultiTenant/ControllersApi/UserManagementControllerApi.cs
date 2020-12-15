using AutoMapper;
using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/users")]
    public class UserManagementControllerApi
        : UsersTenantControllerApiAbstract<IdentityManagerUser, UserDto>
    {
        public UserManagementControllerApi(
            IIdentityManagerUserTenantService<IdentityManagerUser, UserDto> userManager,
            IMapper mapper)
        : base(userManager, mapper)
        {
        }
    }
}