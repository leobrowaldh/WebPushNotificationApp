
namespace WebPushNotificationApp.Mvc.Models;

public record HomeIndexViewModel
(
    string UserId,
    string ProfilePicture,
    IList<ContactDTO> Contacts,
    IEnumerable<Database.EntityModels.Message> Messages,
    IEnumerable<Database.EntityModels.AplicationUser> Users
);
