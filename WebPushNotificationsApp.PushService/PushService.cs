using WebPush;
using Microsoft.Extensions.Configuration;

namespace WebPushNotificationsApp.PushService;

public class PushService: IPushService
{
    private readonly VapidDetails _vapidDetails;

    public PushService(IConfiguration configuration)
    {
        _vapidDetails = new VapidDetails(
            configuration["VapidDetails:Subject"],
            configuration["VapidDetails:PublicKey"],
            configuration["VapidDetails:PrivateKey"]
            );
    }

    /// <summary>
    /// Send the notification determined in Payload to user stored in subscription
    /// </summary>
    /// <param name="subscription">A user subscription object to the web push, should have been stored when the user subscribed</param>
    /// <param name="payload">Specifies the information contained in the notification to send</param>
    /// <returns></returns>
    public async Task SendNotificationAsync(PushSubscription subscription, string payload)
    {
        var webPushClient = new WebPushClient();

        try
        {
            await webPushClient.SendNotificationAsync(subscription, payload, _vapidDetails);
        }
        catch (Exception ex) 
        {
            Console.WriteLine($"{ex.Message}");
        }
    }
}
