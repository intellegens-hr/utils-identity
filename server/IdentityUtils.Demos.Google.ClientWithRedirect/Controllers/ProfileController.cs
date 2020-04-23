using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityUtils.Demos.Google.ClientWithRedirect.Controllers
{
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly HttpContext httpContext;
        public ProfileController(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }

        [HttpGet("/api/profile")]
        public JsonResult Get()
        {
            var user = httpContext.User.Identity as ClaimsIdentity;
            return new JsonResult(user.Claims.Select(x => new { x.Type, x.Value }).ToList());
        }
    }
}
