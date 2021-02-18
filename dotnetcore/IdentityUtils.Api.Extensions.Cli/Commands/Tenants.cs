using IdentityUtils.Api.Extensions.Cli.Commons;
using IdentityUtils.Api.Extensions.Cli.Models;
using IdentityUtils.Api.Extensions.Cli.Utils;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityUtils.Api.Extensions.Cli.Commands
{
    [Command("tenants", Description = "Manage tenants"),
                Subcommand(typeof(List)),
                Subcommand(typeof(Delete)),
                Subcommand(typeof(Add)),
                Subcommand(typeof(Update))]
    internal class Tenants
    {
        [Option(ShortName = "r", LongName = "api-base-route", Description = "Base route tenant management API uses (defaults to /api/management/tenants)")]
        internal static string ApiBaseRoute { get; set; }

        private static void ConsoleOutputTenants(IConsole console, TenantDto tenant)
            => ConsoleOutputTenants(console, new TenantDto[] { tenant });

        private static void ConsoleOutputTenants(IConsole console, IEnumerable<TenantDto> tenants)
        {
            console.WriteLine("\tID\t\t\tNAME\t\tHOSTNAME");
            console.WriteLine("--------------------------------------------");
            foreach (var tenant in tenants)
            {
                console.WriteLine($"{tenant.TenantId}\t\t{tenant.Name}\t\t{string.Join(';', tenant.Hostnames)}");
            }
        }

        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }

        [Command(Description = "Add tenant"), HelpOption]
        private class Add
        {
            [Required(ErrorMessage = "Must specify tenant host")]
            [Option(Description = "Hostnames (separate multiple hosts with ';')")]
            public string Hostnames { get; }

            [Required(ErrorMessage = "Must specify tenant name")]
            [Option(Description = "Tenant name")]
            public string Name { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                TenantDto tenant = new TenantDto
                {
                    Name = Name,
                    Hostnames = Hostnames.Split(';')
                };

                var tenantAddResult = await Shared.GetTenantManagementApi(console).AddTenant(tenant);

                tenantAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                ConsoleOutputTenants(console, tenantAddResult.Data.First());
            }
        }

        [Command("delete", Description = "Delete tenant", AllowArgumentSeparator = true, UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
        private class Delete
        {
            [Required(ErrorMessage = "You must specify the tenant ID")]
            [GuidValidator]
            [Option(Description = "Tenant to delete")]
            public string Id { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                var tenantId = Guid.Parse(Id);

                var result = await Shared.GetTenantManagementApi(console).DeleteTenant(tenantId);
                result.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }

        [Command(Description = "List all tenants"), HelpOption]
        private class List
        {
            [Option(Description = "Show only tenant with specified ID")]
            [GuidValidator]
            public string Id { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                IEnumerable<TenantDto> tenants;

                if (!string.IsNullOrEmpty(Id))
                {
                    var tenantId = Guid.Parse(Id);

                    var result = await Shared.GetTenantManagementApi(console).GetTenant(tenantId);
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    tenants = result.Data;
                }
                else
                {
                    tenants = (await Shared.GetTenantManagementApi(console).GetTenants()).Data;
                }

                ConsoleOutputTenants(console, tenants);
            }
        }

        [Command(Description = "Update tenant"), HelpOption]
        private class Update
        {
            [Option(Description = "Hostnames (separate multiple hosts with ';')")]
            public string Hostnames { get; }

            [Option(Description = "Tenant ID")]
            [Required, GuidValidator]
            public string Id { get; }

            [Option(Description = "Tenant name")]
            public string Name { get; }

            private async Task OnExecuteAsync(IConsole console)
            {
                var tenantId = Guid.Parse(Id);

                var tenantResult = await Shared.GetTenantManagementApi(console).GetTenant(tenantId);
                if (!tenantResult.Success)
                {
                    tenantResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                    return;
                }

                var tenant = tenantResult.Data.First();

                if (!string.IsNullOrEmpty(Name))
                    tenant.Name = Name;

                if (!string.IsNullOrEmpty(Hostnames))
                    tenant.Hostnames = Hostnames.Split(';');

                var tenantUpdateResult = await Shared.GetTenantManagementApi(console).UpdateTenant(tenant);

                tenantUpdateResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);

                if (tenantUpdateResult.Success)
                    ConsoleOutputTenants(console, tenantUpdateResult.Data.First());
            }
        }
    }
}