using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityUtils.Core.Contracts.Tenants
{
    /// <summary>
    /// Default tenant database model
    /// </summary>
    [Table("TenantHosts")]
    public class IdentityManagerTenantHost
    {
        [Key]
        public Guid TenantHostId { get; set; } = new SequentialGuidValueGenerator().Next(null);

        [ForeignKey(nameof(IdentityManagerTenant))]
        public Guid TenantId { get; set; }

        [Required, StringLength(256, MinimumLength = 6)]
        public string Hostname { get; set; }

        public virtual IdentityManagerTenant IdentityManagerTenant { get; set; }
    }
}