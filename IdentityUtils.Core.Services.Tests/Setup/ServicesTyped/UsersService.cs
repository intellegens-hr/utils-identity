using AutoMapper;
using IdentityUtils.Core.Contracts.Context;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services.Tests.Setup.DtoModels;
using Microsoft.AspNetCore.Identity;

namespace IdentityUtils.Core.Services.Tests.Setup.ServicesTyped
{
    public class UsersService : IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole>
    {
        public UsersService(UserManager<IdentityManagerUser> userManager, RoleManager<IdentityManagerRole> roleManager, IIdentityManagerUserContext<IdentityManagerUser> dbContext, IMapper mapper) : base(userManager, roleManager, dbContext, mapper)
        {
        }
    }
}