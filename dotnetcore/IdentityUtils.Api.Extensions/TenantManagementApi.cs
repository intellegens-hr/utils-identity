using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
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
    public class TenantManagementApi<TTenantDto>
        where TTenantDto : class, IIdentityManagerTenantDto
    {
        private readonly RestClient restClient;

        public TenantManagementApi(RestClient restClient, IApiExtensionsConfig apiExtensionsConfig)
        {
            BasePath = $"{apiExtensionsConfig.Hostname.TrimEnd('/')}/{apiExtensionsConfig.TenantManagementBaseRoute.TrimStart('/')}";
            this.restClient = restClient;
        }

        private string BasePath { get; }

        public Task<IdentityUtilsResult<TTenantDto>> AddTenant(TTenantDto tenant)
            => restClient.Post<IdentityUtilsResult<TTenantDto>>($"{BasePath}", tenant).ParseRestResultTask();

        public Task<IdentityUtilsResult> DeleteTenant(Guid id)
            => restClient.Delete<IdentityUtilsResult>($"{BasePath}/{id}").ParseRestResultTask();

        public Task<IdentityUtilsResult<TTenantDto>> GetTenant(Guid id)
            => restClient.Get<IdentityUtilsResult<TTenantDto>>($"{BasePath}/{id}").ParseRestResultTask();

        public async Task<IdentityUtilsResult<TTenantDto>> GetTenants()
        {
            var response = await restClient.Get<IEnumerable<TTenantDto>>($"{BasePath}");
            return response.ToIdentityResult();
        }

        public Task<IdentityUtilsResult<TTenantDto>> Search(TenantSearch search)
            => restClient.Post<IdentityUtilsResult<TTenantDto>>($"{BasePath}/search", search).ParseRestResultTask();

        public Task<IdentityUtilsResult<TTenantDto>> UpdateTenant(TTenantDto tenant)
            => restClient.Post<IdentityUtilsResult<TTenantDto>>($"{BasePath}/{tenant.TenantId}", tenant).ParseRestResultTask();
    }
}