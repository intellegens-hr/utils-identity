using IdentityUtils.Core.Contracts;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Controllers
{
    /// <summary>
    /// Basic controller which provides all needed API endpoints for role management.
    /// </summary>
    /// <typeparam name="TUser">User database model</typeparam>
    /// <typeparam name="TUserDto">User DTO model</typeparam>
    /// <typeparam name="TRole">Role database model</typeparam>
    /// <typeparam name="TRoleDto">Role DTO model</typeparam>
    [ApiController]
    public abstract class RolesControllerApiAbstract<TUser, TUserDto, TRole, TRoleDto> : ControllerBase
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
        where TRole : IdentityManagerRole
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        private readonly IdentityManagerRolesService<TUser, TRole, TRoleDto> rolesService;

        protected RolesControllerApiAbstract(IdentityManagerRolesService<TUser, TRole, TRoleDto> rolesService)
        {
            this.rolesService = rolesService;
        }

        [HttpGet]
        public async Task<IList<TRoleDto>> GetAllRoles()
            => await rolesService.GetAllRoles();

        [HttpPost]
        public async Task<IdentityManagementResult<TRoleDto>> AddRole(TRoleDto roleDto)
        {
            var roleResult = await rolesService.AddRole(roleDto);
            return roleResult.ToTypedResult<TRoleDto>(roleDto);
        }

        [HttpGet("{roleId}")]
        public async Task<IdentityManagementResult<TRoleDto>> GetRoleById([FromRoute]Guid roleId)
            => await rolesService.GetRole(roleId);

        [HttpDelete("{roleId}")]
        public async Task<IdentityManagementResult> DeleteRole([FromRoute]Guid roleId)
            => await rolesService.DeleteRole(roleId);

        [HttpGet("rolename/{roleName}")]
        public async Task<IdentityManagementResult<TRoleDto>> GetRoleByNormalizedName([FromRoute]string roleName)
        {
            roleName = WebUtility.UrlDecode(roleName);
            return await rolesService.GetRole(roleName);
        }
    }
}