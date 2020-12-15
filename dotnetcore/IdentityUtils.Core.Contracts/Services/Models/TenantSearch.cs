namespace IdentityUtils.Core.Contracts.Services.Models
{
    public class TenantSearch
    {
        public TenantSearch(string? hostname = null)
        {
            Hostname = hostname;
        }

        public string? Hostname { get; set; }
    }
}