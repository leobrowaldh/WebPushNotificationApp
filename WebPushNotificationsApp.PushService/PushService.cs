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

    public async Task SendNotificationAsync(PushSubscription subscription, string payload)
    {
        var webPushClient = new WebPushClient();

        try
        {
            await webPushClient.SendNotificationAsync(subscription, payload, _vapidDetails);
        }
        catch (WebPushException ex) 
        {
            Console.WriteLine($"WebPush error: {ex.StatusCode}, {ex.Message}, {ex.StackTrace}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}, {ex.StackTrace}");
        }
    }
}
