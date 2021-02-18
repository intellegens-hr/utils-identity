using IdentityModel;
using IdentityServer4;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Services;
using IdentityUtils.Core.Contracts.Tenants;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.DbContext;
using IdentityUtils.Demos.IdentityServer4.MultiTenant.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityUtils.Demos.IdentityServer4.MultiTenant
{
    public class SeedData
    {
        private static readonly SequentialGuidValueGenerator guidValueGenerator = new SequentialGuidValueGenerator();

        public static async Task EnsureSeedData(IServiceCollection services)
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            var context = scope.ServiceProvider.GetService<Is4DemoDbContext>();
            context.Database.Migrate();

            await LoadTenants(scope);
            await LoadRoles(scope);
            await LoadUsers(scope);
        }

        private static async Task LoadRoles(IServiceScope scope)
        {
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityManagerRole>>();
            var rolesToCreate = new string[] { "ADMIN", "GODMODE", "READER" };

            foreach (var role in rolesToCreate)
            {
                if (!await roleMgr.RoleExistsAsync(role))
                {
                    var appRole = new IdentityManagerRole(role)
                    {
                        Id = guidValueGenerator.Next(null)
                    };
                    await roleMgr.CreateAsync(appRole);
                }
            }
        }

        private static async Task LoadTenants(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Is4DemoDbContext>();

            var tenants = new IdentityManagerTenant[]
            {
                new IdentityManagerTenant{TenantId = guidValueGenerator.Next(null), Name = "Intellegens Exams"},
                new IdentityManagerTenant{TenantId = guidValueGenerator.Next(null), Name = "Intellegens Administration"},
                new IdentityManagerTenant{TenantId = guidValueGenerator.Next(null), Name = "Cmars Exams"}
            };

            foreach (var tenant in tenants)
            {
                var exists = await dbContext.Tenants.AnyAsync(x => x.Name == tenant.Name);
                if (!exists)
                    dbContext.Tenants.Add(tenant);
            }

            var hosts = new IdentityManagerTenantHost[]
            {
                new IdentityManagerTenantHost { TenantId = tenants[0].TenantId, Hostname = "https://localhost:5020" },
                new IdentityManagerTenantHost { TenantId = tenants[0].TenantId, Hostname = "https://localhost:5010" },
                new IdentityManagerTenantHost { TenantId = tenants[0].TenantId, Hostname = "https://localhost:5015" },
                new IdentityManagerTenantHost { TenantId = tenants[1].TenantId, Hostname = "https://localhost:5011" },
                new IdentityManagerTenantHost { TenantId = tenants[2].TenantId, Hostname = "https://localhost:5012" }
            };

            foreach (var host in hosts)
            {
                var exists = await dbContext.TenantHosts.AnyAsync(x => x.Hostname == host.Hostname);
                if (!exists)
                    dbContext.TenantHosts.Add(host);
            }

            await dbContext.SaveChangesAsync();
        }

        private static async Task LoadUsers(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Is4DemoDbContext>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<IdentityManagerUser>>();
            var tenantUserService = scope.ServiceProvider.GetRequiredService<IIdentityManagerUserTenantService<IdentityManagerUser, UserDto>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityManagerRole>>();

            var tenants = await dbContext.Tenants.ToListAsync();

            var roleAdmin = await roleMgr.FindByNameAsync("ADMIN");
            var roleReader = await roleMgr.FindByNameAsync("READER");
            var roleGodmode = await roleMgr.FindByNameAsync("GODMODE");

            var aliceExists = (await userMgr.FindByNameAsync("alice")) != null;
            if (!aliceExists)
            {
                var alice = new UserDto
                {
                    Id = guidValueGenerator.Next(null),
                    Username = "alice",
                    Password = "Pass123$"
                };
                IdentityUtilsResult result = await tenantUserService.CreateUser(alice);
                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessages.First());
                }

                foreach (var tenant in tenants)
                {
                    var roles = new Guid[] { roleAdmin.Id, roleReader.Id, roleGodmode.Id };
                    result = await tenantUserService.AddToRolesAsync(alice.Id, tenant.TenantId, roles);
                }

                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessages.First());
                }

                result = await tenantUserService.AddClaimsAsync(alice.Id, new Claim[]{
                            new Claim(JwtClaimTypes.Id, Guid.NewGuid().ToString()),
                            new Claim(JwtClaimTypes.Name, "Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json)
                        });
                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessages.First());
                }
            }

            var bobExists = (await userMgr.FindByNameAsync("bob")) != null;
            if (!bobExists)
            {
                var bob = new UserDto
                {
                    Id = guidValueGenerator.Next(null),
                    Username = "bob",
                    Password = "Pass123$"
                };
                IdentityUtilsResult result = await tenantUserService.CreateUser(bob);
                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessages.First());
                }

                foreach (var tenant in tenants)
                {
                    var roles = new Guid[] { roleAdmin.Id, roleReader.Id };
                    result = await tenantUserService.AddToRolesAsync(bob.Id, tenant.TenantId, roles);
                }

                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessages.First());
                }

                result = await tenantUserService.AddClaimsAsync(bob.Id, new Claim[]{
                            new Claim(JwtClaimTypes.Id, Guid.NewGuid().ToString()),
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("location", "somewhere")
                        });
                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessages.First());
                }
            }
        }
    }
}