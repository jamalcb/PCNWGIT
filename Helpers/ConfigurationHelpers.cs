namespace PCNW.Helpers
{
    public class AppSettings
    {
        //Properties for the JWT token
        public string Site { get; set; }
        public string Audience { get; set; }
        public string TokenExpireMinutes { get; set; }
        public string Secret { get; set; }
        public string BaseURL { get; set; }
        public string RefreshTokenExpireTime { get; set; }
        public string SharedApplicationName { get; set; }
        public string SecurityKey { get; set; }
        public string ViewDocUrl { get; set; }
    }
    public class UserAndTokenSettings
    {
        public string MaxFailedAccessAttempts { get; set; }
        public string UserLockoutMinutes { get; set; }
        public string UserAuthSessionMinutes { get; set; }
        public string RecoverTimeMinutes { get; set; }

    }

}