using WebPush;
using Microsoft.Extensions.Configuration;

namespace WebPushNotificationsApp.PushService;

public class PushService
{
    private readonly VapidDetails _vapidDetails;

    public PushService(IConfiguration configuration)
    {
        _vapidDetails = new VapidDetails(
            "mailto:leobrowaldh@gmail.com",
            configuration["VapidKeys:PublicKey"],
            configuration["VapidKeys:PrivateKey"]
            );
    }


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
