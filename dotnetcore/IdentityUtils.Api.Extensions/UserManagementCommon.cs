using IdentityUtils.Api.Models.Users;
using IdentityUtils.Commons;
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
    public class UserManagementCommon<TUserDto>
        where TUserDto : class, IIdentityManagerUserDto
    {
        public UserManagementCommon(RestClient restClient, IApiExtensionsConfig apiExtensionsConfig)
        {
            BasePath = $"{apiExtensionsConfig.Hostname.TrimEnd('/')}/{apiExtensionsConfig.UserManagementBaseRoute.TrimStart('/')}";
            RestClient = restClient;
        }

        protected string BasePath { get; }
        protected RestClient RestClient { get; }

        public Task<IdentityUtilsResult<TUserDto>> CreateUser(TUserDto userDto)
            => RestClient.Post<IdentityUtilsResult<TUserDto>>($"{BasePath}", userDto).ParseRestResultTask();

        public Task<IdentityUtilsResult> DeleteUser(Guid id)
            => RestClient.Delete<IdentityUtilsResult>($"{BasePath}/{id}").ParseRestResultTask();

        public async Task<IdentityUtilsResult<TUserDto>> GetAllUsers()
        {
            var response = await RestClient.Get<TUserDto>($"{BasePath}");
            return response.ToIdentityResult();
        }

        public Task<IdentityUtilsResult<PasswordForgottenResponse>> GetPasswordResetToken(PasswordForgottenRequest passwordForgottenRequest)
            => RestClient.Post<IdentityUtilsResult<PasswordForgottenResponse>>($"{BasePath}/passwordreset", passwordForgottenRequest).ParseRestResultTask();

        public Task<IdentityUtilsResult<TUserDto>> GetUserById(Guid id)
            => RestClient.Get<IdentityUtilsResult<TUserDto>>($"{BasePath}/{id}").ParseRestResultTask();

        public Task<IdentityUtilsResult> SetNewPasswordAfterReset(PasswordForgottenNewPassword newPassword)
            => RestClient.Post<IdentityUtilsResult>($"{BasePath}/passwordreset/newpassword", newPassword).ParseRestResultTask();

        public Task<IdentityUtilsResult<TUserDto>> UpdateUser(TUserDto userDto)
            => RestClient.Post<IdentityUtilsResult<TUserDto>>($"{BasePath}/{userDto.Id}", userDto).ParseRestResultTask();
    }
}