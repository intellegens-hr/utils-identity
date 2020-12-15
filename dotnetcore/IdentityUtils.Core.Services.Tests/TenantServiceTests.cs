using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Core.Services.Tests
{
    public class TenantServiceTests : TestAbstractMultiTenant
    {
        private readonly TenantsService tenantsService;

        public TenantServiceTests() : base()
        {
            tenantsService = GetService<TenantsService>();
        }

        private TenantDto TestTenant => new TenantDto
        {
            Name = $"Test tenant{GetUniqueId()}",
            Hostnames = new List<string> { $"host1{GetUniqueId()}", $"host2{GetUniqueId()}" }
        };

        [Fact]
        public async Task Adding_already_assigned_hostname_should_fail_gracefully()
        {
            var tenantCreatedResult1 = await tenantsService.AddTenant(TestTenant);
            var tenantCreatedResult2 = await tenantsService.AddTenant(TestTenant);

            var tenant = tenantCreatedResult1.Data;
            tenant.Hostnames.AddRange(tenantCreatedResult2.Data.Hostnames);

            var tenantUpdatedResult = await tenantsService.UpdateTenant(tenant);

            Assert.True(tenantCreatedResult1.Success);
            Assert.True(tenantCreatedResult2.Success);
            Assert.False(tenantUpdatedResult.Success);
        }

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var tenantDto = TestTenant;

            var resultCreated = await tenantsService.AddTenant(tenantDto);
            var resultFetched = await tenantsService.GetTenant(resultCreated.Data.TenantId);

            var createdTenant = resultCreated.Data;
            var fetchedTenant = resultFetched.Data;

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(createdTenant, fetchedTenant);
        }

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var tenantDto = TestTenant;

            var result = await tenantsService.AddTenant(tenantDto);

            Assert.True(result.Success);
            Assert.Equal(tenantDto.Name, result.Data.Name);
            Assert.Equal(tenantDto.Hostnames, result.Data.Hostnames);
        }

        [Fact]
        public async Task Creating_tenant_with_already_assigned_host_should_fail_gracefully()
        {
            var tenantDto1 = TestTenant;
            var tenantDto2 = TestTenant;

            tenantDto2.Hostnames = tenantDto1.Hostnames;

            var resultCreated1 = await tenantsService.AddTenant(tenantDto1);
            var resultCreated2 = await tenantsService.AddTenant(tenantDto2);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Creating_tenant_with_invalid_parameters_should_fail_gracefully()
        {
            var tenantDto1 = new TenantDto();
            var result1 = await tenantsService.AddTenant(tenantDto1);

            var tenantDto2 = TestTenant;
            tenantDto2.Name = new string('X', 200);
            var result2 = await tenantsService.AddTenant(tenantDto2);

            Assert.False(result1.Success);
            Assert.False(result2.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_tenant_should_fail_gracefully()
        {
            var deleteResult = await tenantsService.DeleteTenant(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Deleting_tenant_should_work()
        {
            var createdResult = await tenantsService.AddTenant(TestTenant);
            var deleteResult = await tenantsService.DeleteTenant(createdResult.Data.TenantId);

            Assert.True(createdResult.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task Fetching_missing_ID_should_fail_gracefully()
        {
            var resultFetched = await tenantsService.GetTenant(Guid.NewGuid());

            Assert.False(resultFetched.Success);
        }

        [Fact]
        public async Task Fetching_nonexisting_hostname_should_fail_gracefully()
        {
            var resultFetched = await tenantsService.Search(new TenantSearch("host-which-does-not-exist"));

            Assert.False(resultFetched.Any());
        }

        [Fact]
        public async Task Service_should_find_tenant_by_hostname()
        {
            var tenant = TestTenant;
            var resultCreated = await tenantsService.AddTenant(tenant);
            var resultFetched = await tenantsService.Search(new TenantSearch(tenant.Hostnames[0]));

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Any());
            Assert.Equal(resultCreated.Data.TenantId, resultFetched.First().TenantId);
        }

        [Fact]
        public async Task Service_should_return_all_created_tenants()
        {
            await tenantsService.AddTenant(TestTenant);
            await tenantsService.AddTenant(TestTenant);

            var tenants = await tenantsService.GetTenants();
            Assert.Equal(2, tenants.Count);
        }

        [Fact]
        public async Task Updated_dto_should_match_original_dto()
        {
            var tenantCreatedResult = await tenantsService.AddTenant(TestTenant);
            var tenant = tenantCreatedResult.Data;

            tenant.Name += " UPDATED";
            tenant.Hostnames.Add("new-hostname-for-tenant");

            var tenantUpdatedResult = await tenantsService.UpdateTenant(tenant);

            Assert.True(tenantCreatedResult.Success);
            Assert.True(tenantUpdatedResult.Success);
            Assert.Equal(tenant, tenantUpdatedResult.Data);
        }
    }
}