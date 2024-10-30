using WebPush;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using Database.Repositories;

namespace WebPushNotificationsApp.PushService;

public class PushService : IPushService
{
    private readonly VapidDetails _vapidDetails;
    private readonly ILogger<PushService> _logger;

    public PushService(IConfiguration configuration, ILogger<PushService> logger)
    {
        _vapidDetails = new VapidDetails(
            configuration["VapidDetails:Subject"],
            configuration["VapidDetails:PublicKey"],
            configuration["VapidDetails:PrivateKey"]
        );
        _logger = logger;
    }

    public async Task<bool> SendNotificationAsync(PushSubscription subscription, string payload)
    {
        var webPushClient = new WebPushClient();

        try
        {
            await webPushClient.SendNotificationAsync(subscription, payload, _vapidDetails);
        }
        catch (WebPushException ex) when (ex.StatusCode == HttpStatusCode.Gone || ex.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogWarning(ex, "WebPush error: {StatusCode}", ex.StatusCode);
            return true;
        }
        catch (WebPushException ex)
        {
            _logger.LogError(ex, "WebPush error: {StatusCode}", ex.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "General error while sending notification");
        }
        return false;
    }
}
