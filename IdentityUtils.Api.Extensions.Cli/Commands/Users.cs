﻿using IdentityManagement.Models.ModelsDto;
using IdentityUtils.Api.Extensions.Cli.Commons;
using IdentityUtils.Api.Extensions.Cli.Models;
using IdentityUtils.Api.Extensions.Cli.Utils;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Api.Extensions.Cli.Commands
{
    [Command("users", Description = "Manage users"),
        Subcommand(typeof(List)),
        Subcommand(typeof(ListPerRole)),
        Subcommand(typeof(Delete)),
        Subcommand(typeof(Add)),
        Subcommand(typeof(AddToRole)),
        Subcommand(typeof(RemoveFromRole)),
        Subcommand(typeof(Update))]
    internal class Users
    {
        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }

        private static void ConsoleOutputUsers(IConsole console, UserDto user)
            => ConsoleOutputUsers(console, new List<UserDto> { user });

        private static void ConsoleOutputUsers(IConsole console, List<UserDto> users)
        {
            console.WriteLine("\tID\t\tUSERNAME\t\tEMAIL\t\tADDITIONAL DATA");
            console.WriteLine("--------------------------------------------");
            foreach (var user in users)
            {
                console.WriteLine($"{user.Id}\t\t{user.UserName}\t\t{user.Email}\t\t{user.AdditionalDataJson}");
            }
        }

        [Command(Description = "List all users"), HelpOption]
        private class List
        {
            [Option(Description = "Show only user with specified ID")]
            [GuidValidator]
            public string Id { get; }

            private void OnExecute(IConsole console)
            {
                var users = new List<UserDto>();

                if (!string.IsNullOrEmpty(Id))
                {
                    var userId = Guid.Parse(Id);

                    var result = Shared.GetUserManagementApi(console).GetUserById(userId).Result;
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    users.Add(result.Payload);
                }
                else
                {
                    users.AddRange(Shared.GetUserManagementApi(console).GetAllUsers().Result);
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

            private void OnExecute(IConsole console)
            {
                var roles = new List<RoleDto>();
                var tenants = new List<TenantDto>();

                if (!string.IsNullOrEmpty(RoleId))
                {
                    var roleId = Guid.Parse(RoleId);
                    var roleResult = Shared.GetRoleManagementApi(console).GetRoleById(roleId).Result;

                    if (!roleResult.Success)
                    {
                        roleResult.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    roles.Add(roleResult.Payload);
                }
                else
                {
                    var rolesList = Shared.GetRoleManagementApi(console).GetRoles().Result;
                    roles.AddRange(rolesList);
                }

                if (!string.IsNullOrEmpty(TenantId))
                {
                    var tenantId = Guid.Parse(TenantId);

                    var tenantResult = Shared.GetTenantManagementApi(console).GetTenant(tenantId).Result;

                    if (!tenantResult.Success)
                    {
                        tenantResult.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    tenants.Add(tenantResult.Payload);
                }
                else
                {
                    tenants.AddRange(Shared.GetTenantManagementApi(console).GetTenants().Result);
                }

                foreach (var tenant in tenants)
                {
                    console.WriteLine($"TENANT: {tenant.Name} (ID: {tenant.TenantId})");
                    console.WriteLine("--------------------------------------");

                    foreach (var role in roles)
                    {
                        console.WriteLine($"ROLE: {role.Name} (ID: {role.Id})");

                        var usersResult = Shared.GetUserManagementApi(console).RoleUsersPerTenant(role.Id, tenant.TenantId).Result;

                        if (!usersResult.Success)
                        {
                            usersResult.ToConsoleResult().WriteMessages(console);
                            return;
                        }

                        foreach (var user in usersResult.Payload)
                        {
                            console.WriteLine($"{user.Id} - {user.UserName}");
                        }

                        if (usersResult.Payload.Count == 0)
                            console.WriteLine($"No user in role for tenant {tenant.TenantId}");

                        console.WriteLine("");
                    }

                    console.WriteLine("");
                    console.WriteLine("");
                }
            }
        }

        [Command(Description = "Add user"), HelpOption]
        private class Add
        {
            [Required(ErrorMessage = "Must specify username")]
            [Option(Description = "Username")]
            public string Username { get; }

            [Required(ErrorMessage = "Must specify user e-mail")]
            [Option(Description = "Email")]
            public string Email { get; }

            [Required(ErrorMessage = "Must specify user password")]
            [Option(Description = "Password")]
            public string Password { get; }

            private void OnExecute(IConsole console)
            {
                UserDto user = new UserDto
                {
                    UserName = Username,
                    Email = Email,
                    Password = Password
                };

                var userAddResult = Shared.GetUserManagementApi(console).CreateUser(user).Result;

                userAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                ConsoleOutputUsers(console, userAddResult.Payload);
            }
        }

        [Command(Description = "Update user"), HelpOption]
        private class Update
        {
            [Required(ErrorMessage = "Must specify ID to update")]
            [GuidValidator]
            [Option(Description = "User ID")]
            public string Id { get; }

            [Required(ErrorMessage = "Must specify user e-mail")]
            [Option(Description = "Email")]
            public string Email { get; }

            private void OnExecute(IConsole console)
            {
                var userId = Guid.Parse(Id);

                var userResult = Shared.GetUserManagementApi(console).GetUserById(userId).Result;
                if (!userResult.Success)
                {
                    userResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                    return;
                }

                var user = userResult.Payload;

                if (!string.IsNullOrEmpty(Email))
                    user.Email = Email;

                var userUpdateResult = Shared.GetUserManagementApi(console).UpdateUser(user).Result;

                userUpdateResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);

                if (userUpdateResult.Success)
                    ConsoleOutputUsers(console, userUpdateResult.Payload);
            }
        }

        [Command("delete", Description = "Delete user", AllowArgumentSeparator = true, UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
        private class Delete
        {
            [Required(ErrorMessage = "You must specify the user ID")]
            [Option(Description = "User to delete")]
            [GuidValidator]
            public string Id { get; }

            private void OnExecute(IConsole console)
            {
                var userId = Guid.Parse(Id);
                var result = Shared.GetUserManagementApi(console).DeleteUser(userId).Result;
                result.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }

        [Command(Description = "Add user to role"), HelpOption]
        private class AddToRole
        {
            [Required(ErrorMessage = "Must specify user ID")]
            [Option(Description = "User Id")]
            [GuidValidator]
            public string UserId { get; }

            [Required(ErrorMessage = "Must specify role ID")]
            [Option(Description = "Role Id")]
            [GuidValidator]
            public string RoleId { get; }

            [Required(ErrorMessage = "Must specify tenantId")]
            [Option(Description = "Tenant Id")]
            [GuidValidator]
            public string TenantId { get; }

            private void OnExecute(IConsole console)
            {
                var userGuid = Guid.Parse(UserId);
                var roleGuid = Guid.Parse(RoleId);
                var tenantGuid = Guid.Parse(TenantId);

                var userRoleAddResult = Shared.GetUserManagementApi(console).AddToRole(userGuid, roleGuid, tenantGuid).Result;
                userRoleAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }

        [Command(Description = "Remove user from role"), HelpOption]
        private class RemoveFromRole
        {
            [Required(ErrorMessage = "Must specify user ID")]
            [Option(Description = "User Id")]
            [GuidValidator]
            public string UserId { get; }

            [Required(ErrorMessage = "Must specify role ID")]
            [Option(Description = "Role Id")]
            [GuidValidator]
            public string RoleId { get; }

            [Required(ErrorMessage = "Must specify tenantId")]
            [Option(Description = "Tenant Id")]
            [GuidValidator]
            public string TenantId { get; }

            private void OnExecute(IConsole console)
            {
                var userGuid = Guid.Parse(UserId);
                var roleGuid = Guid.Parse(RoleId);
                var tenantGuid = Guid.Parse(TenantId);

                var userRoleRemoveResult = Shared.GetUserManagementApi(console).RemoveFromRole(userGuid, roleGuid, tenantGuid).Result;
                userRoleRemoveResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }
    }
}