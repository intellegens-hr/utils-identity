namespace IdentityUtils.Commons.Mailing.Google
{
    public interface IGoogleMailingProviderConfig
    {
        /// <summary>
        /// GMail username
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// DON'T USE YOUR PASSWORD HERE
        /// Instead, use app password as described on Google support page
        /// https://support.google.com/accounts/answer/185833?hl=en
        /// </summary>
        public string Password { get; }
    }
}