﻿using IdentityUtils.Api.Extensions.Cli.Commons;
using IdentityUtils.Api.Extensions.Cli.Models;
using IdentityUtils.Api.Extensions.Cli.Utils;
using IdentityUtils.Core.Contracts.Services.Models;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions.Cli.Commands
{
    [Command("users-tenant", Description = "Manage multitenancy users"),
        Subcommand(typeof(List)),
        Subcommand(typeof(ListPerRole)),
        Subcommand(typeof(Delete)),
        Subcommand(typeof(Add)),
        Subcommand(typeof(AddToRole)),
        Subcommand(typeof(RemoveFromRole)),
        Subcommand(typeof(Update))]
    internal class UsersTenant
    {
        [Option(ShortName = "r", LongName = "api-base-route", Description = "Base route user management API uses (defaults to /api/management/users)")]
        internal static string ApiBaseRoute { get; set; }

        private static void ConsoleOutputUsers(IConsole console, UserDto user)
            => ConsoleOutputUsers(console, new UserDto[] { user });

        private static void ConsoleOutputUsers(IConsole console, IEnumerable<UserDto> users)
        {
            console.WriteLine("\tID\t\tUSERNAME\t\tEMAIL\t\tADDITIONAL DATA");
            console.WriteLine("--------------------------------------------");
            foreach (var user in users)
            {
                console.WriteLine($"{user.Id}\t\t{user.Username}\t\t{user.Email}\t\t{user.AdditionalDataJson}");
            }
        }

        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }

        [Command(Description = "Add user"), HelpOption]
        private class Add
        {
            [Required(ErrorMessage = "Must specify user e-mail")]
            [Option(Description = "Email")]
            public string Email { get; }

            [Required(ErrorMessage = "Must specify user password")]
            [Option(Description = "Password")]
            public string Password { get; }

            [Required(ErrorMessage = "Must specify username")]
            [Option(Description = "Username")]
            public string Username { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                UserDto user = new UserDto
                {
                    Username = Username,
                    Email = Email,
                    Password = Password
                };

                var userAddResult = await Shared.GetUserTenantManagementApi(console).CreateUser(user);

                userAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                ConsoleOutputUsers(console, userAddResult.Data.First());
            }
        }

        [Command(Description = "Add user to role"), HelpOption]
        private class AddToRole
        {
            [Required(ErrorMessage = "Must specify role ID")]
            [Option(Description = "Role Id")]
            [GuidValidator]
            public string RoleId { get; }

            [Required(ErrorMessage = "Must specify tenantId")]
            [Option(Description = "Tenant Id")]
            [GuidValidator]
            public string TenantId { get; }

            [Required(ErrorMessage = "Must specify user ID")]
            [Option(Description = "User Id")]
            [GuidValidator]
            public string UserId { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                var userGuid = Guid.Parse(UserId);
                var roleGuid = Guid.Parse(RoleId);
                var tenantGuid = Guid.Parse(TenantId);

                var userRoleAddResult = await Shared.GetUserTenantManagementApi(console).AddUserToRole(userGuid, tenantGuid, roleGuid);
                userRoleAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }

        [Command("delete", Description = "Delete user", AllowArgumentSeparator = true, UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
        private class Delete
        {
            [Required(ErrorMessage = "You must specify the user ID")]
            [Option(Description = "User to delete")]
            [GuidValidator]
            public string Id { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                var userId = Guid.Parse(Id);
                var result = await Shared.GetUserTenantManagementApi(console).DeleteUser(userId);
                result.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }

        [Command(Description = "List all users"), HelpOption]
        private class List
        {
            [Option(Description = "Show only user with specified ID")]
            [GuidValidator]
            public string Id { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                IEnumerable<UserDto> users;

                if (!string.IsNullOrEmpty(Id))
                {
                    var userId = Guid.Parse(Id);

                    var result = await Shared.GetUserTenantManagementApi(console).GetUserById(userId);
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    users = result.Data;
                }
                else
                {
                    users = (await Shared.GetUserTenantManagementApi(console).GetAllUsers()).Data;
                }

                ConsoleOutputUsers(console, users);
            }
        }

        [Command(Description = "List all users per role"), HelpOption]
        private class ListPerRole
        {
            [Option(Description = "Filter per role ID")]
            [GuidValidator]
            public string RoleId { get; }

            [Option(Description = "Filter per tenant ID")]
            [GuidValidator]
            public string TenantId { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                IEnumerable<RoleDto> roles;
                IEnumerable<TenantDto> tenants;

                if (!string.IsNullOrEmpty(RoleId))
                {
                    var roleId = Guid.Parse(RoleId);
                    var roleResult = await Shared.GetRoleManagementApi(console).GetRoleById(roleId);

                    if (!roleResult.Success)
                    {
                        roleResult.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    roles = roleResult.Data;
                }
                else
                {
                    var rolesList = (await Shared.GetRoleManagementApi(console).GetRoles()).Data;
                    roles = rolesList;
                }

                if (!string.IsNullOrEmpty(TenantId))
                {
                    var tenantId = Guid.Parse(TenantId);

                    var tenantResult = await Shared.GetTenantManagementApi(console).GetTenant(tenantId);

                    if (!tenantResult.Success)
                    {
                        tenantResult.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    tenants = tenantResult.Data;
                }
                else
                {
                    tenants = (await Shared.GetTenantManagementApi(console).GetTenants()).Data;
                }

                foreach (var tenant in tenants)
                {
                    console.WriteLine($"TENANT: {tenant.Name} (ID: {tenant.TenantId})");
                    console.WriteLine("--------------------------------------");

                    foreach (var role in roles)
                    {
                        console.WriteLine($"ROLE: {role.Name} (ID: {role.Id})");

                        var usersResult = await Shared.GetUserTenantManagementApi(console).Search(new UsersTenantSearch(tenant.TenantId, role.Id));

                        if (!usersResult.Success)
                        {
                            usersResult.ToConsoleResult().WriteMessages(console);
                            return;
                        }

                        foreach (var user in usersResult.Data)
                        {
                            console.WriteLine($"{user.Id} - {user.Username}");
                        }

                        if (!usersResult.Data.Any())
                            console.WriteLine($"No user in role for tenant {tenant.TenantId}");

                        console.WriteLine("");
                    }

                    console.WriteLine("");
                    console.WriteLine("");
                }
            }
        }

        [Command(Description = "Remove user from role"), HelpOption]
        private class RemoveFromRole
        {
            [Required(ErrorMessage = "Must specify role ID")]
            [Option(Description = "Role Id")]
            [GuidValidator]
            public string RoleId { get; }

            [Required(ErrorMessage = "Must specify tenantId")]
            [Option(Description = "Tenant Id")]
            [GuidValidator]
            public string TenantId { get; }

            [Required(ErrorMessage = "Must specify user ID")]
            [Option(Description = "User Id")]
            [GuidValidator]
            public string UserId { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                var userGuid = Guid.Parse(UserId);
                var roleGuid = Guid.Parse(RoleId);
                var tenantGuid = Guid.Parse(TenantId);

                var userRoleRemoveResult = await Shared.GetUserTenantManagementApi(console).RemoveUserFromRole(userGuid, tenantGuid, roleGuid);
                userRoleRemoveResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }

        [Command(Description = "Update user"), HelpOption]
        private class Update
        {
            [Required(ErrorMessage = "Must specify user e-mail")]
            [Option(Description = "Email")]
            public string Email { get; }

            [Required(ErrorMessage = "Must specify ID to update")]
            [GuidValidator]
            [Option(Description = "User ID")]
            public string Id { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                var userId = Guid.Parse(Id);

                var userResult = await Shared.GetUserTenantManagementApi(console).GetUserById(userId);
                if (!userResult.Success)
                {
                    userResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                    return;
                }

                var user = userResult.Data.First();

                if (!string.IsNullOrEmpty(Email))
                    user.Email = Email;

                var userUpdateResult = await Shared.GetUserTenantManagementApi(console).UpdateUser(user);

                userUpdateResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);

                if (userUpdateResult.Success)
                    ConsoleOutputUsers(console, userUpdateResult.Data.First());
            }
        }
    }
}