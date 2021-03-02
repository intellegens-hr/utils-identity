using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Users;
using System;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    /// <summary>
    /// Wraps and exposes all tenant management API endpoints
    /// </summary>
    /// <typeparam name="TUserDto">User DTO model used</typeparam>
    public class UserManagementApi<TUserDto> : UserManagementCommon<TUserDto>
        where TUserDto : class, IIdentityManagerUserDto
    {
        public UserManagementApi(RestClient restClient, IApiExtensionsConfig apiExtensionsConfig) : base(restClient, apiExtensionsConfig)
        {
        }

        public Task<IdentityUtilsResult> AddUserToRole(Guid userId, Guid roleId)
            => RestClient.Post<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{roleId}").ParseRestResultTask();

        public Task<IdentityUtilsResult<RoleBasicData>> GetUserRoles(Guid userId)
            => RestClient.Get<IdentityUtilsResult<RoleBasicData>>($"{BasePath}/{userId}/roles").ParseRestResultTask();

        public Task<IdentityUtilsResult> RemoveUserFromRole(Guid userId, Guid roleId)
            => RestClient.Delete<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{roleId}").ParseRestResultTask();

        public Task<IdentityUtilsResult<TUserDto>> Search(UsersSearch search)
            => RestClient.Post<IdentityUtilsResult<TUserDto>>($"{BasePath}/search", search).ParseRestResultTask();
    }
}