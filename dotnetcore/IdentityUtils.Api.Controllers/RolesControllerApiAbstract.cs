using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Controllers
{
    /// <summary>
    /// Basic controller which provides all needed API endpoints for role management.
    /// </summary>
    /// <typeparam name="TRoleDto">Role DTO model</typeparam>
    [ApiController]
    public abstract class RolesControllerApiAbstract<TRoleDto> : ControllerBase
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        private readonly IIdentityManagerRolesService<TRoleDto> rolesService;

        protected RolesControllerApiAbstract(IIdentityManagerRolesService<TRoleDto> rolesService)
        {
            this.rolesService = rolesService;
        }

        [HttpPost]
        public virtual async Task<IdentityUtilsResult<TRoleDto>> AddRole(TRoleDto roleDto)
        {
            return await rolesService.AddRole(roleDto);
        }

        [HttpDelete("{roleId}")]
        public virtual async Task<IdentityUtilsResult> DeleteRole([FromRoute] Guid roleId)
            => await rolesService.DeleteRole(roleId);

        [HttpGet]
        public virtual async Task<IEnumerable<TRoleDto>> GetAllRoles()
            => await rolesService.GetAllRoles();

        [HttpGet("{roleId}")]
        public virtual async Task<IdentityUtilsResult<TRoleDto>> GetRoleById([FromRoute] Guid roleId)
            => await rolesService.GetRole(roleId);

        [HttpPost("search")]
        public virtual async Task<IdentityUtilsResult<IEnumerable<TRoleDto>>> Search([FromBody] RoleSearch searchModel)
            => IdentityUtilsResult<IEnumerable<TRoleDto>>.SuccessResult(await rolesService.Search(searchModel));
    }
}