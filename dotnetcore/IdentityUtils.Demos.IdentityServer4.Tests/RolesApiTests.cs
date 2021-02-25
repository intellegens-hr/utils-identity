using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Demos.IdentityServer4.MultiTenant;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using IdentityUtils.Demos.IdentityServer4.Tests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    [Collection(nameof(RolesApiTests))]
    public class RolesApiTests : TestAbstract<TestMultiTenantStartup, Startup, MultitenantWebApplicationFactory>
    {
        public RolesApiTests(MultitenantWebApplicationFactory factory) : base(factory)
        {
        }

        private RoleDto GetUniqueTestRole => new RoleDto
        {
            Name = "ROLE1" + Guid.NewGuid().ToString()
        };

        protected override string DatabaseName => $"IntegrationTestDatabase_{nameof(RolesApiTests)}.db";

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var (resultCreated, roleCreated) = await RoleManagementApi.AddRole(GetUniqueTestRole).UnpackSingleOrDefault();
            var (resultFetched, roleFetched) = await RoleManagementApi.GetRoleById(roleCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(roleCreated, roleFetched);
        }

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var roleDto = GetUniqueTestRole;
            var (resultCreated, roleCreated) = await RoleManagementApi.AddRole(roleDto).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.Equal(roleDto.Name, roleCreated.Name);
        }

        [Fact]
        public async Task Deleting_nonexisting_role_should_fail_gracefully()
        {
            var deleteResult = await RoleManagementApi.DeleteRole(Guid.NewGuid());
            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Existing_role_should_be_deleted()
        {
            var (resultCreated, roleCreated) = await RoleManagementApi.AddRole(GetUniqueTestRole).UnpackSingleOrDefault();
            var deleteResult = await RoleManagementApi.DeleteRole(roleCreated.Id);

            Assert.True(resultCreated.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task Role_name_should_be_unique()
        {
            var roleDto = GetUniqueTestRole;

            var resultCreated1 = await RoleManagementApi.AddRole(roleDto);
            var resultCreated2 = await RoleManagementApi.AddRole(roleDto);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Role_should_be_fetched_by_normalized_name()
        {
            var role = new RoleDto { Name = $"strange Name{Guid.NewGuid()}" };
            var (resultCreated, roleCreated) = await RoleManagementApi.AddRole(role).UnpackSingleOrDefault();
            var resultSearch = await RoleManagementApi.Search(new RoleSearch(name: roleCreated.NormalizedName));

            Assert.True(resultCreated.Success);
            Assert.True(resultSearch.Success);
            Assert.Contains(roleCreated, resultSearch.Data);
        }

        [Fact]
        public async Task Service_should_return_all_roles()
        {
            var (resultCreated1, roleCreated1) = await RoleManagementApi.AddRole(GetUniqueTestRole).UnpackSingleOrDefault();
            var (resultCreated2, roleCreated2) = await RoleManagementApi.AddRole(GetUniqueTestRole).UnpackSingleOrDefault();

            var roles = await RoleManagementApi.GetRoles();
            var count = roles.Data.Where(x => x.Id == roleCreated1.Id || x.Id == roleCreated2.Id).Count();

            Assert.True(resultCreated1.Success);
            Assert.True(resultCreated2.Success);
            Assert.Equal(2, count);
        }
    }
}