using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    internal class TenantService : IdentityManagerTenantService<IdentityManagerTenant, TenantDto>
    {
        public TenantService(IIdentityManagerTenantContext<IdentityManagerTenant> dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}