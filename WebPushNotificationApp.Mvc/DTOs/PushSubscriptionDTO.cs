namespace WebPushNotificationApp.Mvc.DTOs;

public class PushSubscriptionDto
{
    public string Endpoint { get; set; }
    public Keys Keys { get; set; }
}

public class Keys
{
    public string P256DH { get; set; }
    public string Auth { get; set; }
}
