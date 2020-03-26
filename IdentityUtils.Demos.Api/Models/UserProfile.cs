namespace IdentityUtils.Demos.Api.Models
{
    public class UserProfile
    {
        public bool IsAuthenticated { get; set; }
        public ApiUser UserProfileData { get; set; }
    }
}