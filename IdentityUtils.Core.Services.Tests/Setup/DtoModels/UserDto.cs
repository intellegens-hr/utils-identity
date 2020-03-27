using IdentityUtils.Core.Contracts.Users;
using System;

namespace IdentityUtils.Core.Services.Tests.Setup.DtoModels
{
    public class UserDto : IIdentityManagerUserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string AdditionalDataJson { get; set; }
    }
}