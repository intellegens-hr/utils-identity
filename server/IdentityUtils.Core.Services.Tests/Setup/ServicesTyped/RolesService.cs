using AutoMapper;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    internal class RolesService : IdentityManagerRolesService<IdentityManagerUser, IdentityManagerRole, RoleDto>
    {
        public RolesService(RoleManager<IdentityManagerRole> roleManager, IMapper mapper) : base(roleManager, mapper)
        {
        }
    }
}