using System;

namespace IdentityUtils.Core.Contracts.Users
{
    /// <summary>
    /// Properties that all user domain transfer objects need to implement.
    /// </summary>
    public interface IIdentityManagerUserDto
    {
        Guid Id { get; }
        string Username { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string AdditionalDataJson { get; set; }
    }

    /// <summary>
    /// Type specification for additional data of domain transfer object.
    /// </summary>
    public interface IIdentityManagerUserDto<T> : IIdentityManagerUserDto
        where T : class
    {
        public T AdditionalData { get; set; }
    }
}