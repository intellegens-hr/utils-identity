using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Services.Tests.Setup.DbModels;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    public class UsersService : IdentityManagerUserService<UserDb, RoleDb, UserDto>
    {
        public UsersService(UserManager<UserDb> userManager, RoleManager<RoleDb> roleManager, IdentityManagerDbContext<UserDb, RoleDb> dbContext, IMapper mapper) 
            : base(userManager, dbContext, mapper)
        {
        }
    }
}