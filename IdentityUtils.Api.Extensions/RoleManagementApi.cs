using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    /// <summary>
    /// Wraps and exposes all role management API endpoints
    /// </summary>
    /// <typeparam name="TRoleDto">Role DTO model used</typeparam>
    public class RoleManagementApi<TRoleDto>
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        private readonly RestClient restClient;
        private string BasePath { get; }

        public RoleManagementApi(RestClient restClient, IApiExtensionsConfig apiExtensionsConfig)
        {
            BasePath = $"{apiExtensionsConfig.Hostname.TrimEnd('/')}/{apiExtensionsConfig.RoleManagementBaseRoute.TrimStart('/')}";
            this.restClient = restClient;
        }

        public Task<List<TRoleDto>> GetRoles()
            => restClient.Get<List<TRoleDto>>($"{BasePath}");

        public Task<IdentityUtilsResult<TRoleDto>> AddRole(TRoleDto role)
            => restClient.Post<IdentityUtilsResult<TRoleDto>>($"{BasePath}", role);

        public Task<IdentityUtilsResult> DeleteRole(Guid id)
            => restClient.Delete<IdentityUtilsResult>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult<TRoleDto>> GetRoleById(Guid id)
            => restClient.Get<IdentityUtilsResult<TRoleDto>>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult<TRoleDto>> GetRoleByNormalizedName(string roleName)
            => restClient.Get<IdentityUtilsResult<TRoleDto>>($"{BasePath}/rolename/{WebUtility.UrlEncode(roleName)}");
    }
}