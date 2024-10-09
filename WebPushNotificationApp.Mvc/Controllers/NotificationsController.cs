using Database.EntityModels;
using Database.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebPush;
using WebPushNotificationsApp.PushService;

namespace WebPushNotificationApp.Mvc.Controllers;

[Route("Notifications")]
public class NotificationsController(
    ILogger<HomeController> _logger, 
    IPushService _pushService, 
    IUserRepository _userRepository,
    UserManager<AplicationUser> _userManager) : Controller
{

    [HttpPost("SavingSubscriptionToDb")]
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

    [HttpPost("SendNotification")]
    public async Task<IActionResult> SendNotification(string userId)
    {
        // Retrieve subscriptions from database
        List <Subscription> subscriptions = await _userRepository.GetUserSubscriptionsAsync(userId);

        if (subscriptions.Count == 0)
        {
            return BadRequest("No subscription found in database.");
        }

        foreach (Subscription subscription in subscriptions)
        {
            if (string.IsNullOrEmpty(subscription.SubscriptionJson))
            {
                _logger.LogWarning("SubscriptionJson is null or empty for subscription with ID {SubscriptionId}.", subscription.Id);
                continue; // Skipping this subscription if SubscriptionJson is invalid
            }
            var subscriptionToPushTo = JsonConvert.DeserializeObject<PushSubscription>(subscription.SubscriptionJson);

            if (subscriptionToPushTo == null)
            {
                _logger.LogWarning("Failed to deserialize SubscriptionJson for subscription with ID {SubscriptionId}.", subscription.Id);
                continue; // Skipping this subscription if deserialization fails
            }

            var payload = JsonConvert.SerializeObject(new
            {
                title = "Test Notification",
                message = "This is a notification for you!",
                icon = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
                badge = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
            });

            await _pushService.SendNotificationAsync(subscriptionToPushTo, payload);
        }
        return Ok("Notification Sent");
    }

    [HttpPost("CheckUserSubscriptionAsync")]
    public async Task<IActionResult> CheckUserSubscriptionAsync([FromBody] PushSubscription subscription)
    {
        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId is null)
        {
            _logger.LogWarning("User not logged in, redirecting to login page.");
            return Redirect($"~/Identity/Account/Login?ReturnUrl={Url.Action("Index", "Home")}");
        }

        var subscriptionJson = JsonConvert.SerializeObject(subscription);
        _logger.LogInformation("Serialized subscription from client: {subscriptionJson}", subscriptionJson);

        try
        {
            bool isUserSubscription = await _userRepository.IsUserSubscriptionAsync(subscriptionJson, currentUserId);
            _logger.LogInformation("IsUserSubscriptionAsync result: {isUserSubscription}", isUserSubscription); 

            return Ok(new { isUserSubscription });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking user subscription.");
            return StatusCode(500, "An error occurred while checking the subscription.");
        }
    }


    [HttpPost("RemoveSubscriptionAsync")]
    public async Task<IActionResult> RemoveSubscriptionAsync([FromBody] PushSubscription subscription)
    {
        bool success = await _userRepository.RemoveSubscriptionAsync(JsonConvert.SerializeObject(subscription));
        if (success)
        {
            return Ok("subscription successfully removed from database.");
        }
        else 
        { 
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to remove the subscription from database."); 
        }
    }
}
