import { removeSubscriptionFromDatabase } from './notifications.js'; 

// send-notification button-listener:
document.getElementById('push-button').addEventListener('click', async function () {
    console.log('Push button clicked');
    // Wait for the service worker to be ready
    const registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready:', registration);

    if (userId) {
        console.log('Sending notification...');
        //sending the id via form data to the server:
        const formData = new FormData();
        formData.append('userId', userId);
        const notificationResponse = await fetch('/Notifications/SendNotification', {
            method: 'POST',
            body: formData
        });
        console.log('Notification response:', notificationResponse);
    } else {
        console.log('No userId available to send notification.');
    }
})

//unsubscribe button listener:
document.getElementById('unsubscribe-button').addEventListener('click', async function () {
    // Wait for the service worker to be ready
    const registration = await navigator.serviceWorker.ready;
    console.log('Service Worker is ready:', registration);
    const existingSubscription = await registration.pushManager.getSubscription();
    try {
        const unsubscribeResult = await existingSubscription.unsubscribe();
        if (unsubscribeResult) {
            console.log('Successfully unsubscribed from browser.');

            if (existingSubscription) {
                await removeSubscriptionFromDatabase(existingSubscription);
            } else {
                console.log('No existing subscription found.');
            }
        }
    } catch (error) {
        console.error('Error unsubscribing from browser:', error);
    }
})