using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
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
    public class UserTenantManagementApi<TUserDto> : UserManagementCommon<TUserDto>
        where TUserDto : class, IIdentityManagerUserDto
    {
        public UserTenantManagementApi(RestClient restClient, IApiExtensionsConfig apiExtensionsConfig) : base(restClient, apiExtensionsConfig)
        {
        }

        public Task<IdentityUtilsResult> AddUserToRole(Guid userId, Guid tenantId, Guid roleId)
            => RestClient.Post<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{tenantId}/{roleId}").ParseRestResultTask();

        public Task<IdentityUtilsResult<RoleBasicData>> GetUserRoles(Guid userId, Guid tenantId)
            => RestClient.Get<IdentityUtilsResult<RoleBasicData>>($"{BasePath}/{userId}/roles/{tenantId}").ParseRestResultTask();

        public Task<IdentityUtilsResult> RemoveUserFromRole(Guid userId, Guid tenantId, Guid roleId)
            => RestClient.Delete<IdentityUtilsResult>($"{BasePath}/{userId}/roles/{tenantId}/{roleId}").ParseRestResultTask();

        public async Task<IdentityUtilsResult<TUserDto>> Search(UsersTenantSearch search)
            => (await RestClient.Post<IEnumerable<TUserDto>>($"{BasePath}/search", search)).ToIdentityResult();
    }
}