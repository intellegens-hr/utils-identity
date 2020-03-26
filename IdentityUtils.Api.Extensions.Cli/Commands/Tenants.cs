using IdentityUtils.Api.Extensions.Cli.Commons;
using IdentityUtils.Api.Extensions.Cli.Models;
using IdentityUtils.Api.Extensions.Cli.Utils;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }

        private static void ConsoleOutputTenants(IConsole console, TenantDto tenant)
            => ConsoleOutputTenants(console, new List<TenantDto> { tenant });

        private static void ConsoleOutputTenants(IConsole console, List<TenantDto> tenants)
        {
            console.WriteLine("\tID\t\t\tNAME\t\tHOSTNAME");
            console.WriteLine("--------------------------------------------");
            foreach (var tenant in tenants)
            {
                console.WriteLine($"{tenant.TenantId}\t\t{tenant.Name}\t\t{tenant.Hostname}");
            }
        }

        [Command(Description = "List all tenants"), HelpOption]
        private class List
        {
            [Option(Description = "Show only tenant with specified ID")]
            [GuidValidator]
            public string Id { get; }

            private void OnExecute(IConsole console)
            {
                var tenants = new List<TenantDto>();

                if (!string.IsNullOrEmpty(Id))
                {
                    var tenantId = Guid.Parse(Id);

                    var result = Shared.GetTenantManagementApi(console).GetTenant(tenantId).Result;
                    if (!result.Success)
                    {
                        result.ToConsoleResult().WriteMessages(console);
                        return;
                    }

                    tenants.Add(result.Payload);
                }
                else
                {
                    tenants.AddRange(Shared.GetTenantManagementApi(console).GetTenants().Result);
                }

                ConsoleOutputTenants(console, tenants);
            }
        }

        [Command(Description = "Add tenant"), HelpOption]
        private class Add
        {
            [Required(ErrorMessage = "Must specify tenant name")]
            [Option(Description = "Tenant name")]
            public string Name { get; }

            [Required(ErrorMessage = "Must specify tenant host")]
            [Option(Description = "Hostname")]
            public string Hostname { get; }

            private void OnExecute(IConsole console)
            {
                TenantDto tenant = new TenantDto
                {
                    Name = Name,
                    Hostname = Hostname
                };

                var tenantAddResult = Shared.GetTenantManagementApi(console).AddTenant(tenant).Result;

                tenantAddResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                ConsoleOutputTenants(console, tenantAddResult.Payload);
            }
        }

        [Command(Description = "Update tenant"), HelpOption]
        private class Update
        {
            [Option(Description = "Tenant ID")]
            [Required, GuidValidator]
            public string Id { get; }

            [Option(Description = "Tenant name")]
            public string Name { get; }

            [Option(Description = "Hostname")]
            public string Hostname { get; }

            private void OnExecute(IConsole console)
            {
                var tenantId = Guid.Parse(Id);

                var tenantResult = Shared.GetTenantManagementApi(console).GetTenant(tenantId).Result;
                if (!tenantResult.Success)
                {
                    tenantResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);
                    return;
                }

                var tenant = tenantResult.Payload;

                if (!string.IsNullOrEmpty(Name))
                    tenant.Name = Name;

                if (!string.IsNullOrEmpty(Hostname))
                    tenant.Hostname = Hostname;

                var tenantUpdateResult = Shared.GetTenantManagementApi(console).UpdateTenant(tenant).Result;

                tenantUpdateResult.ToConsoleResultWithDefaultMessages().WriteMessages(console);

                if (tenantUpdateResult.Success)
                    ConsoleOutputTenants(console, tenantUpdateResult.Payload);
            }
        }

        [Command("delete", Description = "Delete tenant", AllowArgumentSeparator = true, UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect)]
        private class Delete
        {
            [Required(ErrorMessage = "You must specify the tenant ID")]
            [GuidValidator]
            [Option(Description = "Tenant to delete")]
            public string Id { get; }

            private void OnExecute(IConsole console)
            {
                var tenantId = Guid.Parse(Id);

                var result = Shared.GetTenantManagementApi(console).DeleteTenant(tenantId).Result;
                result.ToConsoleResultWithDefaultMessages().WriteMessages(console);
            }
        }
    }
}