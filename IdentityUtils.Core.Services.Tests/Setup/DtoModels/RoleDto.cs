using IdentityUtils.Core.Contracts.Roles;
using System;
using System.Diagnostics.CodeAnalysis;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class RoleDto : IIdentityManagerRoleDto, IEquatable<RoleDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }

        public bool Equals(RoleDto other)
        {
            return Id == other.Id
                && Name == other.Name
                && NormalizedName == other.NormalizedName;
        }
    }
}