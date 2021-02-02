﻿using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using IdentityUtils.Api.Models.Authentication;
using IdentityUtils.Core.Contracts.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using TokenResponse = IdentityUtils.Api.Models.Authentication.TokenResponse;

namespace IdentityUtils.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationControllerApi : ControllerBase
    {
        private readonly IEventService events;
        private readonly IClientStore clientStore;
        private readonly ILogger logger;
        private readonly ITokenRequestValidator requestValidator;
        private readonly ITokenResponseGenerator responseGenerator;
        private readonly IClientSecretValidator clientValidator;

        public AuthenticationControllerApi(
            ITokenRequestValidator requestValidator,
            ITokenResponseGenerator responseGenerator,
            IClientSecretValidator clientValidator,
            IEventService events,
            IClientStore clientStore,
            ILogger<AuthenticationControllerApi> logger)
        {
            this.requestValidator = requestValidator;
            this.responseGenerator = responseGenerator;
            this.clientValidator = clientValidator;
            this.events = events;
            this.clientStore = clientStore;
            this.logger = logger;
        }

        [HttpPost("token")]
        public virtual async Task<IdentityUtilsResult<TokenResponse>> GetToken(TokenRequest request)
        {
            HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "client_id", request.ClientId },
                { "grant_type",  request.GrantType},
                { "username",  request.Username},
                { "password",  request.Password },
                { "refresh_token",  request.RefreshToken },
            });

            // validate client
            var client = await clientStore.FindEnabledClientByIdAsync(request.ClientId);
            var invalidClient = !client.AllowedGrantTypes.Intersect(GrantTypes.ResourceOwnerPassword).Any() && !(request.GrantType == AuthenticationConstants.GrantTypeRefreshToken);
            if (client == null || invalidClient)
            {
                return IdentityUtilsResult<TokenResponse>.ErrorResult($"Client id not found or invalid grant type");
            }
            var clientResult = new ClientSecretValidationResult
            {
                IsError = false,
                Client = client
            };

            var form = new NameValueCollection
            {
                { "client_id", request.ClientId },
                { "grant_type", request.GrantType },
                { "refresh_token", request.RefreshToken },
                { "username", request.Username },
                { "password", request.Password },
            };

            // validate request
            var requestResult = await requestValidator.ValidateRequestAsync(form, clientResult);

            if (requestResult.IsError)
            {
                await events.RaiseAsync(new TokenIssuedFailureEvent(requestResult));
                return IdentityUtilsResult<TokenResponse>.ErrorResult($"{requestResult.Error} - {requestResult.ErrorDescription}");
            }

            // create response
            var response = await responseGenerator.ProcessAsync(requestResult);

            await events.RaiseAsync(new TokenIssuedSuccessEvent(response, requestResult));

            // return result
            var tokenResponse = new TokenResponse
            {
                AccessToken = response.AccessToken,
                RefreshToken = response.RefreshToken,
                Lifetime = response.AccessTokenLifetime,
                Scopes = response.Scope.Split(' ')
            };
            return IdentityUtilsResult<TokenResponse>.SuccessResult(tokenResponse);
        }

        [Authorize]
        [HttpGet("init")]
        public virtual async Task<IdentityUtilsResult<dynamic>> ProfileInit()
        {
            var claims = User.Claims.Select(x => new { x.Value, x.Type });
            return IdentityUtilsResult<dynamic>.SuccessResult(claims);
        }
    }
}