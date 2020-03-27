using IdentityUtils.Core.Services.Tests.Setup;
using System;
using Xunit;

namespace IdentityUtils.Core.Services.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var serviceRoles = ServicesFactory.RolesService;
            var serviceTenants = ServicesFactory.TenantService;
            var serviceUsers = ServicesFactory.UserService;
        }
    }
}
