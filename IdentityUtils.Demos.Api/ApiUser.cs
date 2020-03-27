using IdentityUtils.Api.Extensions;
using IdentityUtils.Core.Contracts.Claims;
using IdentityUtils.Demos.Api.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace IdentityUtils.Demos.Api
{
    public class ApiUser
    {
        public ApiUser(IHttpContextAccessor httpContextAccessor, TenantManagementApi<TenantDto> tenantManagementApi)
        {
            var httpContext = httpContextAccessor.HttpContext;
            IsAuthenticated = httpContext.User.Identity.IsAuthenticated;

            if (IsAuthenticated)
            {
                var originHost = httpContextAccessor.HttpContext.Request.Headers.First(x => x.Key == "Origin").Value;
                var tenant = tenantManagementApi.GetTenantByHostname(originHost).Result;

                var claims = httpContext
                    .User
                    .Claims
                    .Select(x => new
                    {
                        x.Type,
                        x.Value
                    })
                    .ToList();

                UserId = Guid.Parse(claims.First(x => x.Type == "userId").Value);
                //TODO: current tenant
                TenantId = tenant.TenantId;
                TenantRoles = claims.Where(x => x.Type == TenantClaimsSchema.TenantRolesData).Select(x => x.Value.DeserializeToTenantRolesClaimData()).ToList();
                Roles = TenantRoles.First(x => x.TenantId == tenant.TenantId).Roles;
            };
        }

        public bool IsAuthenticated { get; set; }
        public Guid TenantId { get; } = Guid.NewGuid();
        public Guid UserId { get; } = Guid.NewGuid();
        public List<string> Roles { get; set; }
        public List<TenantRolesClaimData> TenantRoles { get; set; }
    }
}