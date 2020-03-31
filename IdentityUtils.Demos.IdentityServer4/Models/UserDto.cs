using IdentityUtils.Core.Contracts.Users;
using System;
using System.Diagnostics.CodeAnalysis;

namespace IdentityUtils.Demos.IdentityServer4.Models
{
    public class UserDto : IIdentityManagerUserDto, IEquatable<UserDto>
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AdditionalDataJson { get; set; }

        public bool Equals([AllowNull] UserDto other)
        {
            return Id == other.Id
                && Username == other.Username
                && Email == other.Email
                && Password == other.Password
                && AdditionalDataJson == other.AdditionalDataJson;
        }
    }
}