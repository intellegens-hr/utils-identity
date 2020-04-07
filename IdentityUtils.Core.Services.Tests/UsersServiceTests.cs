using IdentityUtils.Core.Contracts.Claims;
using IdentityUtils.Core.Services.Tests.Setup;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using IdentityUtils.Core.Services.Tests.Setup.ServicesTyped;
using IdentityUtils.Core.Services.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Core.Services.Tests
{
    public class UsersServiceTests : IDisposable
    {
        private readonly DisposableContextService serviceProviderDisposable = new DisposableContextService();
        private readonly UsersService usersService;
        private readonly RolesService rolesService;
        private readonly TenantsService tenantsService;

        public UsersServiceTests()
        {
            usersService = serviceProviderDisposable.GetService<UsersService>();
            rolesService = serviceProviderDisposable.GetService<RolesService>();
            tenantsService = serviceProviderDisposable.GetService<TenantsService>();
        }

        private async Task LoadTenantsAndRoles()
        {
            var tenant1Result = await tenantsService.AddTenant(Tenant1);
            var tenant2Result = await tenantsService.AddTenant(Tenant2);

            var role1Result = await rolesService.AddRole(Role1);
            var role2Result = await rolesService.AddRole(Role2);

            Tenant1 = tenant1Result.Data;
            Tenant2 = tenant2Result.Data;

            Role1 = role1Result.Data;
            Role2 = role2Result.Data;
        }

        private TenantDto Tenant1 { get; set; } = new TenantDto
        {
            Name = "Tenant 1"
        };

        private TenantDto Tenant2 { get; set; } = new TenantDto
        {
            Name = "Tenant 2"
        };

        private RoleDto Role1 { get; set; } = new RoleDto
        {
            Name = "Role 1"
        };

        private RoleDto Role2 { get; set; } = new RoleDto
        {
            Name = "Role 2"
        };

        private UserDto TestUser1 => new UserDto
        {
            Username = "testuser1@email.com",
            Password = "Wery5trong!Pa55word",
            Email = "testuser1@email.com"
        };

        private UserDto TestUser2 => new UserDto
        {
            Username = "testuser2@email.com",
            Password = "Wery5trong!Pa55word",
            Email = "testuser2@email.com"
        };

        [Fact]
        public async Task Getting_all_users_should_work()
        {
            var createdResult1 = await usersService.CreateUser(TestUser1);
            var createdResult2 = await usersService.CreateUser(TestUser2);

            var users = await usersService.GetAllUsers();

            Assert.True(createdResult1.Success);
            Assert.True(createdResult2.Success);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task Getting_user_by_multiple_params_should_work()
        {
            var createdResult = await usersService.CreateUser(TestUser1);

            var fetchResultDb1 = await usersService.FindByIdAsync(createdResult.Data.Id);
            var fetchResultDb2 = await usersService.FindByNameAsync(createdResult.Data.Username);

            var fetchResult1 = await usersService.FindByIdAsync<UserDto>(createdResult.Data.Id);
            var fetchResult2 = await usersService.FindByNameAsync<UserDto>(createdResult.Data.Username);

            Assert.True(createdResult.Success);
            Assert.True(fetchResultDb1.Success);
            Assert.True(fetchResultDb2.Success);
            Assert.True(fetchResult1.Success);
            Assert.True(fetchResult2.Success);
        }

        [Fact]
        public async Task Created_user_dto_should_match_original_dto()
        {
            var createdResult = await usersService.CreateUser(TestUser1);

            Assert.True(createdResult.Success);
            Assert.Equal(TestUser1.Email, createdResult.Data.Email);
            Assert.Equal(TestUser1.Username, createdResult.Data.Username);
            Assert.Equal(TestUser1.AdditionalDataJson, createdResult.Data.AdditionalDataJson);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_id_dto()
        {
            var createdResult = await usersService.CreateUser(TestUser1);
            var fetchResult = await usersService.FindByIdAsync<UserDto>(createdResult.Data.Id);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(createdResult.Data, fetchResult.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_username_dto()
        {
            var createdResult = await usersService.CreateUser(TestUser1);
            var fetchResult = await usersService.FindByNameAsync<UserDto>(createdResult.Data.Username);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(createdResult.Data, fetchResult.Data);
        }

        [Theory]
        [ClassData(typeof(InvalidUserData))]
        public async Task Creating_user_with_invalid_data_or_password_should_fail(UserDto userDto)
        {
            var createdResult = await usersService.CreateUser(userDto);
            Assert.False(createdResult.Success);
        }

        [Fact]
        public async Task User_should_be_updated()
        {
            var createdResult = await usersService.CreateUser(TestUser1);
            var userDto = createdResult.Data;

            userDto.Email = "changed@email.com";
            var updatedResult = await usersService.UpdateUser(userDto);

            var fetchResult = await usersService.FindByIdAsync<UserDto>(userDto.Id);

            Assert.True(createdResult.Success);
            Assert.True(updatedResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(userDto.Email, fetchResult.Data.Email);
        }

        [Fact]
        public async Task User_should_be_deleted()
        {
            var createdResult = await usersService.CreateUser(TestUser1);
            var deleteResult = await usersService.DeleteUser(createdResult.Data.Id);

            Assert.True(createdResult.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_able_to_change_password()
        {
            var createdResult = await usersService.CreateUser(TestUser1);
            var tokenResult = await usersService.GeneratePasswordResetTokenAsync(createdResult.Data.Username);
            var changePasswordResult = await usersService.ResetPasswordAsync(createdResult.Data.Username, tokenResult.Data, "NewStrong1324Pa55!");

            Assert.True(createdResult.Success);
            Assert.True(tokenResult.Success);
            Assert.True(changePasswordResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_user_should_fail_gracefully()
        {
            var deleteResult = await usersService.DeleteUser(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_assigned_to_role()
        {
            var userResult = await usersService.CreateUser(TestUser1);

            await LoadTenantsAndRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
        }

        [Fact]
        public async Task Adding_user_to_role_multiple_times_shouldnt_do_anything()
        {
            var userResult = await usersService.CreateUser(TestUser1);

            await LoadTenantsAndRoles();
            var role1AddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var role2AddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(role1AddResult.Success);
            Assert.True(role2AddResult.Success);
            Assert.Equal(1, userRoles.Count);
        }

        [Fact]
        public async Task Adding_user_to_multiple_roles_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser1);
            await LoadTenantsAndRoles();

            var rolesToAdd = new List<Guid> { Role1.Id, Role2.Id };
            var roleAddResult = await usersService.AddToRolesAsync(userResult.Data.Id, Tenant1.TenantId, rolesToAdd);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.Equal(2, userRoles.Count);
        }

        [Fact]
        public async Task Removing_user_from_role_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser1);

            await LoadTenantsAndRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var roleRemoveResult = await usersService.RemoveFromRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Equal(0, userRoles.Count);
        }

        [Fact]
        public async Task Removing_user_from_unassigned_role_shouldnt_do_anything()
        {
            var userResult = await usersService.CreateUser(TestUser1);

            await LoadTenantsAndRoles();

            await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role2.Id);
            await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role2.Id);
            await usersService.RemoveFromRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            await usersService.RemoveFromRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role2.Id);
            await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role2.Id);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id, Tenant1.TenantId);

            Assert.Equal(1, userRoles.Count);
        }

        [Fact]
        public async Task Managing_user_roles_should_work_properly_after_multiple_operations()
        {
            var userResult = await usersService.CreateUser(TestUser1);

            await LoadTenantsAndRoles();
            var roleRemoveResult = await usersService.RemoveFromRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Equal(0, userRoles.Count);
        }

        [Fact]
        public async Task Checking_if_user_is_in_role_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser1);

            await LoadTenantsAndRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var isUserInRole = await usersService.IsInRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.NormalizedName);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(isUserInRole);
        }

        [Fact]
        public async Task Checking_if_user_is_in_unassigend_role_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser1);

            await LoadTenantsAndRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var isUserInRole = await usersService.IsInRoleAsync(userResult.Data.Id, Tenant1.TenantId, Role2.NormalizedName);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.False(isUserInRole);
        }

        [Fact]
        public async Task Fetching_users_per_role_should_filter_correctly()
        {
            await LoadTenantsAndRoles();
            var userCreatedResult1 = await usersService.CreateUser(TestUser1);
            var userCreatedResult2 = await usersService.CreateUser(TestUser2);

            await usersService.AddToRoleAsync(userCreatedResult1.Data.Id, Tenant1.TenantId, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult1.Data.Id, Tenant2.TenantId, Role2.Id);
            await usersService.AddToRoleAsync(userCreatedResult2.Data.Id, Tenant2.TenantId, Role2.Id);

            var usersInRoleResult1 = await usersService.RoleUsersPerTenant(Role1.Id, Tenant1.TenantId);
            var usersInRoleResult2 = await usersService.RoleUsersPerTenant(Role2.Id, Tenant2.TenantId);

            Assert.Single(usersInRoleResult1.Data);
            Assert.Equal(2, usersInRoleResult2.Data.Count);
        }

        [Fact]
        public async Task Adding_user_claims_should_work()
        {
            var userCreatedResult = await usersService.CreateUser(TestUser1);

            var claimsToAdd = new List<Claim> {
                new Claim("testclaim2", "testvalue"),
                new Claim("testclaim3", "testvalue")
            };

            var resultAdding1 = await usersService.AddClaimAsync(userCreatedResult.Data.Id, new Claim("testclaim1", "testvalue"));
            var resultAdding2 = await usersService.AddClaimsAsync(userCreatedResult.Data.Id, claimsToAdd);

            Assert.True(resultAdding1.Success);
            Assert.True(resultAdding2.Success);
        }

        [Fact]
        public async Task Fetching_user_tenant_claim_data_should_work()
        {
            var userCreatedResult = await usersService.CreateUser(TestUser1);
            await LoadTenantsAndRoles();

            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Tenant1.TenantId, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Tenant2.TenantId, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Tenant2.TenantId, Role2.Id);

            var userTenantClaims = await usersService.GetUserTenantRolesClaims(userCreatedResult.Data.Id);

            Assert.Equal(2, userTenantClaims.ToList().Count);
        }

        [Fact]
        public async Task Fetching_user_tenant_claim_data_should_work_and_be_parsed()
        {
            var userCreatedResult = await usersService.CreateUser(TestUser1);
            await LoadTenantsAndRoles();

            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Tenant1.TenantId, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Tenant2.TenantId, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Tenant2.TenantId, Role2.Id);

            var userTenantClaims = await usersService.GetUserTenantRolesClaims(userCreatedResult.Data.Id);
            
            var userTenantClaimsParsed = userTenantClaims.Select(x => x.Value.DeserializeToTenantRolesClaimData()).ToList();

            var tenant1RolesCount = userTenantClaimsParsed.First(x => x.TenantId == Tenant1.TenantId).Roles.Count();
            var tenant2RolesCount = userTenantClaimsParsed.First(x => x.TenantId == Tenant2.TenantId).Roles.Count();

            Assert.Equal(1, tenant1RolesCount);
            Assert.Equal(2, tenant2RolesCount);
        }

        public void Dispose()
        {
            serviceProviderDisposable.Dispose();
        }
    }
}