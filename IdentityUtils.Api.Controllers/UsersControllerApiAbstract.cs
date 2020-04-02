using AutoMapper;
using IdentityUtils.Api.Models.Users;
using IdentityUtils.Commons.Mailing;
using IdentityUtils.Core.Contracts.Commons;
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
    /// Basic controller which provides all needed API endpoints for user management.
    /// </summary>
    /// <typeparam name="TUser">User database model</typeparam>
    /// <typeparam name="TRole">Role database model</typeparam>
    /// <typeparam name="TUserDto">User DTO model</typeparam>
    [ApiController]
    public abstract class UsersControllerApiAbstract<TUser, TRole, TUserDto> : ControllerBase
        where TUser : IdentityManagerUser
        where TRole : IdentityManagerRole
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IdentityManagerUserService<TUser, TUserDto, TRole> userManager;
        private readonly IMapper mapper;

        public UsersControllerApiAbstract(
            IdentityManagerUserService<TUser, TUserDto, TRole> userManager,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<List<TUserDto>> GetAllUsers()
            => mapper.Map<List<TUser>, List<TUserDto>>(await userManager.GetAllUsers());

        [HttpGet("{id}")]
        public virtual async Task<IdentityUtilsResult<TUserDto>> GetUserById([FromRoute]Guid id)
            => await userManager.FindByIdAsync<TUserDto>(id);

        [HttpDelete("{id}")]
        public virtual async Task<IdentityUtilsResult> DeleteUser([FromRoute]Guid id)
            => await userManager.DeleteUser(id);

        [HttpPost()]
        public virtual async Task<IdentityUtilsResult<TUserDto>> CreateUser([FromBody]TUserDto userDto)
        {
            return await userManager.CreateUser(userDto);
        }

        [HttpPost("{id}")]
        public virtual async Task<IdentityUtilsResult<TUserDto>> UpdateUser([FromRoute]Guid id, [FromBody]TUserDto userDto)
        {
            if (id != userDto.Id)
                throw new UnauthorizedAccessException();

            var result = await userManager.UpdateUser(userDto);
            return result.ToTypedResult(userDto);
        }

        [HttpGet("by/{username}")]
        public virtual async Task<IdentityUtilsResult<TUserDto>> GetUserByUsername([FromRoute]string username)
            => await userManager.FindByNameAsync<TUserDto>(WebUtility.UrlDecode(username));

        [HttpPost("passwordreset")]
        public virtual async Task<IdentityUtilsResult<PasswordForgottenResponse>> GetPasswordResetToken([FromBody]PasswordForgottenRequest passwordForgottenRequest)
        {
            var userResult = await userManager.FindByNameAsync(passwordForgottenRequest.Username);
            if (!userResult.Success)
                return userResult.ToTypedResult<PasswordForgottenResponse>();

            var resetTokenResult = await userManager.GeneratePasswordResetTokenAsync(passwordForgottenRequest.Username);
            if (!resetTokenResult.Success)
                return resetTokenResult.ToTypedResult<PasswordForgottenResponse>();

            var passwordResponse = new PasswordForgottenResponse
            {
                Username = userResult.Payload.UserName,
                Email = userResult.Payload.Email,
                Token = resetTokenResult.Payload
            };

            return IdentityUtilsResult<PasswordForgottenResponse>.SuccessResult(passwordResponse);
        }

        [HttpPost("passwordreset/newpassword")]
        public virtual async Task<IdentityUtilsResult> SetNewPasswordAfterReset([FromBody]PasswordForgottenNewPassword newPassword)
        {
            return await userManager.ResetPasswordAsync(newPassword.Username, newPassword.Token, newPassword.Password);
        }

        [HttpGet("roles/listusers/{tenantId}/{roleId}")]
        public virtual async Task<IdentityUtilsResult<List<TUserDto>>> GetRoleById([FromRoute]Guid tenantId, [FromRoute]Guid roleId)
            => await userManager.RoleUsersPerTenant(roleId, tenantId);

        [HttpGet("{userId}/roles/{tenantId}")]
        public virtual async Task<IdentityUtilsResult<IList<string>>> GetUserRoles([FromRoute]Guid userId, [FromRoute]Guid tenantId)
        {
            var userRoles = await userManager.GetRolesAsync(userId, tenantId);
            return IdentityUtilsResult<IList<string>>.SuccessResult(userRoles);
        }

        [HttpPost("{userId}/roles/{tenantId}/{roleId}")]
        public virtual async Task<IdentityUtilsResult> AddUserToRole([FromRoute]Guid userId, [FromRoute]Guid tenantId, [FromRoute]Guid roleId)
            => await userManager.AddToRoleAsync(userId, tenantId, roleId);

        [HttpDelete("{userId}/roles/{tenantId}/{roleId}")]
        public virtual async Task<IdentityUtilsResult> RemoveUserFromRole([FromRoute]Guid userId, [FromRoute]Guid tenantId, [FromRoute]Guid roleId)
            => await userManager.RemoveFromRoleAsync(userId, tenantId, roleId);
    }
}