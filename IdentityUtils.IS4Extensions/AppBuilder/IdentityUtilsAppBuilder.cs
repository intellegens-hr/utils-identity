using Microsoft.AspNetCore.Builder;

namespace IdentityUtils.IS4Extensions.AppBuilder
{
    public static class IdentityUtilsAppBuilderSetup
    {
        public static IApplicationBuilder LoadIdentityUtilsSettings(this IApplicationBuilder appBuilder)
        {
            appBuilder.UseAuthentication();
            appBuilder.UseAuthorization();

            return appBuilder;
        }
    }
}