using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    public class UsersTenantService : IdentityManagerUserTenantService<UserDb, UserDto, RoleDb>
    {
        public UsersTenantService(UserManager<UserDb> userManager, RoleManager<RoleDb> roleManager, IIdentityManagerUserContext<UserDb> dbContext, IMapper mapper) : base(userManager, roleManager, dbContext, mapper)
        {
        }
    }
}