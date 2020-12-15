using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Core.Services.Tests
{
    public class RolesServiceTests : TestAbstractMultiTenant
    {
        private readonly RolesService rolesService;

        public RolesServiceTests() : base()
        {
            rolesService = GetService<RolesService>();
        }

        private RoleDto TestRole => new RoleDto
        {
            Name = $"ROLE{GetUniqueId()}"
        };

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var resultCreated = await rolesService.AddRole(TestRole);
            var resultFetch = await rolesService.GetRole(resultCreated.Data.Id);

            Assert.True(resultCreated.Success);
            Assert.True(resultFetch.Success);
            Assert.Equal(resultCreated.Data, resultFetch.Data);
        }

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var testRole = TestRole;
            var resultCreated = await rolesService.AddRole(testRole);

            Assert.True(resultCreated.Success);
            Assert.Equal(testRole.Name, resultCreated.Data.Name);
        }

        [Fact]
        public async Task Deleting_nonexisting_role_should_fail_gracefully()
        {
            var deleteResult = await rolesService.DeleteRole(Guid.NewGuid());
            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Existing_role_should_be_deleted()
        {
            var resultCreated = await rolesService.AddRole(TestRole);
            var deleteResult = await rolesService.DeleteRole(resultCreated.Data.Id);

            Assert.True(resultCreated.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task Role_name_should_be_unique()
        {
            var testRole1 = TestRole;
            var testRole2 = TestRole;

            testRole2.Name = testRole1.Name;

            var resultCreated1 = await rolesService.AddRole(testRole1);
            var resultCreated2 = await rolesService.AddRole(testRole2);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Role_should_be_fetched_by_normalized_name()
        {
            var role = new RoleDto { Name = $"strange Name{Guid.NewGuid()}" };
            var resultCreated = await rolesService.AddRole(role);
            var resultFetched = await rolesService.Search(new RoleSearch(resultCreated.Data.Name));

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Any());
            Assert.Equal(resultCreated.Data, resultFetched.First());
        }

        [Fact]
        public async Task Role_with_invalid_data_shoulndt_be_saved()
        {
            var role = TestRole;
            role.Name = $"{new string('X', 100)}";

            var resultCreated = await rolesService.AddRole(role);

            Assert.False(resultCreated.Success);
        }

        [Fact]
        public async Task Service_should_return_all_roles()
        {
            var resultCreated1 = await rolesService.AddRole(TestRole);
            var resultCreated2 = await rolesService.AddRole(TestRole);

            var roles = await rolesService.GetAllRoles();
            var roleIds = roles.Select(x => x.Id);

            Assert.True(resultCreated1.Success);
            Assert.True(resultCreated2.Success);
            Assert.Contains(resultCreated1.Data.Id, roleIds);
            Assert.Contains(resultCreated2.Data.Id, roleIds);
        }
    }
}