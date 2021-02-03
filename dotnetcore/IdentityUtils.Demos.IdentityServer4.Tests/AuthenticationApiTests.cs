using IdentityUtils.Api.Extensions;
using IdentityUtils.Api.Models.Authentication;
using IdentityUtils.Commons;
using IdentityUtils.Core.Contracts.Commons;
using IdentityUtils.Demos.IdentityServer4.SingleTenant;
using IdentityUtils.Demos.IdentityServer4.SingleTenant.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TokenRequest = IdentityUtils.Api.Models.Authentication.TokenRequest;
using TokenResponse = IdentityUtils.Api.Models.Authentication.TokenResponse;

namespace IdentityUtils.Demos.IdentityServer4.Tests
{
    public class AuthenticationApiTests : TestAbstract<Startup>
    {
        private const string ClientId = "jsapp";
        private readonly RestClient restClient = new RestClient();
        private readonly UserManagementApi<UserDto> userManagementApi;

        public AuthenticationApiTests() : base()
        {
            userManagementApi = serviceProvider.GetRequiredService<UserManagementApi<UserDto>>();
        }

        private UserDto GetUniqueTestUser => new UserDto
        {
            Username = "testuser1@email.com" + Guid.NewGuid().ToString().Replace("-", ""),
            Password = "Wery5trong!Pa55word",
            Email = "testuser1@email.com"
        };

        [Fact]
        public async Task Getting_auth_token_should_work()
        {
            var userDto = GetUniqueTestUser;
            await userManagementApi.CreateUser(userDto);

            var tokenRequest = TokenRequest.PasswordTokenRequest(ClientId, userDto.Username, userDto.Password);
            var restResult = await restClient.Post<IdentityUtilsResult<TokenResponse>>($"{HostAddress}/auth/token", tokenRequest);
            var tokenData = restResult.ResponseData?.Data?.FirstOrDefault();

            Assert.True(restResult.Success);
            Assert.True(restResult.ResponseData.Success);
            Assert.NotNull(tokenData);
            Assert.NotEmpty(tokenData.AccessToken);
            Assert.NotEmpty(tokenData.RefreshToken);
        }

        [Fact]
        public async Task Getting_profile_should_work()
        {
            var userDto = GetUniqueTestUser;
            await userManagementApi.CreateUser(userDto);

            var tokenRequest = TokenRequest.PasswordTokenRequest(ClientId, userDto.Username, userDto.Password);
            var restResult = await restClient.Post<IdentityUtilsResult<TokenResponse>>($"{HostAddress}/auth/token", tokenRequest);
            var accessToken = restResult.ResponseData.Data.First().AccessToken;

            var profileRestResult = await restClient.Get<IdentityUtilsResult<UserProfile>>($"{HostAddress}/auth/init", accessToken);

            Assert.True(profileRestResult.Success);
            Assert.True(profileRestResult.ResponseData.Success);
            Assert.NotEmpty(profileRestResult.ResponseData.Data);
        }

        [Fact]
        public async Task Getting_refresh_token_should_work()
        {
            var userDto = GetUniqueTestUser;
            await userManagementApi.CreateUser(userDto);

            var tokenRequest = TokenRequest.PasswordTokenRequest(ClientId, userDto.Username, userDto.Password);
            var restResult = await restClient.Post<IdentityUtilsResult<TokenResponse>>($"{HostAddress}/auth/token", tokenRequest);
            var refreshToken = restResult.ResponseData.Data.First().RefreshToken;

            var refreshTokenRequest = TokenRequest.RefreshTokenRequest(ClientId, refreshToken);
            var restResultRefresh = await restClient.Post<IdentityUtilsResult<TokenResponse>>($"{HostAddress}/auth/token", refreshTokenRequest);

            Assert.True(restResultRefresh.Success);
            Assert.True(restResultRefresh.ResponseData.Success);
            Assert.NotEmpty(restResultRefresh.ResponseData.Data);
        }
    }
}