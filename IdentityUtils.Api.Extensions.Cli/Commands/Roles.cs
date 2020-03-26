using IdentityUtils.Api.Extensions.Cli.Commons;
using IdentityUtils.Api.Extensions.Cli.Models;
using IdentityUtils.Api.Extensions.Cli.Utils;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Api.Extensions.Cli.Commands
{
    [Command("roles", Description = "Manage tenants"),
        Subcommand(typeof(List)),
        Subcommand(typeof(Delete)),
        Subcommand(typeof(Add))
        ]
    internal class Roles
    {
        [Option(ShortName = "r", LongName = "api-base-route", Description = "Base route roles management API uses (defaults to /api/management/roles)")]
        internal static string ApiBaseRoute { get; set; }

        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }

        private static void ConsoleOutputRoles(IConsole console, RoleDto role)
            => ConsoleOutputRoles(console, new List<RoleDto> { role });

        private static void ConsoleOutputRoles(IConsole console, List<RoleDto> roles)
        {
            console.WriteLine("\tID\t\t\tNAME\t\tNORMALIZED NAME");
            console.WriteLine("--------------------------------------------");
            foreach (var role in roles)
            {
                console.WriteLine($"{role.Id}\t\t{role.Name}\t\t{role.NormalizedName}");
            }
        }

        [Command(Description = "List all roles"), HelpOption]
        private class List
        {
            [Option(Description = "Show only role with specified ID")]
            [GuidValidator]
            public string Id { get; }

            [Option(Description = "Show only role with specified normalized name")]
            public string Name { get; }

            private void OnExecute(IConsole console)
            {
                var roles = new List<RoleDto>();

                if (!string.IsNullOrEmpty(Id))
                {
                    var roleId = Guid.Parse(Id);

                    var result = Shared.GetRoleManagementApi(console).GetRoleById(roleId).Result;
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    roles.Add(result.Payload);
                }
                else if (!string.IsNullOrEmpty(Name))
                {
                    var result = Shared.GetRoleManagementApi(console).GetRoleByNormalizedName(Name).Result;
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    roles.Add(result.Payload);
                }
                else
                {
                    roles.AddRange(Shared.GetRoleManagementApi(console).GetRoles().Result);
                }

                ConsoleOutputRoles(console, roles);
            }
        }

        [Command(Description = "Add role"), HelpOption]
        private class Add
        {
            [Required(ErrorMessage = "Must specify role name")]
            [Option(Description = "Role name")]
            public string Name { get; }

            private void OnExecute(IConsole console)
            {
                RoleDto role = new RoleDto
                {
                    Name = Name
                };

                var roleAddResult = Shared.GetRoleManagementApi(console).AddRole(role).Result;

                roleAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                ConsoleOutputRoles(console, roleAddResult.Payload);
            }
        }

        [Command("delete", Description = "Delete role")]
        private class Delete
        {
            [Required(ErrorMessage = "You must specify role ID"), GuidValidator]
            [Option(Description = "Role to delete")]
            public string Id { get; }

            private void OnExecute(IConsole console)
            {
                var roleId = Guid.Parse(Id);
                var result = Shared.GetRoleManagementApi(console).DeleteRole(roleId).Result;
                result.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }
    }
}