using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace IdentityUtils.Core.Contracts.Users
{
    public class IdentityManagerUser : IdentityUser<Guid>
    {
        public override Guid Id { get; set; } = new SequentialGuidValueGenerator().Next(null);

        public string AdditionalDataJson { get; set; }
    }
}