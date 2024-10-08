namespace WebPushNotificationApp.Mvc.Models;

public record HomeIndexViewModel
(
    string UserId,
    string ProfilePicture,
    IList<ContactDTO> Contacts
);
