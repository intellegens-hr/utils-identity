using AutoMapper;
using Commons.Mailing;
using IdentityUtils.ApiExtension.Models.Users;
using IdentityUtils.Core.Contracts;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.ApiExtension.Controllers
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
        private readonly IMailingProvider mailingProvider;
        private readonly IMapper mapper;

        public UsersControllerApiAbstract(
            IdentityManagerUserService<TUser, TUserDto, TRole> userManager,
            IMailingProvider mailingProvider,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.mailingProvider = mailingProvider;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<List<TUserDto>> GetAllUsers()
            => mapper.Map<List<TUser>, List<TUserDto>>(await userManager.GetAllUsers());

        [HttpGet("{id}")]
        public async Task<IdentityManagementResult<TUserDto>> GetUserById([FromRoute]Guid id)
            => await userManager.FindByIdAsync<TUserDto>(id);

        [HttpDelete("{id}")]
        public async Task<IdentityManagementResult> DeleteUser([FromRoute]Guid id)
            => await userManager.DeleteUser(id);

        [HttpPost()]
        public async Task<IdentityManagementResult<TUserDto>> CreateUser([FromBody]TUserDto userDto)
        {
            var result = await userManager.CreateUser(userDto);
            return result.ToTypedResult(userDto);
        }

        [HttpPost("{id}")]
        public async Task<IdentityManagementResult<TUserDto>> UpdateUser([FromRoute]Guid id, [FromBody]TUserDto userDto)
        {
            if (id != userDto.Id)
                throw new UnauthorizedAccessException();

            var result = await userManager.UpdateUser(userDto);
            return result.ToTypedResult(userDto);
        }

        [HttpGet("by/{username}")]
        public async Task<IdentityManagementResult<TUserDto>> GetUserByUsername([FromRoute]string username)
            => await userManager.FindByNameAsync<TUserDto>(username);

        [HttpPost("passwordreset")]
        public async Task<IdentityManagementResult<PasswordForgottenResponse>> GetPasswordResetToken([FromBody]PasswordForgottenRequest passwordForgottenRequest)
        {
            var userResult = await userManager.FindByNameAsync(passwordForgottenRequest.Username);
            if (!userResult.Success)
                return userResult.ToTypedResult<PasswordForgottenResponse>();

            var user = userResult.Payload;
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            var mailMessage = new MailMessageSimple("intellegensdemo@gmail.com", user.Email, "Your password reset request", resetToken);

            try
            {
                await mailingProvider.SendEmailMessage(mailMessage);
            }
            catch
            {
                return IdentityManagementResult<PasswordForgottenResponse>.ErrorResult("Error sending e-mail");
            }

            var passwordResponse = new PasswordForgottenResponse
            {
                Username = user.UserName,
                Token = resetToken
            };

            return IdentityManagementResult<PasswordForgottenResponse>.SuccessResult(passwordResponse);
        }

        [HttpPost("passwordreset/newpassword")]
        public async Task<IdentityManagementResult> SetNewPasswordAfterReset([FromBody]PasswordForgottenNewPassword newPassword)
        {
            var user = await userManager.FindByNameAsync(newPassword.Username);
            return await userManager.ResetPasswordAsync(user.Payload, newPassword.Token, newPassword.Password);
        }

        [HttpGet("roles/listusers/{roleId}/{tenantId}")]
        public async Task<IdentityManagementResult<List<TUserDto>>> GetRoleById([FromRoute]Guid roleId, [FromRoute]Guid tenantId)
            => await userManager.RoleUsersPerTenant(roleId, tenantId);

        [HttpGet("{userId}/roles/{roleId}/{tenantId}")]
        public async Task<IdentityManagementResult> AddUserToRole([FromRoute]Guid userId, [FromRoute]Guid roleId, [FromRoute]Guid tenantId)
            => await userManager.AddToRoleAsync(userId, tenantId, roleId);

        [HttpDelete("{userId}/roles/{roleId}/{tenantId}")]
        public async Task<IdentityManagementResult> RemoveUserFromRole([FromRoute]Guid userId, [FromRoute]Guid roleId, [FromRoute]Guid tenantId)
            => await userManager.RemoveFromRoleAsync(userId, tenantId, roleId);
    }
}