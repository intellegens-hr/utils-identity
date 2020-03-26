using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityUtils.Core.Contracts.Tenants
{
    /// <summary>
    /// Default tenant database model
    /// </summary>
    [Table("Tenants")]
    public class IdentityManagerTenant
    {
        [Key]
        public virtual Guid TenantId { get; set; } = new SequentialGuidValueGenerator().Next(null);

        [Required, StringLength(128, MinimumLength = 6)]
        public virtual string Name { get; set; }

        [InverseProperty(nameof(IdentityManagerTenant))]
        public virtual ICollection<IdentityManagerTenantHost> Hosts { get;set;}
    }
}