using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/roles")]
    public class RoleManagementControllerApi
        : RolesControllerApiAbstract<IdentityManagerUser, UserDto, IdentityManagerRole, RoleDto>
    {
        public RoleManagementControllerApi(IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto> rolesService)
            : base(rolesService)
        {
        }
    }
}