using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityUtils.Api.Models.Authentication
{
    public class TokenRequest : IValidatableObject
    {
        public string ClientId { get; set; }

        [Required]
        public string GrantType { get; set; }

        public string Password { get; set; }

        public string RefreshToken { get; set; }

        public string Username { get; set; }

        public static TokenRequest PasswordTokenRequest(string clientId, string username, string password)
        {
            return new TokenRequest
            {
                GrantType = AuthenticationConstants.GrantTypePassword,
                ClientId = clientId,
                Username = username,
                Password = password
            };
        }

        public static TokenRequest RefreshTokenRequest(string clientId, string refreshToken)
        {
            return new TokenRequest
            {
                GrantType = AuthenticationConstants.GrantTypeRefreshToken,
                ClientId = clientId,
                RefreshToken = refreshToken
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool isUnknownGrant = GrantType != AuthenticationConstants.GrantTypePassword && GrantType != AuthenticationConstants.GrantTypeRefreshToken;
            if (isUnknownGrant)
            {
                yield return new ValidationResult(
                $"Unknown grant type {GrantType}.",
                new[] { nameof(GrantType) });
            }

            bool usernameAndPasswordSpecified = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
            bool usernameOrPasswordSpecified = !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password);
            bool refreshTokenSpecified = !string.IsNullOrEmpty(RefreshToken);

            if (GrantType == AuthenticationConstants.GrantTypePassword && !usernameAndPasswordSpecified)
            {
                yield return new ValidationResult(
                $"Username and password must be provided.",
                new[] { nameof(Username), nameof(Password) });
            }

            if (GrantType == AuthenticationConstants.GrantTypePassword && refreshTokenSpecified)
            {
                yield return new ValidationResult(
                $"Refresh token not needed for password grant type",
                new[] { nameof(RefreshToken) });
            }

            if (GrantType == AuthenticationConstants.GrantTypeRefreshToken && !refreshTokenSpecified)
            {
                yield return new ValidationResult(
                $"Refresh token must be provided.",
                new[] { nameof(RefreshToken) });
            }

            if (GrantType == AuthenticationConstants.GrantTypeRefreshToken && usernameOrPasswordSpecified)
            {
                yield return new ValidationResult(
                $"Username and/or password not needed for password grant type",
                new[] { nameof(Username), nameof(Password) });
            }
        }
    }
}