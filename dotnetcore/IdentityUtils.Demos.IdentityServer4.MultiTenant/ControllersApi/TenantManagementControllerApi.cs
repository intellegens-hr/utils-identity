using IdentityUtils.Api.Controllers;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/tenants")]
    public class TenantManagementControllerApi
        : TenantControllerApiAbstract<TenantDto>
    {
        public TenantManagementControllerApi(IIdentityManagerTenantService<TenantDto> tenantService)
            : base(tenantService)
        {
        }
    }
}