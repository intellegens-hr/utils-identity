using IdentityUtils.Core.Contracts;
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
    public class RoleManagementApi<TRoleDto> : ApiHttpClient
        where TRoleDto : class, IIdentityManagerRoleDto
    {
        protected override string BasePath { get; }
        protected override IApiWrapperConfig WrapperConfig { get; }

        public RoleManagementApi(IApiWrapperConfig wrapperConfig)
        {
            BasePath = $"{wrapperConfig.Is4Hostname.TrimEnd('/')}/api/intellegens/roles";
            WrapperConfig = wrapperConfig;
        }

        public Task<List<TRoleDto>> GetRoles()
            => Get<List<TRoleDto>>($"{BasePath}");

        public Task<IdentityManagementResult<TRoleDto>> AddRole(TRoleDto role)
            => Post<IdentityManagementResult<TRoleDto>>($"{BasePath}", role);

        public Task<IdentityManagementResult> DeleteRole(Guid id)
            => Delete<IdentityManagementResult>($"{BasePath}/{id}");

        public Task<IdentityManagementResult<TRoleDto>> GetRoleById(Guid id)
            => Get<IdentityManagementResult<TRoleDto>>($"{BasePath}/{id}");

        public Task<IdentityManagementResult<TRoleDto>> GetRoleByNormalizedName(string roleName)
            => Get<IdentityManagementResult<TRoleDto>>($"{BasePath}/rolename/{WebUtility.UrlEncode(roleName)}");
    }
}