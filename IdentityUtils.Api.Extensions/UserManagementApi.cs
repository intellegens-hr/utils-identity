using IdentityUtils.Api.Models.Users;
using IdentityUtils.Core.Contracts.Commons;
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

        public Task<IdentityUtilsResult<TUserDto>> GetUserById(Guid id)
            => Get<IdentityUtilsResult<TUserDto>>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult> DeleteUser(Guid id)
            => Delete<IdentityUtilsResult>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult<TUserDto>> CreateUser(TUserDto userDto)
            => Post<IdentityUtilsResult<TUserDto>>($"{BasePath}", userDto);

        public Task<IdentityUtilsResult<TUserDto>> UpdateUser(TUserDto userDto)
           => Post<IdentityUtilsResult<TUserDto>>($"{BasePath}/{userDto.Id}", userDto);

        public Task<IdentityUtilsResult<TUserDto>> GetUserByUsername(string username)
             => Get<IdentityUtilsResult<TUserDto>>($"{BasePath}/{username}"); //TODO: encode

        public Task<IdentityUtilsResult<PasswordForgottenResponse>> GetPasswordResetToken(PasswordForgottenRequest passwordForgottenRequest)
            => Post<IdentityUtilsResult<PasswordForgottenResponse>>($"{BasePath}/passwordreset", passwordForgottenRequest);

        public Task<IdentityUtilsResult> SetNewPasswordAfterReset(PasswordForgottenNewPassword newPassword)
            => Post<IdentityUtilsResult>($"{BasePath}/passwordreset/newpassword", newPassword);

        public Task<IdentityUtilsResult<List<TUserDto>>> RoleUsersPerTenant(Guid roleId, Guid tenantId)
            => Get<IdentityUtilsResult<List<TUserDto>>>($"{BasePath}/roles/listusers/{roleId}/{tenantId}");

        public Task<IdentityUtilsResult> AddToRole(Guid userId, Guid roleId, Guid tenantId)
            => Get<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{roleId}/{tenantId}");

        public Task<IdentityUtilsResult> RemoveFromRole(Guid userId, Guid roleId, Guid tenantId)
            => Delete<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{roleId}/{tenantId}");
    }
}