using IdentityUtils.Api.Extensions;
using IdentityUtils.Api.Models.Users;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    public class UsersApiTests : TestAbstract
    {
        private readonly RoleManagementApi<RoleDto> roleManagementApi;
        private readonly TenantManagementApi<TenantDto> tenantManagementApi;
        private readonly UserManagementApi<UserDto> userManagementApi;

        public UsersApiTests() : base()
        {
            roleManagementApi = serviceProvider.GetRequiredService<RoleManagementApi<RoleDto>>();
            tenantManagementApi = serviceProvider.GetRequiredService<TenantManagementApi<TenantDto>>();
            userManagementApi = serviceProvider.GetRequiredService<UserManagementApi<UserDto>>();
        }

        private TenantDto Tenant1 { get; set; } = new TenantDto
        {
            Name = "Users API integration test - Tenant 1" + Guid.NewGuid().ToString()
        };

        private TenantDto Tenant2 { get; set; } = new TenantDto
        {
            Name = "Users API integration test - Tenant 2" + Guid.NewGuid().ToString()
        };

        private RoleDto Role1 { get; set; } = new RoleDto
        {
            Name = "Users API integration test - Role 1" + Guid.NewGuid().ToString()
        };

        private RoleDto Role2 { get; set; } = new RoleDto
        {
            Name = "Users API integration test - Role 2" + Guid.NewGuid().ToString()
        };

        private UserDto GetUniqueTestUser => new UserDto
        {
            Username = "testuser1@email.com"+Guid.NewGuid().ToString().Replace("-", ""),
            Password = "Wery5trong!Pa55word",
            Email = "testuser1@email.com"
        };

        private async Task LoadTenantsAndRoles()
        {
            var rolesResult = await roleManagementApi.GetRoles();
            var tenantsResult = await tenantManagementApi.GetTenants();

            var roles = rolesResult.Data;
            var tenants = tenantsResult.Data;

            if (!roles.Any(x => x.Name == Role1.Name))
            {
                var result = await roleManagementApi.AddRole(Role1);
                Role1 = result.Data;
            }

            if (!roles.Any(x => x.Name == Role2.Name))
            {
                var result = await roleManagementApi.AddRole(Role2);
                Role2 = result.Data;
            }

            if (!tenants.Any(x => x.Name == Tenant1.Name))
            {
                var result = await tenantManagementApi.AddTenant(Tenant1);
                Tenant1 = result.Data;
            }

            if (!tenants.Any(x => x.Name == Tenant2.Name))
            {
                var result = await tenantManagementApi.AddTenant(Tenant2);
                Tenant2 = result.Data;
            }
        }

        [Fact]
        public async Task Getting_user_by_multiple_params_should_work()
        {
            var createdResult = await userManagementApi.CreateUser(GetUniqueTestUser);

            var fetchResult1 = await userManagementApi.GetUserById(createdResult.Data.Id);
            var fetchResult2 = await userManagementApi.GetUserByUsername(createdResult.Data.Username);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult1.Success);
            Assert.True(fetchResult2.Success);
        }

        [Fact]
        public async Task Created_user_dto_should_match_original_dto()
        {
            var userDto = GetUniqueTestUser;
            var createdResult = await userManagementApi.CreateUser(userDto);

            Assert.True(createdResult.Success);
            Assert.Equal(userDto.Email, createdResult.Data.Email);
            Assert.Equal(userDto.Username, createdResult.Data.Username);
            Assert.Equal(userDto.AdditionalDataJson, createdResult.Data.AdditionalDataJson);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_id_dto()
        {
            var createdResult = await userManagementApi.CreateUser(GetUniqueTestUser);
            var fetchResult = await userManagementApi.GetUserById(createdResult.Data.Id);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(createdResult.Data, fetchResult.Data);
        }

        [Fact]
        public async Task Created_user_dto_should_match_fetched_by_username_dto()
        {
            var createdResult = await userManagementApi.CreateUser(GetUniqueTestUser);
            var fetchResult = await userManagementApi.GetUserByUsername(createdResult.Data.Username);

            Assert.True(createdResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(createdResult.Data, fetchResult.Data);
        }

        [Fact]
        public async Task User_should_be_updated()
        {
            var createdResult = await userManagementApi.CreateUser(GetUniqueTestUser);
            var userDto = createdResult.Data;

            userDto.Email = "changed@email.com";
            var updatedResult = await userManagementApi.UpdateUser(userDto);

            var fetchResult = await userManagementApi.GetUserById(userDto.Id);

            Assert.True(createdResult.Success);
            Assert.True(updatedResult.Success);
            Assert.True(fetchResult.Success);
            Assert.Equal(userDto.Email, fetchResult.Data.Email);
        }

        [Fact]
        public async Task User_should_be_deleted()
        {
            var createdResult = await userManagementApi.CreateUser(GetUniqueTestUser);
            var deleteResult = await userManagementApi.DeleteUser(createdResult.Data.Id);

            Assert.True(createdResult.Success);
            Assert.True(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_able_to_change_password()
        {
            var createdResult = await userManagementApi.CreateUser(GetUniqueTestUser);

            var passwordRequest = new PasswordForgottenRequest
            {
                Username = createdResult.Data.Username
            };

            var tokenResult = await userManagementApi.GetPasswordResetToken(passwordRequest);

            var newPasswordRequest = new PasswordForgottenNewPassword
            {
                Username = tokenResult.Data.Username,
                Token = tokenResult.Data.Token,
                Password = "Ver!32StrongPa55word"
            };

            var changePasswordResult = await userManagementApi.SetNewPasswordAfterReset(newPasswordRequest);

            Assert.True(createdResult.Success);
            Assert.True(tokenResult.Success);
            Assert.True(changePasswordResult.Success);
        }

        [Fact]
        public async Task Deleting_nonexisting_user_should_fail_gracefully()
        {
            var deleteResult = await userManagementApi.DeleteUser(Guid.NewGuid());

            Assert.False(deleteResult.Success);
        }

        [Fact]
        public async Task User_should_be_assigned_to_role()
        {
            var userResult = await userManagementApi.CreateUser(GetUniqueTestUser);

            await LoadTenantsAndRoles();
            var roleAddResult = await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
        }

        [Fact]
        public async Task Adding_user_to_role_multiple_times_shouldnt_do_anything()
        {
            var userResult = await userManagementApi.CreateUser(GetUniqueTestUser);

            await LoadTenantsAndRoles();
            var role1AddResult = await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var role2AddResult = await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await userManagementApi.GetUserRoles(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(role1AddResult.Success);
            Assert.True(role2AddResult.Success);
            Assert.Equal(1, userRoles.Data.Count);
        }

        [Fact]
        public async Task Adding_user_to_multiple_roles_should_work()
        {
            var userResult = await userManagementApi.CreateUser(GetUniqueTestUser);
            await LoadTenantsAndRoles();

            var roleAddResult1 = await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var roleAddResult2 = await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role2.Id);

            var userRoles = await userManagementApi.GetUserRoles(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult1.Success);
            Assert.True(roleAddResult2.Success);
            Assert.Equal(2, userRoles.Data.Count);
        }

        [Fact]
        public async Task Removing_user_from_role_should_work()
        {
            var userResult = await userManagementApi.CreateUser(GetUniqueTestUser);

            await LoadTenantsAndRoles();
            var roleAddResult = await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var roleRemoveResult = await userManagementApi.RemoveFromRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);

            var userRoles = await userManagementApi.GetUserRoles(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(roleAddResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Equal(0, userRoles.Data.Count);
        }

        [Fact]
        public async Task Removing_user_from_unassigned_role_shouldnt_do_anything()
        {
            var userResult = await userManagementApi.CreateUser(GetUniqueTestUser);

            await LoadTenantsAndRoles();

            await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role2.Id);
            await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role2.Id);
            await userManagementApi.RemoveFromRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            await userManagementApi.RemoveFromRole(userResult.Data.Id, Tenant1.TenantId, Role2.Id);
            await userManagementApi.AddToRole(userResult.Data.Id, Tenant1.TenantId, Role2.Id);

            var userRoles = await userManagementApi.GetUserRoles(userResult.Data.Id, Tenant1.TenantId);

            Assert.Equal(1, userRoles.Data.Count);
        }

        [Fact]
        public async Task Managing_user_roles_should_work_properly_after_multiple_operations()
        {
            var userResult = await userManagementApi.CreateUser(GetUniqueTestUser);

            await LoadTenantsAndRoles();
            var roleRemoveResult = await userManagementApi.RemoveFromRole(userResult.Data.Id, Tenant1.TenantId, Role1.Id);
            var userRoles = await userManagementApi.GetUserRoles(userResult.Data.Id, Tenant1.TenantId);

            Assert.True(userResult.Success);
            Assert.True(roleRemoveResult.Success);
            Assert.Equal(0, userRoles.Data.Count);
        }

        [Fact]
        public async Task Fetching_users_per_role_should_filter_correctly()
        {
            await LoadTenantsAndRoles();
            var userCreatedResult1 = await userManagementApi.CreateUser(GetUniqueTestUser);
            var userCreatedResult2 = await userManagementApi.CreateUser(GetUniqueTestUser);

            await userManagementApi.AddToRole(userCreatedResult1.Data.Id, Tenant1.TenantId, Role1.Id);
            await userManagementApi.AddToRole(userCreatedResult1.Data.Id, Tenant2.TenantId, Role2.Id);
            await userManagementApi.AddToRole(userCreatedResult2.Data.Id, Tenant2.TenantId, Role2.Id);

            var usersInRoleResult1 = await userManagementApi.RoleUsersPerTenant(Tenant1.TenantId, Role1.Id);
            var usersInRoleResult2 = await userManagementApi.RoleUsersPerTenant(Tenant2.TenantId, Role2.Id);

            Assert.Single(usersInRoleResult1.Data);
            Assert.Equal(2, usersInRoleResult2.Data.Count);
        }
    }
}