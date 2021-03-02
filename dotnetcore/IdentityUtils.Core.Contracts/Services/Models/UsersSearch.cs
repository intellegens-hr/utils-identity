using System;

namespace IdentityUtils.Core.Contracts.Services.Models
{
    public class UsersSearch
    {
        public UsersSearch(Guid? roleId = null, string username = null)
        {
            RoleId = roleId;
            Username = username;
        }

        public Guid? RoleId { get; set; }
        public string? Username { get; set; }
    }
}