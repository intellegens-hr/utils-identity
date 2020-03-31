using IdentityUtils.Api.Extensions;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    public class TenantApiTests : TestAbstract
    {
        private readonly TenantManagementApi<TenantDto> tenantManagementApi;

        public TenantApiTests() : base()
        {
            tenantManagementApi = serviceProvider.GetRequiredService<TenantManagementApi<TenantDto>>();
        }

        private TenantDto GetUniqueTestTenant => new TenantDto
        {
            Name = "Test tenant - " + Guid.NewGuid().ToString(),
            Hostnames = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
        };

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var tenantDto = GetUniqueTestTenant;

            var result = await tenantManagementApi.AddTenant(tenantDto);

            Assert.True(result.Success);
            Assert.Equal(tenantDto.Name, result.Payload.Name);
            Assert.Equal(tenantDto.Hostnames, result.Payload.Hostnames);
        }

        [Fact]
        public async Task Creating_tenant_with_invalid_parameters_should_fail_gracefully()
        {
            var tenantDto = new TenantDto();

            var result = await tenantManagementApi.AddTenant(tenantDto);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task Creating_tenant_with_already_assigned_host_should_fail_gracefully()
        {
            var tenantDto1 = GetUniqueTestTenant;
            var tenantDto2 = GetUniqueTestTenant;

            tenantDto2.Hostnames = tenantDto1.Hostnames;

            var resultCreated1 = await tenantManagementApi.AddTenant(tenantDto1);
            var resultCreated2 = await tenantManagementApi.AddTenant(tenantDto2);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var tenantDto = GetUniqueTestTenant;

            var resultCreated = await tenantManagementApi.AddTenant(tenantDto);
            var resultFetched = await tenantManagementApi.GetTenant(resultCreated.Payload.TenantId);

            var createdTenant = resultCreated.Payload;
            var fetchedTenant = resultFetched.Payload;

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(createdTenant, fetchedTenant);
        }

        [Fact]
        public async Task Fetching_missing_ID_should_fail_gracefully()
        {
            var resultFetched = await tenantManagementApi.GetTenant(Guid.NewGuid());

            Assert.False(resultFetched.Success);
        }

        [Fact]
        public async Task Service_should_return_all_created_tenants()
        {
            var result1 = await tenantManagementApi.AddTenant(GetUniqueTestTenant);
            var result2 = await tenantManagementApi.AddTenant(GetUniqueTestTenant);

            var tenants = await tenantManagementApi.GetTenants();
            var count = tenants
                .Where(x => x.TenantId == result1.Payload.TenantId || x.TenantId == result2.Payload.TenantId)
                .Count();

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Service_should_find_tenant_by_hostname()
        {
            var resultCreated = await tenantManagementApi.AddTenant(GetUniqueTestTenant);
            var resultFetched = await tenantManagementApi.GetTenantByHostname(resultCreated.Payload.Hostnames[0]);

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(resultCreated.Payload.TenantId, resultFetched.Payload.TenantId);
        }

        [Fact]
        public async Task Fetching_nonexisting_hostname_should_fail_gracefully()
        {
            var resultFetched = await tenantManagementApi.GetTenantByHostname("host-which-does-not-exist");

            Assert.False(resultFetched.Success);
        }

        [Fact]
        public async Task Updated_dto_should_match_original_dto()
        {
            var tenantCreatedResult = await tenantManagementApi.AddTenant(GetUniqueTestTenant);
            var tenant = tenantCreatedResult.Payload;

            tenant.Name += " UPDATED";
            tenant.Hostnames.Add("new-hostname-for-tenant");

            var tenantUpdatedResult = await tenantManagementApi.UpdateTenant(tenant);

            Assert.True(tenantCreatedResult.Success);
            Assert.True(tenantUpdatedResult.Success);
            Assert.Equal(tenant, tenantUpdatedResult.Payload);
        }

        [Fact]
        public async Task Adding_already_assigned_hostname_should_fail_gracefully()
        {
            var tenantCreatedResult1 = await tenantManagementApi.AddTenant(GetUniqueTestTenant);
            var tenantCreatedResult2 = await tenantManagementApi.AddTenant(GetUniqueTestTenant);

            var tenant = tenantCreatedResult1.Payload;
            tenant.Hostnames.AddRange(tenantCreatedResult2.Payload.Hostnames);

            var tenantUpdatedResult = await tenantManagementApi.UpdateTenant(tenant);

            Assert.True(tenantCreatedResult1.Success);
            Assert.True(tenantCreatedResult2.Success);
            Assert.False(tenantUpdatedResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_tenant_should_fail_gracefully()
        {
            var deleteResult = await tenantManagementApi.DeleteTenant(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Deleting_tenant_should_work()
        {
            var createdResult = await tenantManagementApi.AddTenant(GetUniqueTestTenant);
            var deleteResult = await tenantManagementApi.DeleteTenant(createdResult.Payload.TenantId);

            Assert.True(createdResult.Success);
            Assert.True(deleteResult.Success);
        }
    }
}