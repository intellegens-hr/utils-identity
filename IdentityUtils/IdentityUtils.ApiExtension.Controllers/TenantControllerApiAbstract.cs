using IdentityUtils.ApiExtension.Models.Tenants;
using IdentityUtils.Core.Contracts;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.ApiExtension.Controllers
{
    /// <summary>
    /// Basic controller which provides all needed API endpoints for tenant management.
    /// </summary>
    /// <typeparam name="TTenant">Tenant database model</typeparam>
    /// <typeparam name="TTenantDto">Tenant DTO model</typeparam>
    [ApiController]
    public abstract class TenantControllerApiAbstract<TTenant, TTenantDto> : ControllerBase
        where TTenant : IdentityManagerTenant
        where TTenantDto : class, IIdentityManagerTenantDto
    {
        private readonly IdentityManagerTenantService<TTenant, TTenantDto> tenantService;

        public TenantControllerApiAbstract(IdentityManagerTenantService<TTenant, TTenantDto> tenantService)
        {
            this.tenantService = tenantService;
        }

        [HttpGet]
        public async Task<IList<TTenantDto>> GetTenants()
            => await tenantService.GetTenants();

        [HttpPost]
        public async Task<IdentityManagementResult<TTenantDto>> AddTenant([FromBody]TTenantDto tenant)
            => await tenantService.AddTenant(tenant);

        [HttpGet("{tenantId}")]
        public async Task<IdentityManagementResult<TTenantDto>> GetTenant([FromRoute]Guid tenantId)
            => await tenantService.GetTenant(tenantId);

        [HttpPost("{tenantId}")]
        public async Task<IdentityManagementResult<TTenantDto>> UpdateTenant([FromRoute]Guid tenantId, [FromBody]TTenantDto tenant)
        {
            if (tenantId != tenant.TenantId)
                return IdentityManagementResult<TTenantDto>.ErrorResult("Id specified in route and payload are not the same!");

            return await tenantService.UpdateTenant(tenant);
        }

        [HttpDelete("{tenantId}")]
        public async Task<IdentityManagementResult> DeleteTenant([FromRoute]Guid tenantId)
            => await tenantService.DeleteTenant(tenantId);

        [HttpPost("byhostname")]
        public async Task<TTenantDto> GetByHostname([FromBody]TenantRequest tenantRequest)
        {
            return await tenantService.GetTenantByHostname(tenantRequest.Hostname);
        }
    }
}