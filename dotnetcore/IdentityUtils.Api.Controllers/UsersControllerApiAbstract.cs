using AutoMapper;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public abstract class UsersControllerApiAbstract<TUser, TUserDto> : UsersControllerApiBase<TUser, TUserDto>
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IIdentityManagerUserService<TUser, TUserDto> userManager;

        public UsersControllerApiAbstract(
            IIdentityManagerUserService<TUser, TUserDto> userManager,
            IMapper mapper) : base(userManager, mapper)
        {
            this.userManager = userManager;
        }

        [HttpPost("{userId}/roles/{roleId}")]
        public virtual async Task<IdentityUtilsResult> AddUserToRole([FromRoute] Guid userId, [FromRoute] Guid roleId)
            => await userManager.AddToRoleAsync(userId, roleId);

        [HttpGet("{userId}/roles")]
        public virtual async Task<IdentityUtilsResult<RoleBasicData>> GetUserRoles([FromRoute] Guid userId)
        {
            var userRoles = await userManager.GetRolesAsync(userId);
            return IdentityUtilsResult<RoleBasicData>.SuccessResult(userRoles);
        }

        [HttpDelete("{userId}/roles/{roleId}")]
        public virtual async Task<IdentityUtilsResult> RemoveUserFromRole([FromRoute] Guid userId, [FromRoute] Guid roleId)
            => await userManager.RemoveFromRoleAsync(userId, roleId);

        [HttpPost("search")]
        public virtual async Task<IdentityUtilsResult<TUserDto>> Search([FromBody] UsersSearch search)
            => IdentityUtilsResult<TUserDto>.SuccessResult(await userManager.Search(search));
    }
}