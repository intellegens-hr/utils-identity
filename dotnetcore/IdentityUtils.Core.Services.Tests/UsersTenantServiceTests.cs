using IdentityUtils.Core.Contracts.Claims;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using IdentityUtils.Core.Services.Tests.TestData;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Core.Services.Tests
{
    public class UsersTenantServiceTests : TestAbstractMultiTenant
    {
        private readonly RolesService rolesService;
        private readonly TenantsService tenantsService;
        private readonly UsersTenantService usersTenantService;

        public UsersTenantServiceTests() : base()
        {
            usersTenantService = GetService<UsersTenantService>();
            rolesService = GetService<RolesService>();
            tenantsService = GetService<TenantsService>();
        }

        private RoleDto Role1 { get; set; }

        private RoleDto Role2 { get; set; }

        private TenantDto Tenant1 { get; set; }

        private TenantDto Tenant2 { get; set; }

        private UserDto TestUser => new UserDto
        {
            Username = $"testuser1{GetUniqueId()}@email.com",
            Password = "Wery5trong!Pa55word",
            Email = "testuser1{GetUniqueId()}@email.com"
        };

        [Fact]
        public async Task Adding_user_claims_should_work()
        {
            var (_, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            var claimsToAdd = new Claim[] {
                new Claim("testclaim2", "testvalue"),
                new Claim("testclaim3", "testvalue")
            };

            var resultAdding1 = await usersTenantService.AddClaimAsync(userCreated.Id, new Claim("testclaim1", "testvalue"));
            var resultAdding2 = await usersTenantService.AddClaimsAsync(userCreated.Id, claimsToAdd);

            Assert.True(resultAdding1.Success);
            Assert.True(resultAdding2.Success);
        }

        [Fact]
        public async Task Adding_user_to_multiple_roles_should_work()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            await LoadTenantsAndRoles();

            var rolesToAdd = new Guid[] { Role1.Id, Role2.Id };
            var roleAddResult = await usersTenantService.AddToRolesAsync(userCreated.Id, Tenant1.TenantId, rolesToAdd);

            var userRoles = await usersTenantService.GetRolesAsync(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.Equal(2, userRoles.Count());
        }

        [Fact]
        public async Task Adding_user_to_role_multiple_times_shouldnt_do_anything()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var role1AddResult = await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var role2AddResult = await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await usersTenantService.GetRolesAsync(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(role1AddResult.Success);
            Assert.True(role2AddResult.Success);
            Assert.Single(userRoles);
        }

        [Fact]
        public async Task Checking_if_user_is_in_role_should_work()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleAddResult = await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var isUserInRole = await usersTenantService.IsInRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(isUserInRole);
        }

        [Fact]
        public async Task Checking_if_user_is_in_unassigend_role_should_work()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleAddResult = await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var isUserInRole = await usersTenantService.IsInRoleAsync(userCreated.Id, Tenant1.TenantId, Guid.NewGuid());

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.False(isUserInRole);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_id_dto()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (resultFetched, userFetched) = await usersTenantService.FindByIdAsync<UserDto>(userCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated, userFetched);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_username_dto()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (resultFetched, userFetched) = await usersTenantService.FindByNameAsync<UserDto>(userCreated.Username).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated, userFetched);
        }

        [Fact]
        public async Task Created_user_dto_should_match_original_dto()
        {
            var dto = TestUser;
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(dto).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.Equal(dto.Email, userCreated.Email);
            Assert.Equal(dto.Username, userCreated.Username);
            Assert.Equal(dto.DisplayName, userCreated.DisplayName);
            Assert.Equal(dto.AdditionalDataJson, userCreated.AdditionalDataJson);
        }

        [Theory]
        [ClassData(typeof(InvalidUserData))]
        public async Task Creating_user_with_invalid_data_or_password_should_fail(UserDto userDto)
        {
            var createdResult = await usersTenantService.CreateUser(userDto);
            Assert.False(createdResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_user_should_fail_gracefully()
        {
            var deleteResult = await usersTenantService.DeleteUser(Guid.NewGuid());
            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Fetching_user_tenant_claim_data_should_work()
        {
            var (_, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            await LoadTenantsAndRoles();

            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant2.TenantId, Role1.Id);
            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant2.TenantId, Role2.Id);

            var userTenantClaims = await usersTenantService.GetUserTenantRolesClaims(userCreated.Id);

            Assert.Equal(2, userTenantClaims.Count());
        }

        [Fact]
        public async Task Fetching_user_tenant_claim_data_should_work_and_be_parsed()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            await LoadTenantsAndRoles();

            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant2.TenantId, Role1.Id);
            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant2.TenantId, Role2.Id);

            var userTenantClaims = await usersTenantService.GetUserTenantRolesClaims(userCreated.Id);

            var userTenantClaimsParsed = userTenantClaims.Select(x => x.Value.DeserializeToTenantRolesClaimData());

            var tenant1RolesCount = userTenantClaimsParsed.First(x => x.TenantId == Tenant1.TenantId).Roles.Count();
            var tenant2RolesCount = userTenantClaimsParsed.First(x => x.TenantId == Tenant2.TenantId).Roles.Count();

            Assert.Equal(1, tenant1RolesCount);
            Assert.Equal(2, tenant2RolesCount);
        }

        [Fact]
        public async Task Fetching_users_per_role_should_filter_correctly()
        {
            await LoadTenantsAndRoles();
            var (_, userCreated1) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (_, userCreated2) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await usersTenantService.AddToRoleAsync(userCreated1.Id, Tenant1.TenantId, Role1.Id);
            await usersTenantService.AddToRoleAsync(userCreated1.Id, Tenant2.TenantId, Role2.Id);
            await usersTenantService.AddToRoleAsync(userCreated2.Id, Tenant2.TenantId, Role2.Id);

            var usersInRoleResult1 = await usersTenantService.Search(new UsersTenantSearch(Tenant1.TenantId, Role1.Id));
            var usersInRoleResult2 = await usersTenantService.Search(new UsersTenantSearch(Tenant2.TenantId, Role2.Id));

            Assert.Single(usersInRoleResult1);
            Assert.Equal(2, usersInRoleResult2.Count());
        }

        [Fact]
        public async Task Getting_all_users_should_work()
        {
            var (resultCreated1, userCreated1) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (resultCreated2, userCreated2) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            var users = await usersTenantService.GetAllUsers();
            var userIds = users.Select(x => x.Id);

            Assert.True(resultCreated2.Success);
            Assert.True(resultCreated2.Success);
            Assert.Contains(userCreated1.Id, userIds);
            Assert.Contains(userCreated2.Id, userIds);
        }

        [Fact]
        public async Task Getting_user_by_multiple_params_should_work()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            var fetchResultDb1 = await usersTenantService.FindByIdAsync(userCreated.Id);
            var fetchResultDb2 = await usersTenantService.FindByNameAsync(userCreated.Username);

            var fetchResult1 = await usersTenantService.FindByIdAsync<UserDto>(userCreated.Id);
            var fetchResult2 = await usersTenantService.FindByNameAsync<UserDto>(userCreated.Username);

            Assert.True(resultCreated.Success);
            Assert.True(fetchResultDb1.Success);
            Assert.True(fetchResultDb2.Success);
            Assert.True(fetchResult1.Success);
            Assert.True(fetchResult2.Success);
        }

        [Fact]
        public async Task Managing_user_roles_should_work_properly_after_multiple_operations()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleRemoveResult = await usersTenantService.RemoveFromRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var userRoles = await usersTenantService.GetRolesAsync(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles);
        }

        [Fact]
        public async Task Removing_user_from_role_should_work()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleAddResult = await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var roleRemoveResult = await usersTenantService.RemoveFromRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await usersTenantService.GetRolesAsync(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles);
        }

        [Fact]
        public async Task Removing_user_from_unassigned_role_shouldnt_do_anything()
        {
            var (_, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();

            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role2.Id);
            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role2.Id);
            await usersTenantService.RemoveFromRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);
            await usersTenantService.RemoveFromRoleAsync(userCreated.Id, Tenant1.TenantId, Role2.Id);
            await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role2.Id);

            var userRoles = await usersTenantService.GetRolesAsync(userCreated.Id, Tenant1.TenantId);

            Assert.Single(userRoles);
        }

        [Fact]
        public async Task User_should_be_able_to_change_password()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            var tokenResult = await usersTenantService.GeneratePasswordResetTokenAsync(userCreated.Username);
            var changePasswordResult = await usersTenantService.ResetPasswordAsync(userCreated.Username, tokenResult.Data.First(), "NewStrong1324Pa55!");

            Assert.True(resultCreated.Success);
            Assert.True(tokenResult.Success);
            Assert.True(changePasswordResult.Success);
        }

        [Fact]
        public async Task User_should_be_assigned_to_role()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleAddResult = await usersTenantService.AddToRoleAsync(userCreated.Id, Tenant1.TenantId, Role1.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
        }

        [Fact]
        public async Task User_should_be_deleted()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();
            var deleteResult = await usersTenantService.DeleteUser(userCreated.Id);

            Assert.True(resultCreated.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_updated()
        {
            var (resultCreated, userCreated) = await usersTenantService.CreateUser(TestUser).UnpackSingleOrDefault();

            userCreated.Email = "changed@email.com";
            var updatedResult = await usersTenantService.UpdateUser(userCreated);

            var (resultFetched, dataFetched) = await usersTenantService.FindByIdAsync<UserDto>(userCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(updatedResult.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated.Email, dataFetched.Email);
        }

        private async Task LoadTenantsAndRoles()
        {
            Tenant1 = new TenantDto
            {
                Name = $"Tenant 1{GetUniqueId()}"
            };

            Tenant2 = new TenantDto
            {
                Name = $"Tenant 1{GetUniqueId()}"
            };

            Role1 = new RoleDto
            {
                Name = $"Role 1{GetUniqueId()}"
            };

            Role2 = new RoleDto
            {
                Name = $"Role 2{GetUniqueId()}"
            };

            var tenant1Result = await tenantsService.AddTenant(Tenant1);
            var tenant2Result = await tenantsService.AddTenant(Tenant2);

            var role1Result = await rolesService.AddRole(Role1);
            var role2Result = await rolesService.AddRole(Role2);

            Tenant1 = tenant1Result.Data.Single();
            Tenant2 = tenant2Result.Data.Single();

            Role1 = role1Result.Data.Single();
            Role2 = role2Result.Data.Single();
        }
    }
}