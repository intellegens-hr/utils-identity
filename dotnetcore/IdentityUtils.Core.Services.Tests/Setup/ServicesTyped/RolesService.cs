using AutoMapper;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    internal class RolesService : IdentityManagerRolesService<RoleDb, RoleDto>
    {
        public RolesService(RoleManager<RoleDb> roleManager, IMapper mapper) : base(roleManager, mapper)
        {
        }
    }
}