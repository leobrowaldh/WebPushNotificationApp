
namespace WebPushNotificationApp.Mvc.Models;

public record HomeIndexViewModel
(
    string UserId,
    string ProfilePicture,
    IEnumerable<Database.EntityModels.Message> Messages,
    IEnumerable<ContactDTO> Contacts
);
