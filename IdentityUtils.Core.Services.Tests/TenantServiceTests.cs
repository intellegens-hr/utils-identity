using IdentityUtils.Core.Services.Tests.Setup;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Core.Services.Tests
{
    public class TenantServiceTests
    {
        private TenantDto TestTenant1 => new TenantDto
        {
            Name = "Test tenant",
            Hostnames = new List<string> { "host1", "host2" }
        };

        private TenantDto TestTenant2 => new TenantDto
        {
            Name = "Another test tenant",
            Hostnames = new List<string> { "host3", "host4" }
        };

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var tenantDto = TestTenant1;

            var result = await serviceTenants.Service.AddTenant(tenantDto);

            Assert.True(result.Success);
            Assert.Equal(tenantDto.Name, result.Payload.Name);
            Assert.Equal(tenantDto.Hostnames, result.Payload.Hostnames);
        }

        [Fact]
        public async Task Creating_tenant_with_invalid_parameters_should_fail_gracefully()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var tenantDto = new TenantDto();

            var result = await serviceTenants.Service.AddTenant(tenantDto);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task Creating_tenant_with_already_assigned_host_should_fail_gracefully()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var tenantDto1 = TestTenant1;
            var tenantDto2 = TestTenant2;

            tenantDto2.Hostnames = tenantDto1.Hostnames;

            var resultCreated1 = await serviceTenants.Service.AddTenant(tenantDto1);
            var resultCreated2 = await serviceTenants.Service.AddTenant(tenantDto2);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var tenantDto = TestTenant1;

            var resultCreated = await serviceTenants.Service.AddTenant(tenantDto);
            var resultFetched = await serviceTenants.Service.GetTenant(resultCreated.Payload.TenantId);

            var createdTenant = resultCreated.Payload;
            var fetchedTenant = resultFetched.Payload;

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(createdTenant, fetchedTenant);
        }

        [Fact]
        public async Task Fetching_missing_ID_should_fail_gracefully()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var resultFetched = await serviceTenants.Service.GetTenant(Guid.NewGuid());

            Assert.False(resultFetched.Success);
        }

        [Fact]
        public async Task Service_should_return_all_created_tenants()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var resultCreated1 = await serviceTenants.Service.AddTenant(TestTenant1);
            var resultCreated2 = await serviceTenants.Service.AddTenant(TestTenant2);

            var tenants = await serviceTenants.Service.GetTenants();
            Assert.Equal(2, tenants.Count);
        }

        [Fact]
        public async Task Service_should_find_tenant_by_hostname()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();
            var resultCreated = await serviceTenants.Service.AddTenant(TestTenant1);

            var resultFetched = await serviceTenants.Service.GetTenantByHostname("host1");

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(resultCreated.Payload.TenantId, resultFetched.Payload.TenantId);
        }

        [Fact]
        public async Task Fetching_nonexisting_hostname_should_fail_gracefully()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();
            
            var resultFetched = await serviceTenants.Service.GetTenantByHostname("host-which-does-not-exist");

            Assert.False(resultFetched.Success);
        }

        [Fact]
        public async Task Updated_dto_should_match_original_dto()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var tenantCreatedResult = await serviceTenants.Service.AddTenant(TestTenant1);
            var tenant = tenantCreatedResult.Payload;

            tenant.Name += " UPDATED";
            tenant.Hostnames.Add("new-hostname-for-tenant");

            var tenantUpdatedResult = await serviceTenants.Service.UpdateTenant(tenant);

            Assert.True(tenantCreatedResult.Success);
            Assert.True(tenantUpdatedResult.Success);
            Assert.Equal(tenant, tenantUpdatedResult.Payload);
        }

        [Fact]
        public async Task Adding_already_assigned_hostname_should_fail_gracefully()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var tenantCreatedResult1 = await serviceTenants.Service.AddTenant(TestTenant1);
            var tenantCreatedResult2 = await serviceTenants.Service.AddTenant(TestTenant2);

            var tenant = tenantCreatedResult1.Payload;
            tenant.Hostnames.AddRange(tenantCreatedResult2.Payload.Hostnames);

            var tenantUpdatedResult = await serviceTenants.Service.UpdateTenant(tenant);

            Assert.True(tenantCreatedResult1.Success);
            Assert.True(tenantCreatedResult2.Success);
            Assert.False(tenantUpdatedResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_tenant_should_fail_gracefully()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var deleteResult = await serviceTenants.Service.DeleteTenant(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Deleting_tenant_should_work()
        {
            using var serviceTenants = ServicesFactory.GetTenantService();

            var createdResult = await serviceTenants.Service.AddTenant(TestTenant1);
            var deleteResult = await serviceTenants.Service.DeleteTenant(createdResult.Payload.TenantId);

            Assert.True(createdResult.Success);
            Assert.True(deleteResult.Success);
        }
    }
}