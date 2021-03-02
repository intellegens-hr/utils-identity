using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/roles")]
    public class RoleManagementControllerApi
        : RolesControllerApiAbstract<RoleDto>
    {
        public RoleManagementControllerApi(IIdentityManagerRolesService<RoleDto> rolesService)
            : base(rolesService)
        {
        }
    }
}