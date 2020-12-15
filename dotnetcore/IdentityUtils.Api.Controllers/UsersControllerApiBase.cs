using AutoMapper;
using IdentityUtils.Api.Models.Users;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Controllers
{
    /// <summary>
    /// Common enpoints for both single and multi-tenant mode.
    /// </summary>
    /// <typeparam name="TUser">User database model</typeparam>
    /// <typeparam name="TUserDto">User DTO model</typeparam>
    [ApiController]
    public abstract class UsersControllerApiBase<TUser, TUserDto> : ControllerBase
        where TUser : IdentityManagerUser
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly IMapper mapper;
        private readonly IIdentityManagerUserServiceCommon<TUser, TUserDto> userManager;

        public UsersControllerApiBase(
            IIdentityManagerUserServiceCommon<TUser, TUserDto> userManager,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost]
        public virtual async Task<IdentityUtilsResult<TUserDto>> CreateUser([FromBody] TUserDto userDto)
        {
            return await userManager.CreateUser(userDto);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IdentityUtilsResult> DeleteUser([FromRoute] Guid id)
            => await userManager.DeleteUser(id);

        [HttpGet]
        public virtual async Task<List<TUserDto>> GetAllUsers()
            => mapper.Map<List<TUser>, List<TUserDto>>(await userManager.GetAllUsers());

        [HttpPost("passwordreset")]
        public virtual async Task<IdentityUtilsResult<PasswordForgottenResponse>> GetPasswordResetToken([FromBody] PasswordForgottenRequest passwordForgottenRequest)
        {
            var userResult = await userManager.FindByNameAsync(passwordForgottenRequest.Username);
            if (!userResult.Success)
                return userResult.ToTypedResult<PasswordForgottenResponse>();

            var resetTokenResult = await userManager.GeneratePasswordResetTokenAsync(passwordForgottenRequest.Username);
            if (!resetTokenResult.Success)
                return resetTokenResult.ToTypedResult<PasswordForgottenResponse>();

            var passwordResponse = new PasswordForgottenResponse
            {
                Username = userResult.Data.UserName,
                Email = userResult.Data.Email,
                Token = resetTokenResult.Data
            };

            return IdentityUtilsResult<PasswordForgottenResponse>.SuccessResult(passwordResponse);
        }

        [HttpGet("{id}")]
        public virtual async Task<IdentityUtilsResult<TUserDto>> GetUserById([FromRoute] Guid id)
            => await userManager.FindByIdAsync<TUserDto>(id);

        [HttpPost("passwordreset/newpassword")]
        public virtual async Task<IdentityUtilsResult> SetNewPasswordAfterReset([FromBody] PasswordForgottenNewPassword newPassword)
        {
            return await userManager.ResetPasswordAsync(newPassword.Username, newPassword.Token, newPassword.Password);
        }

        [HttpPost("{userId}")]
        public virtual async Task<IdentityUtilsResult<TUserDto>> UpdateUser([FromRoute] Guid userId, [FromBody] TUserDto userDto)
        {
            if (userId != userDto.Id)
                throw new UnauthorizedAccessException();

            var result = await userManager.UpdateUser(userDto);
            return result.ToTypedResult(userDto);
        }
    }
}