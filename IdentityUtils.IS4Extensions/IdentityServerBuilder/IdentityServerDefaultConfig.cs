// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityUtils.IS4Extensions.IdentityServerBuilder
{
    public static class IdentityServerDefaultConfig
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
                {
                    UserClaims = new List<string>{"address", "email"}
                },
                new ApiResource("is4managementapi", "Intellegens IS4 management API")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "is4management",
                    ClientName = "Intellegens management services for IS4",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "openid", "profile", "is4managementapi" }
                },

                new Client
                {
                    ClientId = "jsapp",
                    ClientName = "Javascript App",
                    //AllowedGrantTypes = GrantTypes.Code,

                    // Specifies whether this client can request refresh tokens
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    //AccessTokenType = AccessTokenType.Reference,

                    AllowedGrantTypes = new List<string>{
                        GrantType.AuthorizationCode,
                        GrantType.ResourceOwnerPassword,
                        },
                    Enabled = true,
                    RequireConsent = false,
                    RefreshTokenUsage = TokenUsage.ReUse,

                    // where to redirect to after login
                    RedirectUris = {
                        "https://localhost:5005/is4_callback.html",
                        "https://localhost:5010/index.html"
                        },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = {
                        "https://localhost:5005/logout.html" ,
                        "https://localhost:5010/logout.html"
                        },

                    AllowedScopes = { "openid", "profile", "api1"},
                }
            };
    }
}