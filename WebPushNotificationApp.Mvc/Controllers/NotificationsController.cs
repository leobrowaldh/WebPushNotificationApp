using Database.EntityModels;
using Database.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using WebPush;
using WebPushNotificationsApp.PushService;
using static System.Net.WebRequestMethods;

namespace WebPushNotificationApp.Mvc.Controllers;

[Route("Notifications")]
public class NotificationsController(
    ILogger<HomeController> _logger,
    IPushService _pushService,
    ISubscriptionRepository _subscriptionRepository,
    IMessageRepository _messageRepository,
	IConfiguration _configuration,
	UserManager<AplicationUser> _userManager) : Controller
{

    [HttpGet("settings")]
    public  IActionResult Settings()
    {
        ViewBag.UserId = _userManager.GetUserId(User);
		ViewBag.PublicKey = _configuration["VapidDetails:PublicKey"];
		return View();
    }

    [HttpPost("SavingSubscriptionToDb")]
    public async Task<IActionResult> SavingSubscriptionToDb([FromBody] PushSubscriptionDto subscriptionDto)
    {
        // Restructuring the subscription
        var dbSubscription = new WebPush.PushSubscription
        {
            Endpoint = subscriptionDto.Endpoint,
            P256DH = subscriptionDto.Keys.P256DH, // Flattening 'keys'
            Auth = subscriptionDto.Keys.Auth
        };

        _logger.LogInformation("Received subscription: Endpoint = {Endpoint}, P256dh = {P256dh}, Auth = {Auth}",
        dbSubscription.Endpoint, dbSubscription.P256DH, dbSubscription.Auth);

        // Save the subscription object to database.
        // This object contains the endpoint and keys to send push notifications.

        // Ensure keys are not null
        if (string.IsNullOrEmpty(dbSubscription.P256DH) || string.IsNullOrEmpty(dbSubscription.Auth))
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
            subscriptionId = await _subscriptionRepository.SaveSubscriptionAsync(JsonConvert.SerializeObject(dbSubscription), userId);
        }
        if (subscriptionId != 0)
        {
            _logger.LogInformation("Successfully saved the subscription for: {Endpoint}", dbSubscription.Endpoint);
            //this c# anonymous object will be automatically serialized into JSON by asp.net core:
            return Ok(new { message = "Subscription saved to database.", id = subscriptionId });
        }
        else
        {
            _logger.LogError("Failed to save the subscription for: {Endpoint}", dbSubscription.Endpoint);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to save the subscription.");
        }
    }

    [HttpPost("SendNotification")]
    public async Task<IActionResult> SendNotification(string userId)
    {
        // Retrieve subscriptions from database
        List<Subscription> subscriptions = await _subscriptionRepository.GetUserSubscriptionsAsync(userId);
        var lastMessage = _messageRepository.GetLastMessageAsync().Result;
        
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
                message = "This is a test Notification",
                icon = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
                badge = "https://static-00.iconduck.com/assets.00/slightly-smiling-face-emoji-2048x2048-p8h7zhgm.png",
            });

            await _pushService.SendNotificationAsync(subscriptionToPushTo, payload);
        }
        return Ok("Notification Sent");
    }

    [HttpPost("NotifyAll")]
    public async Task<IActionResult> NotifyAllExceptMe()
    {
        AplicationUser? sender = await _userManager.GetUserAsync(User);
        if (sender == null)
        {
            return BadRequest("No logged in user was found.");
        }

        // Retrieve subscriptions from database
        List<Subscription> subscriptions = await _subscriptionRepository.GetAllNonSenderSubscriptionsAsync(sender.Id);

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
                //pictures need to be urls to accessible pictures in the correct format for icon and badge.
                title = "New Message from " + sender.UserName,
                message = _messageRepository.GetLastMessageAsync().Result,
                icon = sender.ProfilePicture,
                badge = "https://static-00.iconduck.com/assets.00/message-icon-1023x1024-7pbl8unr.png", //the app logo
            });

            await _pushService.SendNotificationAsync(subscriptionToPushTo, payload);
        }
        return Ok("Notification Sent");
    }

    [HttpPost("CheckUserSubscriptionAsync")]
    public async Task<IActionResult> CheckUserSubscriptionAsync([FromBody] PushSubscriptionDto subscriptionDto)
    {
        // Restructuring the subscription
        var dbSubscription = new WebPush.PushSubscription
        {
            Endpoint = subscriptionDto.Endpoint,
            P256DH = subscriptionDto.Keys.P256DH,
            Auth = subscriptionDto.Keys.Auth
        };

        string? currentUserId = _userManager.GetUserId(User);

        if (currentUserId is null)
        {
            _logger.LogWarning("User not logged in, redirecting to login page.");
            return Redirect($"~/Identity/Account/Login?ReturnUrl={Url.Action("Index", "Home")}");
        }

        var subscriptionJson = JsonConvert.SerializeObject(dbSubscription);
        _logger.LogInformation("Serialized subscription from client: {subscriptionJson}", subscriptionJson);

        try
        {
            bool isUserSubscription = await _subscriptionRepository.IsUserSubscriptionAsync(subscriptionJson, currentUserId);
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
    public async Task<IActionResult> RemoveSubscriptionAsync([FromBody] PushSubscriptionDto subscriptionDto)
    {
        // Restructuring the subscription
        var dbSubscription = new WebPush.PushSubscription
        {
            Endpoint = subscriptionDto.Endpoint,
            P256DH = subscriptionDto.Keys.P256DH,
            Auth = subscriptionDto.Keys.Auth
        };

        bool success = await _subscriptionRepository.RemoveSubscriptionAsync(JsonConvert.SerializeObject(dbSubscription));
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