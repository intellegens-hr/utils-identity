using IdentityUtils.Api.Models.Tenants;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions
{
    /// <summary>
    /// Wraps and exposes all tenant management API endpoints
    /// </summary>
    /// <typeparam name="TTenantDto">Tenant DTO model used</typeparam>
    public class TenantManagementApi<TTenantDto> : ApiHttpClient
        where TTenantDto : class, IIdentityManagerTenantDto
    {
        protected override string BasePath { get; }
        protected override IApiWrapperConfig WrapperConfig { get; }

        public TenantManagementApi(IApiWrapperConfig wrapperConfig)
        {
            BasePath = $"{wrapperConfig.Is4Hostname.TrimEnd('/')}/api/intellegens/tenants";
            WrapperConfig = wrapperConfig;
        }

        public Task<List<TTenantDto>> GetTenants()
            => Get<List<TTenantDto>>($"{BasePath}");

        public Task<IdentityUtilsResult<TTenantDto>> AddTenant(TTenantDto tenant)
            => Post<IdentityUtilsResult<TTenantDto>>($"{BasePath}", tenant);

        public Task<IdentityUtilsResult<TTenantDto>> GetTenant(Guid id)
            => Get<IdentityUtilsResult<TTenantDto>>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult> DeleteTenant(Guid id)
            => Delete<IdentityUtilsResult>($"{BasePath}/{id}");

        public Task<IdentityUtilsResult<TTenantDto>> UpdateTenant(TTenantDto tenant)
            => Post<IdentityUtilsResult<TTenantDto>>($"{BasePath}/{tenant.TenantId}", tenant);

        public Task<TTenantDto> GetTenantByHostname(string hostname)
            => Post<TTenantDto>($"{BasePath}/byhostname", new TenantRequest { Hostname = hostname });
    }
}