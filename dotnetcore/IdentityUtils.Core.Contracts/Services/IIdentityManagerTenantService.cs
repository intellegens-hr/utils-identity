using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Contracts.Services
{
    public interface IIdentityManagerTenantService<TTenantDto>
        where TTenantDto : class, IIdentityManagerTenantDto
    {
        Task<IdentityUtilsResult<TTenantDto>> AddTenant(TTenantDto tenantDto);

        Task<IdentityUtilsResult> DeleteTenant(Guid id);

        Task<IdentityUtilsResult<TTenantDto>> GetTenant(Guid id);

        Task<IList<TTenantDto>> GetTenants();

        Task<IEnumerable<TTenantDto>> Search(TenantSearch searchParams);

        Task<IdentityUtilsResult<TTenantDto>> UpdateTenant(TTenantDto tenantDto);
    }
}