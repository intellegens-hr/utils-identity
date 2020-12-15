using Microsoft.Extensions.DependencyInjection;
using System;

namespace IdentityUtils.IS4Extensions.ServicesCollection
{
    public static class IdentityUtilsServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityUtilsIs4Extensions(this IServiceCollection services, Action<IdentityUtilsServicesBuilder> builder)
        {
            var identityUtilsServicesBuilder = new IdentityUtilsServicesBuilder(services);
            builder(identityUtilsServicesBuilder);
            return services;
        }

        public static IServiceCollection AddIdentityUtilsIs4MultitenancyExtensions(this IServiceCollection services, Action<IdentityUtilsServicesBuilderMultitenant> builder)
        {
            var identityUtilsServicesBuilder = new IdentityUtilsServicesBuilderMultitenant(services);
            builder(identityUtilsServicesBuilder);
            return services;
        }
    }
}