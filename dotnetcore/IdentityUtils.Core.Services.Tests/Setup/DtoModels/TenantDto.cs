using IdentityUtils.Core.Contracts.Tenants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class TenantDto : IIdentityManagerTenantDto, IEquatable<TenantDto>
    {
        public ICollection<string> Hostnames { get; set; } = new List<string>();
        public string Name { get; set; }
        public Guid TenantId { get; set; }

        public bool Equals(TenantDto other)
        {
            return TenantId == other.TenantId
                && Name == other.Name
                && Enumerable.SequenceEqual(Hostnames.OrderBy(x => x), other.Hostnames.OrderBy(x => x));
        }
    }
}