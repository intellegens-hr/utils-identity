using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Services.Models;
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
    public class UsersServiceTests : TestAbstractSingleTenant
    {
        private readonly RolesService rolesService;
        private readonly UsersService usersService;

        public UsersServiceTests() : base()
        {
            usersService = GetService<UsersService>();
            rolesService = GetService<RolesService>();
        }

        private RoleDto Role1 { get; set; }

        private RoleDto Role2 { get; set; }

        private UserDto TestUser => new UserDto
        {
            Username = $"testuser1{GetUniqueId()}@email.com",
            Password = "Wery5trong!Pa55word",
            Email = "testuser1{GetUniqueId()}@email.com"
        };

        [Fact]
        public async Task Adding_user_claims_should_work()
        {
            var (_, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            var claimsToAdd = new Claim[] {
                new Claim("testclaim2", "testvalue"),
                new Claim("testclaim3", "testvalue")
            };

            var resultAdding1 = await usersService.AddClaimAsync(userCreated.Id, new Claim("testclaim1", "testvalue"));
            var resultAdding2 = await usersService.AddClaimsAsync(userCreated.Id, claimsToAdd);

            Assert.True(resultAdding1.Success);
            Assert.True(resultAdding2.Success);
        }

        [Fact]
        public async Task Adding_user_to_multiple_roles_should_work()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            await LoadRoles();

            var rolesToAdd = new Guid[] { Role1.Id, Role2.Id };
            var roleAddResult = await usersService.AddToRolesAsync(userCreated.Id, rolesToAdd);

            var userRoles = await usersService.GetRolesAsync(userCreated.Id);
            var roleIds = userRoles.Select(x => x.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.Contains(Role1.Id, roleIds);
            Assert.Contains(Role2.Id, roleIds);
            Assert.Equal(2, userRoles.Count());
        }

        [Fact]
        public async Task Adding_user_to_role_multiple_times_shouldnt_do_anything()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadRoles();
            var role1AddResult = await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);
            var role2AddResult = await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);

            var userRoles = await usersService.GetRolesAsync(userCreated.Id);

            Assert.True(resultCreated.Success);
            Assert.True(role1AddResult.Success);
            Assert.True(role2AddResult.Success);
            Assert.Single(userRoles);
        }

        [Fact]
        public async Task Checking_if_user_is_in_role_should_work()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);
            var isUserInRole = await usersService.IsInRoleAsync(userCreated.Id, Role1.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(isUserInRole);
        }

        [Fact]
        public async Task Checking_if_user_is_in_unassigend_role_should_work()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);
            var isUserInRole = await usersService.IsInRoleAsync(userCreated.Id, Guid.NewGuid());

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.False(isUserInRole);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_id_dto()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (resultFetched, userFetched) = await usersService.FindByIdAsync<UserDto>(userCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated, userFetched);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_username_dto()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (resultFetched, userFetched) = await usersService.FindByNameAsync<UserDto>(userCreated.Username).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated, userFetched);
        }

        [Fact]
        public async Task Created_user_dto_should_match_original_dto()
        {
            var dto = TestUser;
            var (resultCreated, userCreated) = await usersService.CreateUser(dto).UnpackSingleOrDefault();

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
            var createdResult = await usersService.CreateUser(userDto);
            Assert.False(createdResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_user_should_fail_gracefully()
        {
            var deleteResult = await usersService.DeleteUser(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task Fetching_user_tenant_claim_data_should_work()
        {
            var (_, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            await LoadRoles();

            await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);
            await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);
            await usersService.AddToRoleAsync(userCreated.Id, Role2.Id);

            var userRoles = await usersService.GetRolesAsync(userCreated.Id);
            var roleIds = userRoles.Select(x => x.Id);

            Assert.Contains(Role1.Id, roleIds);
            Assert.Contains(Role2.Id, roleIds);
            Assert.Equal(2, userRoles.Count());
        }

        [Fact]
        public async Task Fetching_users_per_role_should_filter_correctly()
        {
            await LoadRoles();
            var (_, userCreated1) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (_, userCreated2) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await usersService.AddToRoleAsync(userCreated1.Id, Role1.Id);
            await usersService.AddToRoleAsync(userCreated1.Id, Role2.Id);
            await usersService.AddToRoleAsync(userCreated2.Id, Role2.Id);

            var usersInRoleResult1 = await usersService.Search(new UsersSearch(roleId: Role1.Id));
            var usersInRoleResult2 = await usersService.Search(new UsersSearch(roleId: Role2.Id));

            Assert.Single(usersInRoleResult1);
            Assert.Equal(2, usersInRoleResult2.Count());
        }

        [Fact]
        public async Task Getting_all_users_should_work()
        {
            var (resultCreated1, userCreated1) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (resultCreated2, userCreated2) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            var users = await usersService.GetAllUsers();
            var userIds = users.Select(x => x.Id);

            Assert.True(resultCreated1.Success);
            Assert.True(resultCreated2.Success);
            Assert.Contains(userCreated1.Id, userIds);
            Assert.Contains(userCreated2.Id, userIds);
        }

        [Fact]
        public async Task Getting_user_by_multiple_params_should_work()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            var fetchResultDb1 = await usersService.FindByIdAsync(userCreated.Id);
            var fetchResultDb2 = await usersService.FindByNameAsync(userCreated.Username);

            var fetchResult1 = await usersService.FindByIdAsync<UserDto>(userCreated.Id);
            var fetchResult2 = await usersService.FindByNameAsync<UserDto>(userCreated.Username);

            Assert.True(resultCreated.Success);
            Assert.True(fetchResultDb1.Success);
            Assert.True(fetchResultDb2.Success);
            Assert.True(fetchResult1.Success);
            Assert.True(fetchResult2.Success);
        }

        [Fact]
        public async Task Managing_user_roles_should_work_properly_after_multiple_operations()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadRoles();
            var roleRemoveResult = await usersService.RemoveFromRoleAsync(userCreated.Id, Role1.Id);
            var userRoles = await usersService.GetRolesAsync(userCreated.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles);
        }

        [Fact]
        public async Task Removing_user_from_role_should_work()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);
            var roleRemoveResult = await usersService.RemoveFromRoleAsync(userCreated.Id, Role1.Id);

            var userRoles = await usersService.GetRolesAsync(userCreated.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Empty(userRoles);
        }

        [Fact]
        public async Task Removing_user_from_unassigned_role_shouldnt_do_anything()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadRoles();

            await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);
            await usersService.AddToRoleAsync(userCreated.Id, Role2.Id);
            await usersService.AddToRoleAsync(userCreated.Id, Role2.Id);
            await usersService.RemoveFromRoleAsync(userCreated.Id, Role1.Id);
            await usersService.RemoveFromRoleAsync(userCreated.Id, Role2.Id);
            await usersService.AddToRoleAsync(userCreated.Id, Role2.Id);

            var userRoles = await usersService.GetRolesAsync(userCreated.Id);

            Assert.Single(userRoles);
            Assert.Contains(Role2.Id, userRoles.Select(x => x.Id));
        }

        [Fact]
        public async Task User_should_be_able_to_change_password()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            var (resultToken, token) = await usersService.GeneratePasswordResetTokenAsync(userCreated.Username).UnpackSingleOrDefault();
            var changePasswordResult = await usersService.ResetPasswordAsync(userCreated.Username, token, "NewStrong1324Pa55!");

            Assert.True(resultCreated.Success);
            Assert.True(resultToken.Success);
            Assert.True(changePasswordResult.Success);
        }

        [Fact]
        public async Task User_should_be_assigned_to_role()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userCreated.Id, Role1.Id);

            Assert.True(resultCreated.Success);
            Assert.True(roleAddResult.Success);
        }

        [Fact]
        public async Task User_should_be_deleted()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();
            var deleteResult = await usersService.DeleteUser(userCreated.Id);

            Assert.True(resultCreated.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_updated()
        {
            var (resultCreated, userCreated) = await usersService.CreateUser(TestUser).UnpackSingleOrDefault();

            userCreated.Email = "changed@email.com";
            var updatedResult = await usersService.UpdateUser(userCreated);

            var (resultFetched, userFetched) = await usersService.FindByIdAsync<UserDto>(userCreated.Id).UnpackSingleOrDefault();

            Assert.True(resultCreated.Success);
            Assert.True(updatedResult.Success);
            Assert.True(resultFetched.Success);
            Assert.Equal(userCreated.Email, userFetched.Email);
        }

        private async Task LoadRoles()
        {
            Role1 = new RoleDto
            {
                Name = $"Role 1{GetUniqueId()}"
            };

            Role2 = new RoleDto
            {
                Name = $"Role 2{GetUniqueId()}",
            };

            var role1Result = await rolesService.AddRole(Role1);
            var role2Result = await rolesService.AddRole(Role2);

            Role1 = role1Result.Data.Single();
            Role2 = role2Result.Data.Single();
        }
    }
}