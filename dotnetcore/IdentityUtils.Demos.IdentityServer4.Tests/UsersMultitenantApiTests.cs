using IdentityUtils.Api.Models.Users;
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
    [Collection(nameof(UsersMultitenantApiTests))]
    public class UsersMultitenantApiTests : TestAbstract<TestMultiTenantStartup, Startup, MultitenantWebApplicationFactory>
    {
        public UsersMultitenantApiTests(MultitenantWebApplicationFactory factory) :
            base(factory, solutionRelativeDirectory: "IdentityUtils.Demos.IdentityServer4.MultiTenant")
        {
        }

        protected override string DatabaseName => $"IntegrationTestDatabase_{nameof(UsersMultitenantApiTests)}.db";

        private RoleDto Role1 { get; set; } = new RoleDto
        {
            Name = "Users API integration test - Role 1" + Guid.NewGuid().ToString()
        };

        private RoleDto Role2 { get; set; } = new RoleDto
        {
            Name = "Users API integration test - Role 2" + Guid.NewGuid().ToString()
        };

        private TenantDto Tenant1 { get; set; } = new TenantDto
        {
            Name = "Users API integration test - Tenant 1" + Guid.NewGuid().ToString()
        };

        private TenantDto Tenant2 { get; set; } = new TenantDto
        {
            Name = "Users API integration test - Tenant 2" + Guid.NewGuid().ToString()
        };

        private UserDto UniqueTestUser => new UserDto
        {
            Username = "testuser1@email.com" + Guid.NewGuid().ToString().Replace("-", ""),
            Password = "Wery5trong!Pa55word",
            Email = "testuser1@email.com"
        };

        [Fact]
        public async Task Adding_user_to_multiple_roles_should_work()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();
            await LoadTenantsAndRoles();

            var roleAddResult1 = await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var roleAddResult2 = await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role2.Id);

            var userRoles = await UserTenantManagementApi.GetUserRoles(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult1.Success);
            Assert.True(roleAddResult2.Success);
            Assert.Equal(2, userRoles.Data.Count());
        }

        [Fact]
        public async Task Adding_user_to_role_multiple_times_shouldnt_do_anything()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var role1AddResult = await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var role2AddResult = await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await UserTenantManagementApi.GetUserRoles(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(role1AddResult.Success);
            Assert.True(role2AddResult.Success);
            Assert.Single(userRoles.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_id_dto()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();
            var (resultFetched, userFetched) = await UserTenantManagementApi.GetUserById(userCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated, userFetched);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_username_dto()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();
            var resultSearch = await UserTenantManagementApi.Search(new UsersTenantSearch(username: userCreated.Username));

            Assert.True(resultCreated.Success);
            Assert.True(resultSearch.Success);
            Assert.Contains(userCreated, resultSearch.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_original_dto()
        {
            var userDto = UniqueTestUser;
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(userDto).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.Equal(userDto.Email, userCreated.Email);
            Assert.Equal(userDto.Username, userCreated.Username);
            Assert.Equal(userDto.AdditionalDataJson, userCreated.AdditionalDataJson);
        }

        [Fact]
        public async Task Deleting_nonexisting_user_should_fail_gracefully()
        {
            var deleteResult = await UserTenantManagementApi.DeleteUser(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Fetching_users_per_role_should_filter_correctly()
        {
            await LoadTenantsAndRoles();
            var (_, userCreated1) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();
            var (_, userCreated2) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            await UserTenantManagementApi.AddUserToRole(userCreated1.Id, Tenant1.TenantId, Role1.Id);
            await UserTenantManagementApi.AddUserToRole(userCreated1.Id, Tenant2.TenantId, Role2.Id);
            await UserTenantManagementApi.AddUserToRole(userCreated2.Id, Tenant2.TenantId, Role2.Id);

            var usersInRoleResult1 = await UserTenantManagementApi.Search(new UsersTenantSearch(Tenant1.TenantId, Role1.Id));
            var usersInRoleResult2 = await UserTenantManagementApi.Search(new UsersTenantSearch(Tenant2.TenantId, Role2.Id));

            Assert.Single(usersInRoleResult1.Data);
            Assert.Equal(2, usersInRoleResult2.Data.Count());
        }

        [Fact]
        public async Task Getting_user_by_multiple_params_should_work()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            var fetchResult1 = await UserTenantManagementApi.GetUserById(userCreated.Id);
            var fetchResult2 = await UserTenantManagementApi.Search(new UsersTenantSearch(username: userCreated.Username));

            Assert.True(resultCreated.Success);
            Assert.True(fetchResult1.Success);
            Assert.True(fetchResult2.Success);
        }

        [Fact]
        public async Task Managing_user_roles_should_work_properly_after_multiple_operations()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleRemoveResult = await UserTenantManagementApi.RemoveUserFromRole(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var userRoles = await UserTenantManagementApi.GetUserRoles(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles.Data);
        }

        [Fact]
        public async Task Removing_user_from_role_should_work()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleAddResult = await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role1.Id);
            var roleRemoveResult = await UserTenantManagementApi.RemoveUserFromRole(userCreated.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await UserTenantManagementApi.GetUserRoles(userCreated.Id, Tenant1.TenantId);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles.Data);
        }

        [Fact]
        public async Task Removing_user_from_unassigned_role_shouldnt_do_anything()
        {
            var (_, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();

            await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role1.Id);
            await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role2.Id);
            await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role2.Id);
            await UserTenantManagementApi.RemoveUserFromRole(userCreated.Id, Tenant1.TenantId, Role1.Id);
            await UserTenantManagementApi.RemoveUserFromRole(userCreated.Id, Tenant1.TenantId, Role2.Id);
            await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role2.Id);

            var userRoles = await UserTenantManagementApi.GetUserRoles(userCreated.Id, Tenant1.TenantId);

            Assert.Single(userRoles.Data);
        }

        [Fact]
        public async Task User_should_be_able_to_change_password()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            var passwordRequest = new PasswordForgottenRequest
            {
                Username = userCreated.Username
            };

            var (resultToken, tokenData) = await UserTenantManagementApi.GetPasswordResetToken(passwordRequest).UnpackSingleOrDefault();

            var newPasswordRequest = new PasswordForgottenNewPassword
            {
                Username = tokenData.Username,
                Token = tokenData.Token,
                Password = "Ver!32StrongPa55word"
            };

            var changePasswordResult = await UserTenantManagementApi.SetNewPasswordAfterReset(newPasswordRequest);

            Assert.True(resultCreated.Success);
            Assert.True(resultToken.Success);
            Assert.True(changePasswordResult.Success);
        }

        [Fact]
        public async Task User_should_be_assigned_to_role()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            await LoadTenantsAndRoles();
            var roleAddResult = await UserTenantManagementApi.AddUserToRole(userCreated.Id, Tenant1.TenantId, Role1.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
        }

        [Fact]
        public async Task User_should_be_deleted()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();
            var deleteResult = await UserTenantManagementApi.DeleteUser(userCreated.Id);

            Assert.True(resultCreated.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_updated()
        {
            var (resultCreated, userCreated) = await UserTenantManagementApi.CreateUser(UniqueTestUser).UnpackSingleOrDefault();

            userCreated.Email = "changed@email.com";
            var updatedResult = await UserTenantManagementApi.UpdateUser(userCreated);

            var (resultFetched, userFetched) = await UserTenantManagementApi.GetUserById(userCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(updatedResult.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated.Email, userFetched.Email);
        }

        private async Task LoadTenantsAndRoles()
        {
            var rolesResult = await RoleManagementApi.GetRoles();
            var tenantsResult = await TenantManagementApi.GetTenants();

            var roles = rolesResult.Data;
            var tenants = tenantsResult.Data;

            if (!roles.Any(x => x.Name == Role1.Name))
            {
                var result = await RoleManagementApi.AddRole(Role1);
                Role1 = result.Data.Single();
            }

            if (!roles.Any(x => x.Name == Role2.Name))
            {
                var result = await RoleManagementApi.AddRole(Role2);
                Role2 = result.Data.Single();
            }

            if (!tenants.Any(x => x.Name == Tenant1.Name))
            {
                var result = await TenantManagementApi.AddTenant(Tenant1);
                Tenant1 = result.Data.Single();
            }

            if (!tenants.Any(x => x.Name == Tenant2.Name))
            {
                var result = await TenantManagementApi.AddTenant(Tenant2);
                Tenant2 = result.Data.Single();
            }
        }
    }
}