using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityUtils.IS4Extensions.IdentityServerBuilder
{
    public static class IdentityServerDefaultConfig
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("demo-core-api", "Demo API which will be consumed by client apps")
                {
                    UserClaims = new List<string>{"address", "email", "username", "role"}
                },
                new ApiScope("demo-is4-management-api", "IS4 management API")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "is4management",
                    ClientName = "Identity server 4 management services for IS4",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                    AllowedScopes = { "openid", "profile", "demo-is4-management-api" }
                },

                new Client
                {
                    ClientId = "jsapp",
                    ClientName = "Javascript App",
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    AllowedGrantTypes = new List<string>{
                        GrantType.AuthorizationCode,
                        GrantType.ResourceOwnerPassword,
                        },
                    Enabled = true,
                    RequireConsent = false,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RedirectUris = {
                        "https://127.0.0.1"
                    },

                    AllowedScopes = { "openid", "profile", "demo-core-api"},
                }
        };

        public static IEnumerable<IdentityResource> Ids =>
        new IdentityResource[]
        {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile(),
        };
    }
}