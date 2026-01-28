namespace EventManagementAPI.Commons
{
    public class JwtSettings
    {
        public const string JwtSettingsName = "JwtSettings";
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int ExpiryMinutes { get; set; } = 60;
    }
}
