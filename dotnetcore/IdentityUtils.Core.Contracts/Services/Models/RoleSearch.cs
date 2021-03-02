namespace IdentityUtils.Core.Contracts.Services.Models
{
    public class RoleSearch
    {
        public RoleSearch(string? name = null)
        {
            Name = name;
        }

        public string? Name { get; set; }
    }
}