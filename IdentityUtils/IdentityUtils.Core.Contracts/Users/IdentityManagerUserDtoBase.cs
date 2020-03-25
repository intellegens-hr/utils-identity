using Newtonsoft.Json;
using System;

namespace IdentityUtils.Core.Contracts.Users
{
    /// <summary>
    /// Default implementation of Identity Manager user domain transfer object
    /// which contains additional data (typed)
    /// </summary>
    /// <typeparam name="T">Type used to serialize/deserialize additional JSON data</typeparam>
    public abstract class IdentityManagerUserDtoBase<T> : IIdentityManagerUserDto<T>
       where T : class
    {
        public virtual T AdditionalData
        {
            get
            {
                return AdditionalDataJson == null
                    ? null
                    : JsonConvert.DeserializeObject<T>(AdditionalDataJson);
            }
            set
            {
                AdditionalDataJson = value == null
                        ? null
                        : JsonConvert.SerializeObject(value);
            }
        }

        public virtual Guid Id { get; set; }

        public virtual string Password { get; set; }

        public virtual string AdditionalDataJson { get; set; }
    }
}