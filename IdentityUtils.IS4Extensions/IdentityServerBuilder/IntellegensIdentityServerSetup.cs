using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityUtils.IS4Extensions.IdentityServerBuilder
{
    public static class IntellegensIdentityServerSetup
    {
        public static IIdentityServerBuilder LoadIntellegensIdentityServerSettings(this IIdentityServerBuilder identityServerBuilder, Action<IdentityUtilsIdentityServerBuilder> builder)
        {
            var identityUtilsIdentityServerBuilder = new IdentityUtilsIdentityServerBuilder(identityServerBuilder);
            builder(identityUtilsIdentityServerBuilder);

            return identityServerBuilder;
        }
    }
}