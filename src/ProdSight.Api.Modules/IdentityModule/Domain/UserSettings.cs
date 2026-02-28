namespace ProdSight.Api.Modules.IdentityModule.Domain;
 
public class UserSettings
{
    public UserSettings()
    {
        TimeZoneId = "UTC";
    }

    public UserSettings(string userId, string timeZoneId = "UTC")
    {
        UserId = userId;
        TimeZoneId = timeZoneId;
    }

    public string UserId { get; set; } = null!;
    public string TimeZoneId { get; set; }
}
