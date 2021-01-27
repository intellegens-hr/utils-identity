using AutoMapper;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Controllers
{
    /// <summary>
    /// Basic controller which provides all needed API endpoints for user management.
    /// </summary>
    /// <typeparam name="TUser">User database model</typeparam>
    /// <typeparam name="TRole">Role database model</typeparam>
    /// <typeparam name="TUserDto">User DTO model</typeparam>
    [ApiController]
    public abstract class UsersTenantControllerApiAbstract<TUser, TUserDto> : UsersControllerApiBase<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IIdentityManagerUserTenantService<TUser, TUserDto> userManager;

        public UsersTenantControllerApiAbstract(
            IIdentityManagerUserTenantService<TUser, TUserDto> userManager,
            IMapper mapper) : base(userManager, mapper)
        {
            this.userManager = userManager;
        }

        [HttpPost("{userId}/roles/{tenantId}/{roleId}")]
        public virtual async Task<IdentityUtilsResult> AddUserToRole([FromRoute] Guid userId, [FromRoute] Guid tenantId, [FromRoute] Guid roleId)
            => await userManager.AddToRoleAsync(userId, tenantId, roleId);

        [HttpGet("{userId}/roles/{tenantId}")]
        public virtual async Task<IdentityUtilsResult<RoleBasicData>> GetUserRoles([FromRoute] Guid userId, [FromRoute] Guid tenantId)
        {
            var userRoles = await userManager.GetRolesAsync(userId, tenantId);
            return IdentityUtilsResult<RoleBasicData>.SuccessResult(userRoles);
        }

        [HttpDelete("{userId}/roles/{tenantId}/{roleId}")]
        public virtual async Task<IdentityUtilsResult> RemoveUserFromRole([FromRoute] Guid userId, [FromRoute] Guid tenantId, [FromRoute] Guid roleId)
            => await userManager.RemoveFromRoleAsync(userId, tenantId, roleId);

        [HttpPost("search")]
        public virtual async Task<IEnumerable<TUserDto>> Search([FromBody] UsersTenantSearch search)
            => await userManager.Search(search);
    }
}