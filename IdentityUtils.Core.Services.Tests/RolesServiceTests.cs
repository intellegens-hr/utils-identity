using IdentityUtils.Core.Services.Tests.Setup;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using System;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Core.Services.Tests
{
    public class RolesServiceTests : IDisposable
    {
        private readonly DisposableContextService serviceProviderDisposable = new DisposableContextService();

        private readonly RolesService rolesService;

        public RolesServiceTests()
        {
            rolesService = serviceProviderDisposable.GetService<RolesService>();
        }

        private RoleDto TestRole1 => new RoleDto
        {
            Name = "ROLE1"
        };

        private RoleDto TestRole2 => new RoleDto
        {
            Name = "ROLE2"
        };

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var resultCreated = await rolesService.AddRole(TestRole1);

            Assert.True(resultCreated.Success);
            Assert.Equal(TestRole1.Name, resultCreated.Payload.Name);
        }

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var resultCreated = await rolesService.AddRole(TestRole1);
            var resultFetch = await rolesService.GetRole(resultCreated.Payload.Id);

            Assert.True(resultCreated.Success);
            Assert.True(resultFetch.Success);
            Assert.Equal(resultCreated.Payload, resultFetch.Payload);
        }

        [Fact]
        public async Task Service_should_return_all_roles()
        {
            var resultCreated1 = await rolesService.AddRole(TestRole1);
            var resultCreated2 = await rolesService.AddRole(TestRole2);

            var roles = await rolesService.GetAllRoles();

            Assert.True(resultCreated1.Success);
            Assert.True(resultCreated2.Success);
            Assert.Equal(2, roles.Count);
        }

        [Fact]
        public async Task Role_name_should_be_unique()
        {
            var resultCreated1 = await rolesService.AddRole(TestRole1);
            var resultCreated2 = await rolesService.AddRole(TestRole1);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Role_should_be_fetched_by_normalized_name()
        {
            var role = new RoleDto { Name = "strange Name" };
            var resultCreated = await rolesService.AddRole(role);
            var resultFetched = await rolesService.GetRole(resultCreated.Payload.NormalizedName);

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(resultCreated.Payload, resultFetched.Payload);
        }

        [Fact]
        public async Task Existing_role_should_be_deleted()
        {
            var resultCreated = await rolesService.AddRole(TestRole1);
            var deleteResult = await rolesService.DeleteRole(resultCreated.Payload.Id);

            Assert.True(resultCreated.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_role_should_fail_gracefully()
        {
            var deleteResult = await rolesService.DeleteRole(Guid.NewGuid());
            Assert.False(deleteResult.Success);
        }

        public void Dispose()
        {
            serviceProviderDisposable.Dispose();
        }
    }
}