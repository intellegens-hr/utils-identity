using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Services;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/tenants")]
    public class TenantManagementControllerApi
        : TenantControllerApiAbstract<IdentityManagerTenant, TenantDto>
    {
        public TenantManagementControllerApi(IdentityManagerTenantService<IdentityManagerTenant, TenantDto> tenantService)
            : base(tenantService)
        {
        }
    }
}