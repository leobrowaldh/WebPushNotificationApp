using Database.EntityModels;
using Database.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebPush;
using WebPushNotificationsApp.PushService;

namespace WebPushNotificationApp.Mvc.Controllers;

public class NotificationsController(
    ILogger<HomeController> _logger, 
    IPushService _pushService, 
    IUserRepository _userRepository,
    UserManager<AplicationUser> _userManager) : Controller
{

    [HttpPost]
    public async Task<IActionResult> SavingSubscriptionToDb([FromBody] PushSubscription subscription)
    {
        _logger.LogInformation("Received subscription: Endpoint = {Endpoint}, P256dh = {P256dh}, Auth = {Auth}",
        subscription.Endpoint, subscription.P256DH, subscription.Auth);

        // Save the subscription object to database.
        // This object contains the endpoint and keys to send push notifications.

        // Ensure keys are not null
        if (string.IsNullOrEmpty(subscription.P256DH) || string.IsNullOrEmpty(subscription.Auth))
        {
            _logger.LogWarning("Subscription keys are missing or invalid.");
            return BadRequest("Subscription must include auth and p256dh keys.");
        }

        //check modelstate of subscription object
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid subscription model state: {@ModelState}", ModelState);
            return BadRequest("Invalid subscription data.");
        }

        // Get the logged-in user's ID
        string? userId = null; 
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            userId = _userManager.GetUserId(User);
        }
        //try to save to db.
        int subscriptionId = 0;
        if (userId != null)
        {
            subscriptionId = await _userRepository.SaveSubscriptionAsync(JsonConvert.SerializeObject(subscription), userId);
        }
        if (subscriptionId != 0)
        {
            _logger.LogInformation("Successfully saved the subscription for: {Endpoint}", subscription.Endpoint);
            //this c# anonymous object will be automatically serialized into JSON by asp.net core:
            return Ok(new { message = "Subscription saved to database.", id = subscriptionId });
        }
        else
        {
            _logger.LogError("Failed to save the subscription for: {Endpoint}", subscription.Endpoint);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save the subscription.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendNotification(string userId)
    {
        // Retrieve subscription from database
        string? subscriptionJSON = await _userRepository.GetUserSubscriptionsAsync(userId);

        if (subscriptionJSON is null)
        {
            return BadRequest("No subscription found in database.");
        }

        var subscription = JsonConvert.DeserializeObject<PushSubscription>(subscriptionJSON);
        var payload = JsonConvert.SerializeObject(new
        {
            title = "Test Notification",
            message = "This is a notification for you!",
            icon = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
            badge = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
        });

        await _pushService.SendNotificationAsync(subscription, payload);
        return Ok("Notification Sent");
    }

}
