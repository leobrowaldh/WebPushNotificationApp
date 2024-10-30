

using WebPush;

namespace WebPushNotificationsApp.PushService;

public interface IPushService
{
    /// <summary>
    /// Send the notification determined in Payload to user stored in subscription
    /// </summary>
    /// <param name="subscription">A user subscription object to the web push, should have been stored when the user subscribed</param>
    /// <param name="payload">Specifies the information contained in the notification to send</param>
    /// <returns>true if the subscription is faulty and need to be removed, false if not.</returns>
    public Task<bool> SendNotificationAsync(PushSubscription subscription, string payload);
}
