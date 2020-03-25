using AutoMapper;
using AutoMapper.QueryableExtensions;
using Commons.Validation;
using IdentityUtils.Core.Contracts;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Tenants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityUtils.Core.Services
{
    public class IdentityManagerTenantService<TTenant, TTenantDto>
        where TTenant : IdentityManagerTenant
        where TTenantDto : class, IIdentityManagerTenantDto
    {
        private readonly IIdentityManagerTenantContext<TTenant> dbContext;
        private readonly IMapper mapper;

        public IdentityManagerTenantService(
            IIdentityManagerTenantContext<TTenant> dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<IList<TTenantDto>> GetTenants()
        {
            return await dbContext
                .Tenants
                .ProjectTo<TTenantDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TTenantDto> GetTenantByHostname(string hostname)
        {
            var tenant = await dbContext.Tenants.FirstOrDefaultAsync(x => x.Hostname.ToLower().Contains(hostname));
            return mapper.Map<TTenantDto>(tenant);
        }

        private async Task<IdentityManagementResult<TTenant>> GetTenantDb(Guid id)
        {
            var tenant = await dbContext.Tenants.FirstOrDefaultAsync(x => x.TenantId == id);

            var result = tenant == null
                ? IdentityManagementResult<TTenant>.ErrorResult("Tenant with specified ID does not exist")
                : IdentityManagementResult<TTenant>.SuccessResult(tenant);

            return result;
        }

        public async Task<IdentityManagementResult<TTenantDto>> GetTenant(Guid id)
        {
            var tenantResult = await GetTenantDb(id);

            var result = tenantResult.Success
                ? IdentityManagementResult<TTenantDto>.SuccessResult(mapper.Map<TTenantDto>(tenantResult.Payload))
                : IdentityManagementResult<TTenantDto>.ErrorResult(tenantResult.ErrorMessages);

            return result;
        }

        public async Task<IdentityManagementResult<TTenantDto>> UpdateTenant(TTenantDto tenantDto)
        {
            var tenantDbResult = await GetTenantDb(tenantDto.TenantId);
            if (!tenantDbResult.Success)
                return IdentityManagementResult<TTenantDto>.ErrorResult(tenantDbResult.ErrorMessages);

            var tenant = tenantDbResult.Payload;
            mapper.Map(tenantDto, tenant);

            var result = ModelValidator.ValidateDataAnnotations(tenant);
            if (!result.isValid)
                return IdentityManagementResult<TTenantDto>.ErrorResult(result.ToIdentityManagementResult().ErrorMessages);

            dbContext.Tenants.Update(tenant);
            await dbContext.SaveChangesAsync();

            return IdentityManagementResult<TTenantDto>.SuccessResult(mapper.Map<TTenantDto>(tenant));
        }

        public async Task<IdentityManagementResult<TTenantDto>> AddTenant(TTenantDto tenantDto)
        {
            var tenant = mapper.Map<TTenant>(tenantDto);

            var result = ModelValidator.ValidateDataAnnotations(tenant);
            if (!result.isValid)
                return IdentityManagementResult<TTenantDto>.ErrorResult(result.ToIdentityManagementResult().ErrorMessages);

            dbContext.Tenants.Add(tenant);
            await dbContext.SaveChangesAsync();

            return IdentityManagementResult<TTenantDto>.SuccessResult(mapper.Map<TTenantDto>(tenant));
        }

        public async Task<IdentityManagementResult> DeleteTenant(Guid id)
        {
            var tenantDbResult = await GetTenantDb(id);
            if (!tenantDbResult.Success)
                return IdentityManagementResult.ErrorResult(tenantDbResult.ErrorMessages);

            dbContext.Tenants.Remove(tenantDbResult.Payload);
            await dbContext.SaveChangesAsync();

            return IdentityManagementResult.SuccessResult;
        }
    }
}