using WebPushNotificationApp.Mvc.DTOs;

namespace WebPushNotificationApp.Mvc.Models;

public record HomeIndexViewModel
(
    int UserId,
    string ProfilePicture,
    IList<ContactDTO> Contacts
);
