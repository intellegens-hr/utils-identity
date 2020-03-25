using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace IdentityUtils.Core.Contracts.Roles
{
    /// <summary>
    /// Database model for identity role.
    /// </summary>
    public class IdentityManagerRole : IdentityRole<Guid>
    {
        public IdentityManagerRole()
        {
        }

        public IdentityManagerRole(string roleName) : base(roleName)
        {
        }

        public override Guid Id { get; set; } = new SequentialGuidValueGenerator().Next(null);
    }
}