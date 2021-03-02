using IdentityUtils.Api.Models.Users;
using IdentityUtils.Core.Contracts.Services.Models;
using IdentityUtils.Demos.IdentityServer4.SingleTenant;
using IdentityUtils.Demos.IdentityServer4.SingleTenant.Models;
using IdentityUtils.Demos.IdentityServer4.Tests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    [Collection(nameof(UsersApiTests))]
    public class UsersApiTests : TestAbstract<TestSingleTenantStartup, Startup, SingleTenantWebApplicationFactory>
    {
        public UsersApiTests(SingleTenantWebApplicationFactory factory) : base(factory, solutionRelativeDirectory: "IdentityUtils.Demos.IdentityServer4.SingleTenant")
        {
        }

        protected override string DatabaseName => $"IntegrationTestDatabase_{nameof(UsersApiTests)}.db";

        private UserDto GetUniqueTestUser => new UserDto
        {
            Username = "testuser1@email.com" + Guid.NewGuid().ToString().Replace("-", ""),
            Password = "Wery5trong!Pa55word",
            Email = "testuser1@email.com"
        };

        private RoleDto Role1 { get; set; } = new RoleDto
        {
            Name = "Users API integration test - Role 1" + Guid.NewGuid().ToString()
        };

        private RoleDto Role2 { get; set; } = new RoleDto
        {
            Name = "Users API integration test - Role 2" + Guid.NewGuid().ToString()
        };

        [Fact]
        public async Task Adding_user_to_multiple_roles_should_work()
        {
            var userResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = userResult.Data.First();
            await LoadRoles();

            var roleAddResult1 = await UserManagementApi.AddUserToRole(userData.Id, Role1.Id);
            var roleAddResult2 = await UserManagementApi.AddUserToRole(userData.Id, Role2.Id);

            var userRoles = await UserManagementApi.GetUserRoles(userData.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult1.Success);
            Assert.True(roleAddResult2.Success);
            Assert.Equal(2, userRoles.Data.Count());
        }

        [Fact]
        public async Task Adding_user_to_role_multiple_times_shouldnt_do_anything()
        {
            var userResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = userResult.Data.First();

            await LoadRoles();
            var role1AddResult = await UserManagementApi.AddUserToRole(userData.Id, Role1.Id);
            var role2AddResult = await UserManagementApi.AddUserToRole(userData.Id, Role1.Id);

            var userRoles = await UserManagementApi.GetUserRoles(userData.Id);

            Assert.True(userResult.Success);
            Assert.True(role1AddResult.Success);
            Assert.True(role2AddResult.Success);
            Assert.Single(userRoles.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_id_dto()
        {
            var createdResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var fetchResult = await UserManagementApi.GetUserById(createdResult.Data.First().Id);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(createdResult.Data, fetchResult.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_username_dto()
        {
            var createdResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = createdResult.Data.First();
            var fetchResult = await UserManagementApi.Search(new UsersTenantSearch(username: userData.Username));

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Contains(createdResult.Data.First(), fetchResult.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_original_dto()
        {
            var userDto = GetUniqueTestUser;
            var createdResult = await UserManagementApi.CreateUser(userDto);
            var userData = createdResult.Data.First();

            Assert.True(createdResult.Success);
            Assert.Equal(userDto.Email, userData.Email);
            Assert.Equal(userDto.Username, userData.Username);
            Assert.Equal(userDto.AdditionalDataJson, userData.AdditionalDataJson);
        }

        [Fact]
        public async Task Deleting_nonexisting_user_should_fail_gracefully()
        {
            var deleteResult = await UserManagementApi.DeleteUser(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Fetching_users_per_role_should_filter_correctly()
        {
            await LoadRoles();
            var userCreatedResult1 = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userCreatedResult2 = await UserManagementApi.CreateUser(GetUniqueTestUser);

            var userData1 = userCreatedResult1.Data.First();
            var userData2 = userCreatedResult2.Data.First();

            await UserManagementApi.AddUserToRole(userData1.Id, Role1.Id);
            await UserManagementApi.AddUserToRole(userData1.Id, Role2.Id);
            await UserManagementApi.AddUserToRole(userData2.Id, Role2.Id);

            var usersInRoleResult1 = await UserManagementApi.Search(new UsersSearch(Role1.Id));
            var usersInRoleResult2 = await UserManagementApi.Search(new UsersSearch(Role2.Id));

            Assert.Single(usersInRoleResult1.Data);
            Assert.Equal(2, usersInRoleResult2.Data.Count());
        }

        [Fact]
        public async Task Getting_user_by_multiple_params_should_work()
        {
            var createdResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = createdResult.Data.First();

            var fetchResult1 = await UserManagementApi.GetUserById(userData.Id);
            var fetchResult2 = await UserManagementApi.Search(new UsersTenantSearch(username: userData.Username));

            Assert.True(createdResult.Success);
            Assert.True(fetchResult1.Success);
            Assert.True(fetchResult2.Success);
        }

        [Fact]
        public async Task Managing_user_roles_should_work_properly_after_multiple_operations()
        {
            var userResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = userResult.Data.First();

            await LoadRoles();
            var roleRemoveResult = await UserManagementApi.RemoveUserFromRole(userData.Id, Role1.Id);
            var userRoles = await UserManagementApi.GetUserRoles(userData.Id);

            Assert.True(userResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles.Data);
        }

        [Fact]
        public async Task Removing_user_from_role_should_work()
        {
            var userResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = userResult.Data.First();

            await LoadRoles();
            var roleAddResult = await UserManagementApi.AddUserToRole(userData.Id, Role1.Id);
            var roleRemoveResult = await UserManagementApi.RemoveUserFromRole(userData.Id, Role1.Id);

            var userRoles = await UserManagementApi.GetUserRoles(userData.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles.Data);
        }

        [Fact]
        public async Task Removing_user_from_unassigned_role_shouldnt_do_anything()
        {
            var userResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = userResult.Data.First();

            await LoadRoles();

            await UserManagementApi.AddUserToRole(userData.Id, Role1.Id);
            await UserManagementApi.AddUserToRole(userData.Id, Role2.Id);
            await UserManagementApi.AddUserToRole(userData.Id, Role2.Id);
            await UserManagementApi.RemoveUserFromRole(userData.Id, Role1.Id);
            await UserManagementApi.RemoveUserFromRole(userData.Id, Role2.Id);
            await UserManagementApi.AddUserToRole(userData.Id, Role2.Id);

            var userRoles = await UserManagementApi.GetUserRoles(userData.Id);

            Assert.Single(userRoles.Data);
        }

        [Fact]
        public async Task User_should_be_able_to_change_password()
        {
            var createdResult = await UserManagementApi.CreateUser(GetUniqueTestUser);

            var passwordRequest = new PasswordForgottenRequest
            {
                Username = createdResult.Data.First().Username
            };

            var tokenResult = await UserManagementApi.GetPasswordResetToken(passwordRequest);
            var tokenData = tokenResult.Data.First();

            var newPasswordRequest = new PasswordForgottenNewPassword
            {
                Username = tokenData.Username,
                Token = tokenData.Token,
                Password = "Ver!32StrongPa55word"
            };

            var changePasswordResult = await UserManagementApi.SetNewPasswordAfterReset(newPasswordRequest);

            Assert.True(createdResult.Success);
            Assert.True(tokenResult.Success);
            Assert.True(changePasswordResult.Success);
        }

        [Fact]
        public async Task User_should_be_assigned_to_role()
        {
            var userResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userData = userResult.Data.First();

            await LoadRoles();
            var roleAddResult = await UserManagementApi.AddUserToRole(userData.Id, Role1.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
        }

        [Fact]
        public async Task User_should_be_deleted()
        {
            var createdResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var deleteResult = await UserManagementApi.DeleteUser(createdResult.Data.First().Id);

            Assert.True(createdResult.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_updated()
        {
            var createdResult = await UserManagementApi.CreateUser(GetUniqueTestUser);
            var userDto = createdResult.Data.First();

            userDto.Email = "changed@email.com";
            var updatedResult = await UserManagementApi.UpdateUser(userDto);

            var fetchResult = await UserManagementApi.GetUserById(userDto.Id);

            Assert.True(createdResult.Success);
            Assert.True(updatedResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(userDto.Email, fetchResult.Data.First().Email);
        }

        private async Task LoadRoles()
        {
            var rolesResult = await RoleManagementApiSingleTenant.GetRoles();

            var roles = rolesResult.Data;

            if (!roles.Any(x => x.Name == Role1.Name))
            {
                var result = await RoleManagementApiSingleTenant.AddRole(Role1);
                Role1 = result.Data.First();
            }

            if (!roles.Any(x => x.Name == Role2.Name))
            {
                var result = await RoleManagementApiSingleTenant.AddRole(Role2);
                Role2 = result.Data.First();
            }
        }
    }
}