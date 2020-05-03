using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    internal class TenantsService : IdentityManagerTenantService<TenantDb, TenantDto>
    {
        public TenantsService(IIdentityManagerTenantContext<TenantDb> dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }
    }
}