using IdentityUtils.Api.Models.Users;
using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Users;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    /// <summary>
    /// Wraps and exposes all tenant management API endpoints
    /// </summary>
    /// <typeparam name="TUserDto">User DTO model used</typeparam>
    public class UserManagementApi<TUserDto>
        where TUserDto : class, IIdentityManagerUserDto
    {
        private readonly RestClient restClient;
        private string BasePath { get; }

        public UserManagementApi(RestClient restClient, IApiExtensionsConfig apiExtensionsConfig)
        {
            BasePath = $"{apiExtensionsConfig.Hostname.TrimEnd('/')}/{apiExtensionsConfig.UserManagementBaseRoute.TrimStart('/')}";
            this.restClient = restClient;
        }

        public Task<List<TUserDto>> GetAllUsers()
            => restClient.Get<List<TUserDto>>($"{BasePath}");

        public Task<IdentityUtilsResult<TUserDto>> GetUserById(Guid id)
            => restClient.Get<IdentityUtilsResult<TUserDto>>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult> DeleteUser(Guid id)
            => restClient.Delete<IdentityUtilsResult>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult<TUserDto>> CreateUser(TUserDto userDto)
            => restClient.Post<IdentityUtilsResult<TUserDto>>($"{BasePath}", userDto);

        public Task<IdentityUtilsResult<TUserDto>> UpdateUser(TUserDto userDto)
           => restClient.Post<IdentityUtilsResult<TUserDto>>($"{BasePath}/{userDto.Id}", userDto);

        public Task<IdentityUtilsResult<TUserDto>> GetUserByUsername(string username)
            => restClient.Get<IdentityUtilsResult<TUserDto>>($"{BasePath}/by/{WebUtility.UrlEncode(username)}"); 

        public Task<IdentityUtilsResult<PasswordForgottenResponse>> GetPasswordResetToken(PasswordForgottenRequest passwordForgottenRequest)
            => restClient.Post<IdentityUtilsResult<PasswordForgottenResponse>>($"{BasePath}/passwordreset", passwordForgottenRequest);

        public Task<IdentityUtilsResult> SetNewPasswordAfterReset(PasswordForgottenNewPassword newPassword)
            => restClient.Post<IdentityUtilsResult>($"{BasePath}/passwordreset/newpassword", newPassword);

        public Task<IdentityUtilsResult<List<TUserDto>>> RoleUsersPerTenant(Guid tenantId, Guid roleId)
            => restClient.Get<IdentityUtilsResult<List<TUserDto>>>($"{BasePath}/roles/listusers/{tenantId}/{roleId}");

        public Task<IdentityUtilsResult<IList<string>>> GetUserRoles(Guid userId, Guid tenantId)
            => restClient.Get<IdentityUtilsResult<IList<string>>>($"{BasePath}/{userId}/roles/{tenantId}");

        public Task<IdentityUtilsResult> AddToRole(Guid userId, Guid tenantId, Guid roleId)
            => restClient.Post<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{tenantId}/{roleId}");

        public Task<IdentityUtilsResult> RemoveFromRole(Guid userId, Guid tenantId, Guid roleId)
            => restClient.Delete<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{tenantId}/{roleId}");
    }
}