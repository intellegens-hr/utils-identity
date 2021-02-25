using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace IdentityUtils.IS4Extensions.ServicesCollection
{
    public static class IdentityUtilsServicesBuilderCommon
    {
        public static IServiceCollection AddAuthentication(
            this IServiceCollection services,
            string authority,
            string managementApiAudience,
            Action<JwtBearerOptions> optionsCallback = null)
        {
            Action<JwtBearerOptions> callback = (JwtBearerOptions options) =>
            {
                options.Authority = authority;
                options.Audience = managementApiAudience;

                if (optionsCallback != null)
                    optionsCallback(options);
            };

            services.AddAuthentication(callback);

            return services;
        }

        public static IServiceCollection AddAuthentication(
                    this IServiceCollection services,
                    Action<JwtBearerOptions> optionsCallback)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };

                    options.RequireHttpsMetadata = false;
                    optionsCallback(options);
                });

            return services;
        }
    }
}