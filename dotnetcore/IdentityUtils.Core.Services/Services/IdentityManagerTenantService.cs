using AutoMapper;
using IdentityUtils.Commons.Validation;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Contracts.Tenants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace IdentityUtils.Core.Services
{
    public class IdentityManagerTenantService<TTenant, TTenantDto> : IIdentityManagerTenantService<TTenantDto>
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

        public async Task<IdentityUtilsResult<TTenantDto>> AddTenant(TTenantDto tenantDto)
        {
            tenantDto.Hostnames ??= new List<string>();

            var hostsAlreadyExist = await dbContext.TenantHosts
                .Where(x => tenantDto.Hostnames.Contains(x.Hostname))
                .AnyAsync();

            if (hostsAlreadyExist)
                return IdentityUtilsResult<TTenantDto>.ErrorResult("Hostname already assigned to different tenant");

            var tenant = mapper.Map<TTenant>(tenantDto);
            tenant.Hosts = tenantDto.Hostnames
                .Select(x => new IdentityManagerTenantHost
                {
                    Hostname = x.Trim()
                })
                .ToList();

            var result = ModelValidator.ValidateDataAnnotations(tenant);
            if (!result.isValid)
                return IdentityUtilsResult<TTenantDto>.ErrorResult(result.ToIdentityUtilsResult().ErrorMessages);

            dbContext.Tenants.Add(tenant);
            await dbContext.SaveChangesAsync();

            return IdentityUtilsResult<TTenantDto>.SuccessResult(ToDto(tenant));
        }

        public async Task<IdentityUtilsResult> DeleteTenant(Guid id)
        {
            var tenantDbResult = await GetTenantDb(id);
            if (!tenantDbResult.Success)
                return IdentityUtilsResult.ErrorResult(tenantDbResult.ErrorMessages);

            var hostsToDelete = dbContext.TenantHosts.Where(x => x.TenantId == id);

            dbContext.TenantHosts.RemoveRange(hostsToDelete);
            dbContext.Tenants.Remove(tenantDbResult.Data);
            await dbContext.SaveChangesAsync();

            return IdentityUtilsResult.SuccessResult;
        }

        public async Task<IdentityUtilsResult<TTenantDto>> GetTenant(Guid id)
        {
            var tenantResult = await GetTenantDb(id);

            var result = tenantResult.Success
                ? IdentityUtilsResult<TTenantDto>.SuccessResult(ToDto(tenantResult.Data))
                : IdentityUtilsResult<TTenantDto>.ErrorResult(tenantResult.ErrorMessages);

            return result;
        }

        public async Task<IList<TTenantDto>> GetTenants()
        {
            var tenants = await dbContext
                .Tenants
                .Include(x => x.Hosts)
                .ToListAsync();

            return tenants
                .Select(x => ToDto(x))
                .ToList();
        }

        public async Task<IEnumerable<TTenantDto>> Search(TenantSearch searchParams)
        {
            var tenantsQuery = dbContext
                .Tenants
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchParams.Hostname))
                tenantsQuery = tenantsQuery.Where(x => x.Hosts.Where(y => y.Hostname.ToLower().Contains(searchParams.Hostname)).Any());

            var tenants = await tenantsQuery
                .Include(x => x.Hosts)
                .ToListAsync();

            return ToDto(tenants);
        }

        public async Task<IdentityUtilsResult<TTenantDto>> UpdateTenant(TTenantDto tenantDto)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var hostsAlreadyExist = await dbContext.TenantHosts
                .Where(x => x.TenantId != tenantDto.TenantId)
                .Where(x => tenantDto.Hostnames.Contains(x.Hostname))
                .AnyAsync();

            if (hostsAlreadyExist)
                return IdentityUtilsResult<TTenantDto>.ErrorResult("Hostname already assigned to different tenant");

            var currentHosts = dbContext.TenantHosts.Where(x => x.TenantId == tenantDto.TenantId);
            dbContext.TenantHosts.RemoveRange(currentHosts);
            await dbContext.SaveChangesAsync();

            var tenantDbResult = await GetTenantDb(tenantDto.TenantId, false);
            if (!tenantDbResult.Success)
                return IdentityUtilsResult<TTenantDto>.ErrorResult(tenantDbResult.ErrorMessages);

            var tenant = tenantDbResult.Data;
            mapper.Map(tenantDto, tenant);
            var hosts = tenantDto.Hostnames
                .Select(x => new IdentityManagerTenantHost
                {
                    TenantId = tenantDto.TenantId,
                    Hostname = x
                })
                .ToList();

            var result = ModelValidator.ValidateDataAnnotations(tenant);
            if (!result.isValid)
                return IdentityUtilsResult<TTenantDto>.ErrorResult(result.ToIdentityUtilsResult().ErrorMessages);

            dbContext.Tenants.Update(tenant);
            dbContext.TenantHosts.AddRange(hosts);
            await dbContext.SaveChangesAsync();

            scope.Complete();

            return IdentityUtilsResult<TTenantDto>.SuccessResult(ToDto(tenant));
        }

        private async Task<IdentityUtilsResult<TTenant>> GetTenantDb(Guid id, bool includeHosts = true)
        {
            var tenantQuery = dbContext.Tenants.Where(x => x.TenantId == id);
            TTenant tenant = null;

            if (includeHosts)
                tenant = await tenantQuery.Include(x => x.Hosts).FirstOrDefaultAsync();
            else
                tenant = await tenantQuery.FirstOrDefaultAsync();

            var result = tenant == null
                ? IdentityUtilsResult<TTenant>.ErrorResult("Tenant with specified ID does not exist")
                : IdentityUtilsResult<TTenant>.SuccessResult(tenant);

            return result;
        }

        private TTenantDto ToDto(TTenant tenant)
        {
            var tenantDto = mapper.Map<TTenantDto>(tenant);
            tenantDto.Hostnames = tenant
                .Hosts
                .Select(x => x.Hostname)
                .ToList();

            return tenantDto;
        }

        private IEnumerable<TTenantDto> ToDto(IList<TTenant> tenants)
        {
            foreach (var tenant in tenants)
                yield return ToDto(tenant);
        }
    }
}