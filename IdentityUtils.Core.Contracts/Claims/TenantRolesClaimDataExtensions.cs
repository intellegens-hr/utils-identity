using Newtonsoft.Json;

namespace IdentityUtils.Core.Contracts.Claims
{
    public static class TenantRolesClaimDataExtensions
    {
        public static string Serialize(this TenantRolesClaimData data)
            => JsonConvert.SerializeObject(data);

        public static TenantRolesClaimData DeserializeToTenantRolesClaimData(this string serializedData)
            => JsonConvert.DeserializeObject<TenantRolesClaimData>(serializedData);
    }
}