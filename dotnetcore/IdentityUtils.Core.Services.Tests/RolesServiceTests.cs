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
            var (resultCreated, roleCreated) = await rolesService.AddRole(TestRole).UnpackSingleOrDefault();
            var (resultFetch, roleFetched) = await rolesService.GetRole(roleCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetch.Success);
            Assert.Equal(roleCreated, roleFetched);
        }

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var testRole = TestRole;
            var (resultCreated, roleCreated) = await rolesService.AddRole(testRole).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.Equal(testRole.Name, roleCreated.Name);
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
            var (resultCreated, roleCreated) = await rolesService.AddRole(TestRole).UnpackSingleOrDefault();
            var deleteResult = await rolesService.DeleteRole(roleCreated.Id);

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
            var (resultCreated, roleCreated) = await rolesService.AddRole(role).UnpackSingleOrDefault();
            var resultFetched = await rolesService.Search(new RoleSearch(roleCreated.Name));

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Any());
            Assert.Equal(roleCreated, resultFetched.First());
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
            var (resultCreated1, roleCreated1) = await rolesService.AddRole(TestRole).UnpackSingleOrDefault();
            var (resultCreated2, roleCreated2) = await rolesService.AddRole(TestRole).UnpackSingleOrDefault();

            var roles = await rolesService.GetAllRoles();
            var roleIds = roles.Select(x => x.Id);

            Assert.True(resultCreated1.Success);
            Assert.True(resultCreated2.Success);
            Assert.Contains(roleCreated1.Id, roleIds);
            Assert.Contains(roleCreated2.Id, roleIds);
        }
    }
}