using AutoMapper;
using IdentityUtils.Api.Controllers;
using IdentityUtils.Commons.Mailing;
using IdentityUtils.Core.Contracts.Roles;
using IdentityUtils.Core.Contracts.Users;
using IdentityUtils.Core.Services;
using IdentityUtils.Demos.IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.IdentityServer4.ControllersApi
{
    [Authorize(Policy = "Is4ManagementApi")]
    [Route("/api/management/users")]
    public class UserManagementControllerApi
        : UsersControllerApiAbstract<IdentityManagerUser, IdentityManagerRole, UserDto>
    {
        public UserManagementControllerApi(
            IdentityManagerUserService<IdentityManagerUser, UserDto, IdentityManagerRole> userManager,
            IMailingProvider mailingProvider,
            IMapper mapper)
        : base(userManager, mailingProvider, mapper)
        {
        }
    }
}