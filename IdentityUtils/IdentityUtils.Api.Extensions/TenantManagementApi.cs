using IdentityUtils.Api.Models.Tenants;
using IdentityUtils.Core.Contracts;
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

        public Task<IdentityManagementResult<TTenantDto>> AddTenant(TTenantDto tenant)
            => Post<IdentityManagementResult<TTenantDto>>($"{BasePath}", tenant);

        public Task<IdentityManagementResult<TTenantDto>> GetTenant(Guid id)
            => Get<IdentityManagementResult<TTenantDto>>($"{BasePath}/{id}");

        public Task<IdentityManagementResult> DeleteTenant(Guid id)
            => Delete<IdentityManagementResult>($"{BasePath}/{id}");

        public Task<IdentityManagementResult<TTenantDto>> UpdateTenant(TTenantDto tenant)
            => Post<IdentityManagementResult<TTenantDto>>($"{BasePath}/{tenant.TenantId}", tenant);

        public Task<TTenantDto> GetTenantByHostname(string hostname)
            => Post<TTenantDto>($"{BasePath}/byhostname", new TenantRequest { Hostname = hostname });
    }
}