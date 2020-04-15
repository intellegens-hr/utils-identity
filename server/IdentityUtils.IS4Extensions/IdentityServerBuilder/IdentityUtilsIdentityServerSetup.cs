using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityUtils.IS4Extensions.IdentityServerBuilder
{
    public static class IdentityUtilsIdentityServerSetup
    {
        public static IIdentityServerBuilder LoadIdentityUtilsIdentityServerSettings(this IIdentityServerBuilder identityServerBuilder, Action<IdentityUtilsIdentityServerBuilder> builder)
        {
            var identityUtilsIdentityServerBuilder = new IdentityUtilsIdentityServerBuilder(identityServerBuilder);
            builder(identityUtilsIdentityServerBuilder);

            return identityServerBuilder;
        }
    }
}