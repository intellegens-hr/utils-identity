namespace IdentityUtils.Api.Models.Authentication
{
    public class Claim
    {
        public Claim()
        {
        }

        public Claim(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; }
        public string Value { get; set; }
    }
}