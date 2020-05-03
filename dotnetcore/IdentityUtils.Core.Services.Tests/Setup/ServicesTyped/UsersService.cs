using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    public class UsersService : IdentityManagerUserService<UserDb, UserDto, RoleDb>
    {
        public UsersService(UserManager<UserDb> userManager, RoleManager<RoleDb> roleManager, IIdentityManagerUserContext<UserDb> dbContext, IMapper mapper) : base(userManager, roleManager, dbContext, mapper)
        {
        }
    }
}