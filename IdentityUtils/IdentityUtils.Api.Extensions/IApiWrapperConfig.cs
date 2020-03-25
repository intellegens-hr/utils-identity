namespace IdentityUtils.Api.Extensions
{
    public interface IApiWrapperConfig
    {
        /// <summary>
        /// Identity server 4 hostname (ie. https://10.10.10.10:5002)
        /// </summary>
        string Is4Hostname { get; }
        
        /// <summary>
        /// Client ID used to authorize API calls to IS4
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Secret for client used to authorize API calls to IS4
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// Scope for authorizing API calls between API and IS4
        /// </summary>
        string ClientScope { get; }
    }
}