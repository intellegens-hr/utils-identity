using System;

namespace IdentityUtils.Core.Contracts.Services.Models
{
    public class RoleBasicData: IEquatable<RoleBasicData>
    {
        public RoleBasicData()
        {
        }

        public RoleBasicData(Guid id, string normalizedName)
        {
            Id = id;
            NormalizedName = normalizedName;
        }

        public Guid Id { get; set; }
        public string NormalizedName { get; set; }

        public bool Equals(RoleBasicData other)
        => Id == other.Id && NormalizedName == other.NormalizedName;

        public override int GetHashCode()
        => $"{Id}_{NormalizedName}".GetHashCode();
    }
}