using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class TenantDto : IIdentityManagerTenantDto, IEquatable<TenantDto>
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public List<string> Hostnames { get; set; }

        public bool Equals([AllowNull] TenantDto other)
        {
            return TenantId == other.TenantId
                && Name == other.Name
                && Enumerable.SequenceEqual(Hostnames.OrderBy(x => x), other.Hostnames.OrderBy(x => x));
        }
    }
}