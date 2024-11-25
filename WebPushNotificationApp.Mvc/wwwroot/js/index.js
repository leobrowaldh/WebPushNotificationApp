import { isUserSubscription, registerServiceWorker, urlBase64ToUint8Array, ManagingSubscriptionState } from './notifications.js'; 


// Creating a reference to the notification modal instance
let notificationModal = new bootstrap.Modal(document.getElementById('notification-modal'));

//If the user is logged in, only then do we proceed to check for subscriptions:
if (!localStorage.getItem('modalShown')) {
    // If the user is logged in, check for subscriptions
    if (userId) {


        // Show the modal
        notificationModal.show();

        // Set the item in localStorage to prevent future shows
        localStorage.setItem('modalShown', 'true');
    }
}

//accept-notifications button listener:
document.getElementById('subscribe-button').addEventListener('click', async function () {
    // Hide the modal using the instance
    notificationModal.hide();

    // Ask user for permission through the browser's Notification API
    let permission = await Notification.requestPermission();
    if (permission === 'granted') {

        // Preparing service worker
        await registerServiceWorker();
        let registration = await navigator.serviceWorker.ready;
        console.log('Service Worker is ready');

        // Subscribe the service worker
        const newSubscription = await registration.pushManager.subscribe({
            userVisibleOnly: true, // visible notifications for the user
            applicationServerKey: urlBase64ToUint8Array(publicKey) // Convert public key to the expected format.
        });
        if (newSubscription) {
            console.log('New subscription added to service worker');
        }

        // **************Sending subscription to the server**************

        console.log('Sending POST request to server with subscription data.');
        const response = await fetch('/Notifications/SavingSubscriptionToDb', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newSubscription)
        });
        if (response.ok) {
            console.log('subscription saved to database');
            ManagingSubscriptionState();
        } else {
            console.error('Failed to subscribe:', response.statusText);
        }



    } else if (permission === 'denied') {
        console.log('Push Notifications - permission denied');
        return;
    } else {
        console.log('Push Notifications - permission dismissed or default');
        return;
    }
});

//deny-notifications button listener:
document.getElementById('no-thanks').addEventListener('click', function () {
    console.log('Notifications rejected by user.');

    //remove subscription request and hide subscription button
    notificationModal.hide();
});





