using System;

namespace IdentityUtils.Core.Contracts.Users
{
    /// <summary>
    /// Properties that all user domain transfer objects need to implement.
    /// </summary>
    public interface IIdentityManagerUserDto
    {
        Guid Id { get; }
        string Password { get; }
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