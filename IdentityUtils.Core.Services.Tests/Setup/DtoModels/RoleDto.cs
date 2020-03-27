using IdentityUtils.Core.Contracts.Roles;
using System;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class RoleDto : IIdentityManagerRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
    }
}