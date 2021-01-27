using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Tenants;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Controllers
{
    /// <summary>
    /// Basic controller which provides all needed API endpoints for tenant management.
    /// </summary>
    /// <typeparam name="TTenantDto">Tenant DTO model</typeparam>
    [ApiController]
    public abstract class TenantControllerApiAbstract<TTenantDto> : ControllerBase
        where TTenantDto : class, IIdentityManagerTenantDto
    {
        private readonly IIdentityManagerTenantService<TTenantDto> tenantService;

        public TenantControllerApiAbstract(IIdentityManagerTenantService<TTenantDto> tenantService)
        {
            this.tenantService = tenantService;
        }

        [HttpPost]
        public virtual async Task<IdentityUtilsResult<TTenantDto>> AddTenant([FromBody] TTenantDto tenant)
            => await tenantService.AddTenant(tenant);

        [HttpDelete("{tenantId}")]
        public virtual async Task<IdentityUtilsResult> DeleteTenant([FromRoute] Guid tenantId)
            => await tenantService.DeleteTenant(tenantId);

        [HttpGet("{tenantId}")]
        public virtual async Task<IdentityUtilsResult<TTenantDto>> GetTenant([FromRoute] Guid tenantId)
            => await tenantService.GetTenant(tenantId);

        [HttpGet]
        public virtual async Task<IEnumerable<TTenantDto>> GetTenants()
            => await tenantService.GetTenants();

        [HttpPost("search")]
        public virtual async Task<IdentityUtilsResult<TTenantDto>> Search([FromBody] TenantSearch tenantSearchRequest)
        {
            return IdentityUtilsResult<TTenantDto>.SuccessResult(await tenantService.Search(tenantSearchRequest));
        }

        [HttpPost("{tenantId}")]
        public virtual async Task<IdentityUtilsResult<TTenantDto>> UpdateTenant([FromRoute] Guid tenantId, [FromBody] TTenantDto tenant)
        {
            if (tenantId != tenant.TenantId)
                return IdentityUtilsResult<TTenantDto>.ErrorResult("Id specified in route and payload are not the same!");

            return await tenantService.UpdateTenant(tenant);
        }
    }
}