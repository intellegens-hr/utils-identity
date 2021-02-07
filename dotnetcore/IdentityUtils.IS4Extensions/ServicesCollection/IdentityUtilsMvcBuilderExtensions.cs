using IdentityUtils.Api.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IdentityUtils.IS4Extensions.ServicesCollection
{
    public static class IdentityUtilsMvcBuilderExtensions
    {
        private static Assembly controllersAssembly = typeof(AuthenticationControllerApi).Assembly;

        public static IMvcBuilder AddIdentityUtilsAuthenticationControllerAssemblyPart(this IMvcBuilder builder)
        {
            builder.RemoveIdentityUtilsAuthenticationControllerAssemblyPart();
            builder.AddApplicationPart(controllersAssembly);

            return builder;
        }

        public static IMvcCoreBuilder AddIdentityUtilsAuthenticationControllerAssemblyPart(this IMvcCoreBuilder builder)
        {
            builder.RemoveIdentityUtilsAuthenticationControllerAssemblyPart();
            builder.AddApplicationPart(controllersAssembly);

            return builder;
        }

        public static IMvcBuilder RemoveIdentityUtilsAuthenticationControllerAssemblyPart(this IMvcBuilder builder)
        {
            builder.ConfigureApplicationPartManager(x =>
            {
                x.ApplicationParts.RemoveAuthenticationControllerFromAppParts();
            });

            return builder;
        }

        public static IMvcCoreBuilder RemoveIdentityUtilsAuthenticationControllerAssemblyPart(this IMvcCoreBuilder builder)
        {
            builder.ConfigureApplicationPartManager(x =>
            {
                x.ApplicationParts.RemoveAuthenticationControllerFromAppParts();
            });

            return builder;
        }

        private static void RemoveAuthenticationControllerFromAppParts(this IList<ApplicationPart> parts)
        {
            var part = parts
                .Where(x => x.GetType() == typeof(AssemblyPart))
                .Select(x => (AssemblyPart)x)
                .Where(x => x.Assembly.FullName == controllersAssembly.FullName)
                .FirstOrDefault();

            if (part != null)
                parts.Remove(part);
        }
    }
}