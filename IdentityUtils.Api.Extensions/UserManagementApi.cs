using IdentityUtils.Api.Models.Users;
using IdentityUtils.Core.Contracts;
using IdentityUtils.Core.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    /// <summary>
    /// Wraps and exposes all tenant management API endpoints
    /// </summary>
    /// <typeparam name="TUserDto">User DTO model used</typeparam>
    public class UserManagementApi<TUserDto> : ApiHttpClient
        where TUserDto : class, IIdentityManagerUserDto
    {
        protected override string BasePath { get; }
        protected override IApiWrapperConfig WrapperConfig { get; }

        public UserManagementApi(IApiWrapperConfig wrapperConfig)
        {
            BasePath = $"{wrapperConfig.Is4Hostname.TrimEnd('/')}/api/intellegens/users";
            WrapperConfig = wrapperConfig;
        }

        public Task<List<TUserDto>> GetAllUsers()
            => Get<List<TUserDto>>($"{BasePath}");

        public Task<IdentityManagementResult<TUserDto>> GetUserById(Guid id)
            => Get<IdentityManagementResult<TUserDto>>($"{BasePath}/{id}");

        public Task<IdentityManagementResult> DeleteUser(Guid id)
            => Delete<IdentityManagementResult>($"{BasePath}/{id}");

        public Task<IdentityManagementResult<TUserDto>> CreateUser(TUserDto userDto)
            => Post<IdentityManagementResult<TUserDto>>($"{BasePath}", userDto);

        public Task<IdentityManagementResult<TUserDto>> UpdateUser(TUserDto userDto)
           => Post<IdentityManagementResult<TUserDto>>($"{BasePath}/{userDto.Id}", userDto);

        public Task<IdentityManagementResult<TUserDto>> GetUserByUsername(string username)
             => Get<IdentityManagementResult<TUserDto>>($"{BasePath}/{username}"); //TODO: encode

        public Task<IdentityManagementResult<PasswordForgottenResponse>> GetPasswordResetToken(PasswordForgottenRequest passwordForgottenRequest)
            => Post<IdentityManagementResult<PasswordForgottenResponse>>($"{BasePath}/passwordreset", passwordForgottenRequest);

        public Task<IdentityManagementResult> SetNewPasswordAfterReset(PasswordForgottenNewPassword newPassword)
            => Post<IdentityManagementResult>($"{BasePath}/passwordreset/newpassword", newPassword);

        public Task<IdentityManagementResult<List<TUserDto>>> RoleUsersPerTenant(Guid roleId, Guid tenantId)
            => Get<IdentityManagementResult<List<TUserDto>>>($"{BasePath}/roles/listusers/{roleId}/{tenantId}");

        public Task<IdentityManagementResult> AddToRole(Guid userId, Guid roleId, Guid tenantId)
            => Get<IdentityManagementResult>($"{BasePath}/{userId}/roles/{roleId}/{tenantId}");

        public Task<IdentityManagementResult> RemoveFromRole(Guid userId, Guid roleId, Guid tenantId)
            => Delete<IdentityManagementResult>($"{BasePath}/{userId}/roles/{roleId}/{tenantId}");
    }
}