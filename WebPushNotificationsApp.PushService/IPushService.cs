

using WebPush;

namespace WebPushNotificationsApp.PushService;

public interface IPushService
{
    public Task SendNotificationAsync(PushSubscription subscription, string payload);
}
