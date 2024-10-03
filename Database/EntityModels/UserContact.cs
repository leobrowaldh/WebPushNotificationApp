namespace Database.EntityModels;

public class UserContact
{
    public string UserId { get; set; }
    public AplicationUser User { get; set; }
    public string ContactUserId { get; set; }
    public AplicationUser ContactUser { get; set; }
}
