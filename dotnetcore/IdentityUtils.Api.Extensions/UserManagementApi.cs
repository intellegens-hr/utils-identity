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

        public async Task<IdentityUtilsResult<List<TUserDto>>> GetAllUsers()
        {
            var response = await restClient.Get<List<TUserDto>>($"{BasePath}");
            return response.ToIdentityResult();
        }

        public Task<IdentityUtilsResult<TUserDto>> GetUserById(Guid id)
            => restClient.Get<IdentityUtilsResult<TUserDto>>($"{BasePath}/{id}").ParseRestResultTask();

        public Task<IdentityUtilsResult> DeleteUser(Guid id)
            => restClient.Delete<IdentityUtilsResult>($"{BasePath}/{id}").ParseRestResultTask();

        public Task<IdentityUtilsResult<TUserDto>> CreateUser(TUserDto userDto)
            => restClient.Post<IdentityUtilsResult<TUserDto>>($"{BasePath}", userDto).ParseRestResultTask();

        public Task<IdentityUtilsResult<TUserDto>> UpdateUser(TUserDto userDto)
           => restClient.Post<IdentityUtilsResult<TUserDto>>($"{BasePath}/{userDto.Id}", userDto).ParseRestResultTask();

        public Task<IdentityUtilsResult<TUserDto>> GetUserByUsername(string username)
            => restClient.Get<IdentityUtilsResult<TUserDto>>($"{BasePath}/by/{WebUtility.UrlEncode(username)}").ParseRestResultTask(); 

        public Task<IdentityUtilsResult<PasswordForgottenResponse>> GetPasswordResetToken(PasswordForgottenRequest passwordForgottenRequest)
            => restClient.Post<IdentityUtilsResult<PasswordForgottenResponse>>($"{BasePath}/passwordreset", passwordForgottenRequest).ParseRestResultTask();

        public Task<IdentityUtilsResult> SetNewPasswordAfterReset(PasswordForgottenNewPassword newPassword)
            => restClient.Post<IdentityUtilsResult>($"{BasePath}/passwordreset/newpassword", newPassword).ParseRestResultTask();

        public Task<IdentityUtilsResult<List<TUserDto>>> RoleUsersPerTenant(Guid tenantId, Guid roleId)
            => restClient.Get<IdentityUtilsResult<List<TUserDto>>>($"{BasePath}/roles/listusers/{tenantId}/{roleId}").ParseRestResultTask();

        public Task<IdentityUtilsResult<IList<string>>> GetUserRoles(Guid userId, Guid tenantId)
            => restClient.Get<IdentityUtilsResult<IList<string>>>($"{BasePath}/{userId}/roles/{tenantId}").ParseRestResultTask();

        public Task<IdentityUtilsResult> AddToRole(Guid userId, Guid tenantId, Guid roleId)
            => restClient.Post<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{tenantId}/{roleId}").ParseRestResultTask();

        public Task<IdentityUtilsResult> RemoveFromRole(Guid userId, Guid tenantId, Guid roleId)
            => restClient.Delete<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{tenantId}/{roleId}").ParseRestResultTask();
    }
}