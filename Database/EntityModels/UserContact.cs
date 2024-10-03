namespace Database.EntityModels;

public class UserContact
{
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string ContactUserId { get; set; }
    public ApplicationUser ContactUser { get; set; }
}
