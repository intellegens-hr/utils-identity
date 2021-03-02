using IdentityUtils.Api.Extensions.Cli.Commons;
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
    [Command("roles", Description = "Manage tenants"),
        Subcommand(typeof(List)),
        Subcommand(typeof(Delete)),
        Subcommand(typeof(Add))
        ]
    internal class Roles
    {
        [Option(ShortName = "r", LongName = "api-base-route", Description = "Base route roles management API uses (defaults to /api/management/roles)")]
        internal static string ApiBaseRoute { get; set; }

        private static void ConsoleOutputRoles(IConsole console, RoleDto role)
            => ConsoleOutputRoles(console, new RoleDto[] { role });

        private static void ConsoleOutputRoles(IConsole console, IEnumerable<RoleDto> roles)
        {
            console.WriteLine("\tID\t\t\tNAME\t\tNORMALIZED NAME");
            console.WriteLine("--------------------------------------------");
            foreach (var role in roles)
            {
                console.WriteLine($"{role.Id}\t\t{role.Name}\t\t{role.NormalizedName}");
            }
        }

        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }

        [Command(Description = "Add role"), HelpOption]
        private class Add
        {
            [Required(ErrorMessage = "Must specify role name")]
            [Option(Description = "Role name")]
            public string Name { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                RoleDto role = new RoleDto
                {
                    Name = Name
                };

                var roleAddResult = await Shared.GetRoleManagementApi(console).AddRole(role);

                roleAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                ConsoleOutputRoles(console, roleAddResult.Data.First());
            }
        }

        [Command("delete", Description = "Delete role")]
        private class Delete
        {
            [Required(ErrorMessage = "You must specify role ID"), GuidValidator]
            [Option(Description = "Role to delete")]
            public string Id { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                var roleId = Guid.Parse(Id);
                var result = await Shared.GetRoleManagementApi(console).DeleteRole(roleId);
                result.ToConsoleResultWithDefaultMessages().WriteMessages(console);
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

            private async Task OnExecuteAsync(IConsole console)
            {
                IEnumerable<RoleDto> roles;

                if (!string.IsNullOrEmpty(Id))
                {
                    var roleId = Guid.Parse(Id);

                    var result = await Shared.GetRoleManagementApi(console).GetRoleById(roleId);
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    roles = result.Data;
                }
                else if (!string.IsNullOrEmpty(Name))
                {
                    var result = await Shared.GetRoleManagementApi(console).Search(new RoleSearch(name: Name));
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    roles = result.Data;
                }
                else
                {
                    roles = (await Shared.GetRoleManagementApi(console).GetRoles()).Data;
                }

                ConsoleOutputRoles(console, roles);
            }
        }
    }
}