using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using System;
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
            Hostnames = new string[] { $"host1{GetUniqueId()}", $"host2{GetUniqueId()}" }
        };

        [Fact]
        public async Task Adding_already_assigned_hostname_should_fail_gracefully()
        {
            var (resultCreated1, tenant1) = await tenantsService.AddTenant(TestTenant).UnpackSingleOrDefault();
            var (resultCreated2, tenant2) = await tenantsService.AddTenant(TestTenant).UnpackSingleOrDefault();

            foreach (var host in tenant2.Hostnames)
                tenant1.Hostnames.Add(host);

            var tenantUpdatedResult = await tenantsService.UpdateTenant(tenant1);

            Assert.True(resultCreated1.Success);
            Assert.True(resultCreated2.Success);
            Assert.False(tenantUpdatedResult.Success);
        }

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var tenantDto = TestTenant;

            var (resultCreated, tenantCreated) = await tenantsService.AddTenant(tenantDto).UnpackSingleOrDefault();
            var (resultFetched, tenantFetched) = await tenantsService.GetTenant(tenantCreated.TenantId).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(tenantCreated, tenantFetched);
        }

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var tenantDto = TestTenant;

            var (resultCreated, tenantCreated) = await tenantsService.AddTenant(tenantDto).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.Equal(tenantDto.Name, tenantCreated.Name);
            Assert.Equal(tenantDto.Hostnames, tenantCreated.Hostnames);
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
            var (resultCreated, tenantCreated) = await tenantsService.AddTenant(TestTenant).UnpackSingleOrDefault();
            var resultDeleted = await tenantsService.DeleteTenant(tenantCreated.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(resultDeleted.Success);
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
            var (resultCreated, tenantCreated) = await tenantsService.AddTenant(tenant).UnpackSingleOrDefault();
            var resultFetched = await tenantsService.Search(new TenantSearch(tenant.Hostnames.First()));

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Any());
            Assert.Equal(tenantCreated.TenantId, resultFetched.First().TenantId);
        }

        [Fact]
        public async Task Service_should_return_all_created_tenants()
        {
            await tenantsService.AddTenant(TestTenant);
            await tenantsService.AddTenant(TestTenant);

            var tenants = await tenantsService.GetTenants();
            Assert.Equal(2, tenants.Count());
        }

        [Fact]
        public async Task Updated_dto_should_match_original_dto()
        {
            var (resultCreated, tenantCreated) = await tenantsService.AddTenant(TestTenant).UnpackSingleOrDefault();

            tenantCreated.Name += " UPDATED";
            tenantCreated.Hostnames.Add("new-hostname-for-tenant");

            var (resultUpdated, tenantUpdated) = await tenantsService.UpdateTenant(tenantCreated).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultUpdated.Success);
            Assert.Equal(tenantCreated, tenantUpdated);
        }
    }
}