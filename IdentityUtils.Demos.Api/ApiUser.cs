using IdentityUtils.Api.Extensions;
using IdentityUtils.Core.Contracts.Claims;
using IdentityUtils.Demos.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Demos.Api
{
    public class ApiUser
    {
        private readonly HttpContext httpContext;
        private readonly TenantManagementApi<TenantDto> tenantManagementApi;
        private readonly IMemoryCache memoryCache;

        /// <summary>
        /// All calls to this API are cross-domain and should contain Origin header
        /// Tenant ID is found by using client hostname
        /// </summary>
        /// <returns></returns>
        private Guid GetTenantIdByHostname()
        {
            var originHost = httpContext.Request.Headers.First(x => x.Key == "Origin").Value;

            return memoryCache.GetOrCreate(originHost, (entry) =>
            {
                entry.SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(5));

                var tenant = tenantManagementApi.GetTenantByHostname(originHost).Result;
                return tenant.Data.TenantId;
            });
        }

        public ApiUser(
            IHttpContextAccessor httpContextAccessor,
            TenantManagementApi<TenantDto> tenantManagementApi,
            IMemoryCache memoryCache
            )
        {
            this.httpContext = httpContextAccessor.HttpContext;
            this.tenantManagementApi = tenantManagementApi;
            this.memoryCache = memoryCache;

            IsAuthenticated = httpContext.User.Identity.IsAuthenticated;

            if (IsAuthenticated)
            {
                var tenantId = GetTenantIdByHostname();

                //parse claims list
                var claims = httpContext
                    .User
                    .Claims
                    .Select(x => new { x.Type, x.Value })
                    .ToList();

                UserId = Guid.Parse(claims.First(x => x.Type == "userId").Value);
                TenantId = tenantId;
                TenantRoles = claims
                    .Where(x => x.Type == TenantClaimsSchema.TenantRolesData)
                    .Select(x => x.Value.DeserializeToTenantRolesClaimData())
                    .ToList();

                //Extract roles for current tenant
                Roles = TenantRoles.First(x => x.TenantId == tenantId).Roles;
            };
            
        }

        public bool IsAuthenticated { get; set; }
        public Guid TenantId { get; }
        public Guid UserId { get; }
        public List<string> Roles { get; set; }
        public List<TenantRolesClaimData> TenantRoles { get; set; }
    }
}