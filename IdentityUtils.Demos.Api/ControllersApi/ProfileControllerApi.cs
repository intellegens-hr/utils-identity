using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUtils.Demos.Api.ControllersApi
{
    [Route("api/profile")]
    [Authorize]
    public class ProfileControllerApi : ControllerBase
    {
    }
}