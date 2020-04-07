using IdentityUtils.Api.Extensions;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    public class RolesApiTests : TestAbstract
    {
        private readonly RoleManagementApi<RoleDto> roleManagementApi;

        public RolesApiTests() : base()
        {
            roleManagementApi = serviceProvider.GetRequiredService<RoleManagementApi<RoleDto>>();
        }

        private RoleDto GetUniqueTestRole => new RoleDto
        {
            Name = "ROLE1" + Guid.NewGuid().ToString()
        };

        [Fact]
        public async Task Created_dto_should_match_original_dto()
        {
            var roleDto = GetUniqueTestRole;
            var resultCreated = await roleManagementApi.AddRole(roleDto);

            Assert.True(resultCreated.Success);
            Assert.Equal(roleDto.Name, resultCreated.Data.Name);
        }

        [Fact]
        public async Task Created_dto_should_match_fetched_dto()
        {
            var resultCreated = await roleManagementApi.AddRole(GetUniqueTestRole);
            var resultFetch = await roleManagementApi.GetRoleById(resultCreated.Data.Id);

            Assert.True(resultCreated.Success);
            Assert.True(resultFetch.Success);
            Assert.Equal(resultCreated.Data, resultFetch.Data);
        }

        [Fact]
        public async Task Service_should_return_all_roles()
        {
            var resultCreated1 = await roleManagementApi.AddRole(GetUniqueTestRole);
            var resultCreated2 = await roleManagementApi.AddRole(GetUniqueTestRole);

            var roles = await roleManagementApi.GetRoles();
            var count = roles.Data.Where(x => x.Id == resultCreated1.Data.Id || x.Id == resultCreated2.Data.Id).Count();

            Assert.True(resultCreated1.Success);
            Assert.True(resultCreated2.Success);
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task Role_name_should_be_unique()
        {
            var roleDto = GetUniqueTestRole;

            var resultCreated1 = await roleManagementApi.AddRole(roleDto);
            var resultCreated2 = await roleManagementApi.AddRole(roleDto);

            Assert.True(resultCreated1.Success);
            Assert.False(resultCreated2.Success);
        }

        [Fact]
        public async Task Role_should_be_fetched_by_normalized_name()
        {
            var role = new RoleDto { Name = "strange Name" };
            var resultCreated = await roleManagementApi.AddRole(role);
            var resultFetched = await roleManagementApi.GetRoleByNormalizedName(resultCreated.Data.NormalizedName);

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(resultCreated.Data, resultFetched.Data);
        }

        [Fact]
        public async Task Existing_role_should_be_deleted()
        {
            var resultCreated = await roleManagementApi.AddRole(GetUniqueTestRole);
            var deleteResult = await roleManagementApi.DeleteRole(resultCreated.Data.Id);

            Assert.True(resultCreated.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_role_should_fail_gracefully()
        {
            var deleteResult = await roleManagementApi.DeleteRole(Guid.NewGuid());
            Assert.False(deleteResult.Success);
        }
    }
}