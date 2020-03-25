namespace IdentityUtils.Api.Extensions.Cli
{
    public class ServicesConfiguration : IApiWrapperConfig
    {
        public string Is4Hostname { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ClientScope { get; set; }
    }
}