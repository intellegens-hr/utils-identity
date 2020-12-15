﻿using IdentityUtils.Core.Contracts.Services.Models;
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
            var userCreatedResult = await usersService.CreateUser(TestUser);

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
        public async Task Adding_user_to_multiple_roles_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser);
            await LoadRoles();

            var rolesToAdd = new List<Guid> { Role1.Id, Role2.Id };
            var roleAddResult = await usersService.AddToRolesAsync(userResult.Data.Id, rolesToAdd);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.Equal(2, userRoles.Count());
        }

        [Fact]
        public async Task Adding_user_to_role_multiple_times_shouldnt_do_anything()
        {
            var userResult = await usersService.CreateUser(TestUser);

            await LoadRoles();
            var role1AddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Role1.Id);
            var role2AddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Role1.Id);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id);

            Assert.True(userResult.Success);
            Assert.True(role1AddResult.Success);
            Assert.True(role2AddResult.Success);
            Assert.Equal(1, userRoles.Count());
        }

        [Fact]
        public async Task Checking_if_user_is_in_role_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser);

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Role1.Id);
            var isUserInRole = await usersService.IsInRoleAsync(userResult.Data.Id, Role1.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(isUserInRole);
        }

        [Fact]
        public async Task Checking_if_user_is_in_unassigend_role_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser);

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Role1.Id);
            var isUserInRole = await usersService.IsInRoleAsync(userResult.Data.Id, Guid.NewGuid());

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.False(isUserInRole);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_id_dto()
        {
            var createdResult = await usersService.CreateUser(TestUser);
            var fetchResult = await usersService.FindByIdAsync<UserDto>(createdResult.Data.Id);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(createdResult.Data, fetchResult.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_username_dto()
        {
            var createdResult = await usersService.CreateUser(TestUser);
            var fetchResult = await usersService.FindByNameAsync<UserDto>(createdResult.Data.Username);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(createdResult.Data, fetchResult.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_original_dto()
        {
            var testUser = TestUser;
            var createdResult = await usersService.CreateUser(testUser);

            Assert.True(createdResult.Success);
            Assert.Equal(testUser.Email, createdResult.Data.Email);
            Assert.Equal(testUser.Username, createdResult.Data.Username);
            Assert.Equal(testUser.DisplayName, createdResult.Data.DisplayName);
            Assert.Equal(testUser.AdditionalDataJson, createdResult.Data.AdditionalDataJson);
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
            var userCreatedResult = await usersService.CreateUser(TestUser);
            await LoadRoles();

            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult.Data.Id, Role2.Id);

            var userRoles = await usersService.GetRolesAsync(userCreatedResult.Data.Id);

            Assert.Equal(2, userRoles.Count());
        }

        [Fact]
        public async Task Fetching_users_per_role_should_filter_correctly()
        {
            await LoadRoles();
            var userCreatedResult1 = await usersService.CreateUser(TestUser);
            var userCreatedResult2 = await usersService.CreateUser(TestUser);

            await usersService.AddToRoleAsync(userCreatedResult1.Data.Id, Role1.Id);
            await usersService.AddToRoleAsync(userCreatedResult1.Data.Id, Role2.Id);
            await usersService.AddToRoleAsync(userCreatedResult2.Data.Id, Role2.Id);

            var usersInRoleResult1 = await usersService.Search(new UsersSearch(roleId: Role1.Id));
            var usersInRoleResult2 = await usersService.Search(new UsersSearch(roleId: Role2.Id));

            Assert.Single(usersInRoleResult1);
            Assert.Equal(2, usersInRoleResult2.Count());
        }

        [Fact]
        public async Task Getting_all_users_should_work()
        {
            var createdResult1 = await usersService.CreateUser(TestUser);
            var createdResult2 = await usersService.CreateUser(TestUser);

            var users = await usersService.GetAllUsers();
            var userIds = users.Select(x => x.Id);

            Assert.True(createdResult1.Success);
            Assert.True(createdResult2.Success);
            Assert.Contains(createdResult1.Data.Id, userIds);
            Assert.Contains(createdResult2.Data.Id, userIds);
        }

        [Fact]
        public async Task Getting_user_by_multiple_params_should_work()
        {
            var createdResult = await usersService.CreateUser(TestUser);

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
        public async Task Managing_user_roles_should_work_properly_after_multiple_operations()
        {
            var userResult = await usersService.CreateUser(TestUser);

            await LoadRoles();
            var roleRemoveResult = await usersService.RemoveFromRoleAsync(userResult.Data.Id, Role1.Id);
            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id);

            Assert.True(userResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Equal(0, userRoles.Count());
        }

        [Fact]
        public async Task Removing_user_from_role_should_work()
        {
            var userResult = await usersService.CreateUser(TestUser);

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Role1.Id);
            var roleRemoveResult = await usersService.RemoveFromRoleAsync(userResult.Data.Id, Role1.Id);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Equal(0, userRoles.Count());
        }

        [Fact]
        public async Task Removing_user_from_unassigned_role_shouldnt_do_anything()
        {
            var userResult = await usersService.CreateUser(TestUser);

            await LoadRoles();

            await usersService.AddToRoleAsync(userResult.Data.Id, Role1.Id);
            await usersService.AddToRoleAsync(userResult.Data.Id, Role2.Id);
            await usersService.AddToRoleAsync(userResult.Data.Id, Role2.Id);
            await usersService.RemoveFromRoleAsync(userResult.Data.Id, Role1.Id);
            await usersService.RemoveFromRoleAsync(userResult.Data.Id, Role2.Id);
            await usersService.AddToRoleAsync(userResult.Data.Id, Role2.Id);

            var userRoles = await usersService.GetRolesAsync(userResult.Data.Id);

            Assert.Equal(1, userRoles.Count());
        }

        [Fact]
        public async Task User_should_be_able_to_change_password()
        {
            var createdResult = await usersService.CreateUser(TestUser);
            var tokenResult = await usersService.GeneratePasswordResetTokenAsync(createdResult.Data.Username);
            var changePasswordResult = await usersService.ResetPasswordAsync(createdResult.Data.Username, tokenResult.Data, "NewStrong1324Pa55!");

            Assert.True(createdResult.Success);
            Assert.True(tokenResult.Success);
            Assert.True(changePasswordResult.Success);
        }

        [Fact]
        public async Task User_should_be_assigned_to_role()
        {
            var userResult = await usersService.CreateUser(TestUser);

            await LoadRoles();
            var roleAddResult = await usersService.AddToRoleAsync(userResult.Data.Id, Role1.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
        }

        [Fact]
        public async Task User_should_be_deleted()
        {
            var createdResult = await usersService.CreateUser(TestUser);
            var deleteResult = await usersService.DeleteUser(createdResult.Data.Id);

            Assert.True(createdResult.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_updated()
        {
            var createdResult = await usersService.CreateUser(TestUser);
            var userDto = createdResult.Data;

            userDto.Email = "changed@email.com";
            var updatedResult = await usersService.UpdateUser(userDto);

            var fetchResult = await usersService.FindByIdAsync<UserDto>(userDto.Id);

            Assert.True(createdResult.Success);
            Assert.True(updatedResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(userDto.Email, fetchResult.Data.Email);
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

            Role1 = role1Result.Data;
            Role2 = role2Result.Data;
        }
    }
}