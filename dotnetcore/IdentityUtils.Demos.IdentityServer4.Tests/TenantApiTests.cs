using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Demos.IdentityServer4.MultiTenant;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using IdentityUtils.Demos.IdentityServer4.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    [Collection(nameof(TenantApiTests))]
    public class TenantApiTests : TestAbstract<TestMultiTenantStartup, Startup, MultitenantWebApplicationFactory>
    {
        public TenantApiTests(MultitenantWebApplicationFactory factory) : base(factory)
        {
        }

        protected override string DatabaseName => $"IntegrationTestDatabase_{nameof(TenantApiTests)}.db";

        private TenantDto UniqueTestTenant => new TenantDto
        {
            Name = "Test tenant - " + Guid.NewGuid().ToString(),
            Hostnames = new List<string> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() }
        };

        [Fact]
        public async Task Adding_already_assigned_hostname_should_fail_gracefully()
        {
            var (createdResult1, tenant1) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();
            var (createdResult2, tenant2) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();

            foreach (var host in tenant2.Hostnames)
                tenant1.Hostnames.Add(host);

            var tenantUpdatedResult = await TenantManagementApi.UpdateTenant(tenant1);

            Assert.True(createdResult1.Success);
            Assert.True(createdResult2.Success);
            Assert.False(tenantUpdatedResult.Success);
        }

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var (resultCreated, tenantCreated) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();
            var (resultFetched, tenantFetched) = await TenantManagementApi.GetTenant(tenantCreated.TenantId).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(tenantCreated, tenantFetched);
        }

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var tenant = UniqueTestTenant;
            var (resultCreated, tenantCreated) = await TenantManagementApi.AddTenant(tenant).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.Equal(tenant.Name, tenantCreated.Name);
            Assert.Equal(tenant.Hostnames, tenantCreated.Hostnames);
        }

        [Fact]
        public async Task Creating_tenant_with_already_assigned_host_should_fail_gracefully()
        {
            var tenantDto1 = UniqueTestTenant;
            var tenantDto2 = UniqueTestTenant;

            tenantDto2.Hostnames = tenantDto1.Hostnames;

            var resultCreated1 = await TenantManagementApi.AddTenant(tenantDto1);
            var resultCreated2 = await TenantManagementApi.AddTenant(tenantDto2);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Creating_tenant_with_invalid_parameters_should_fail_gracefully()
        {
            var resultCreated = await TenantManagementApi.AddTenant(new TenantDto());

            Assert.False(resultCreated.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_tenant_should_fail_gracefully()
        {
            var resultDeleted = await TenantManagementApi.DeleteTenant(Guid.NewGuid());

            Assert.False(resultDeleted.Success);
        }

        [Fact]
        public async Task Deleting_tenant_should_work()
        {
            var (resultCreated, tenantCreated) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();
            var resultDeleted = await TenantManagementApi.DeleteTenant(tenantCreated.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(resultDeleted.Success);
        }

        [Fact]
        public async Task Fetching_missing_ID_should_fail_gracefully()
        {
            var resultFetched = await TenantManagementApi.GetTenant(Guid.NewGuid());

            Assert.False(resultFetched.Success);
        }

        [Fact]
        public async Task Fetching_nonexisting_hostname_should_fail_gracefully()
        {
            var resultFetched = await TenantManagementApi.Search(new TenantSearch(hostname: "host-which-does-not-exist"));

            Assert.True(resultFetched.Success);
            Assert.False(resultFetched.Data.Any());
        }

        [Fact]
        public async Task Service_should_find_tenant_by_hostname()
        {
            var (resultCreated, tenantCreated) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();
            var (resultFetched, tenantFetched) = await TenantManagementApi.Search(new TenantSearch(hostname: tenantCreated.Hostnames.First())).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(tenantCreated.TenantId, tenantFetched.TenantId);
        }

        [Fact]
        public async Task Service_should_return_all_created_tenants()
        {
            var (_, tenantCreated1) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();
            var (_, tenantCreated2) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();

            var tenants = await TenantManagementApi.GetTenants();
            var count = tenants
                .Data
                .Where(x => x.TenantId == tenantCreated1.TenantId || x.TenantId == tenantCreated2.TenantId)
                .Count();

            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Updated_dto_should_match_original_dto()
        {
            var (resultCreated, tenantCreated) = await TenantManagementApi.AddTenant(UniqueTestTenant).UnpackSingleOrDefault();

            tenantCreated.Name += " UPDATED";
            tenantCreated.Hostnames.Add($"new-hostname-for-tenant-{Guid.NewGuid()}");

            var (resultUpdated, tenantUpdated) = await TenantManagementApi.UpdateTenant(tenantCreated).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultUpdated.Success);
            Assert.Equal(tenantCreated, tenantUpdated);
        }
    }
}