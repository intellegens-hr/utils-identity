using IdentityUtils.Core.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace IdentityUtils.Core.Contracts.Context
{
    /// <summary>
    /// User management context
    /// </summary>
    /// <typeparam name="TUser">User type which inherits IdentityManagerUser</typeparam>
    public interface IIdentityManagerUserContext<TUser> : IDbContextCommon
        where TUser : IdentityManagerUser
    {
        DbSet<TUser> Users { get; set; }
        DbSet<IdentityUserClaim<Guid>> UserClaims { get; set; }
    }
}