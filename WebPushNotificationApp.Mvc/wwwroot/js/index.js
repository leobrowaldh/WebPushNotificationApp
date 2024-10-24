import { isUserSubscription, registerServiceWorker, urlBase64ToUint8Array } from './notifications.js'; 


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
        console.log('New subscription');

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
            // Extract JSON from the response to retrieve the userId
            const data = await response.json();
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


//checks the current state of webPush subscriptions in this browser and acts accordingly
async function ManagingSubscriptionState() {

    let registration = await navigator.serviceWorker.ready;

    // Check the state of subscription in the worker
    let existingSubscription = null;
    try {
        existingSubscription = await registration.pushManager.getSubscription();
        if (!existingSubscription) {
            console.log('No subscription found');
            // Show modal to ask for subscription
        } else {
            console.log('Subscription exists');
            // Check that subscription corresponds to the logged-in user
            const isUserSub = await isUserSubscription(existingSubscription);
            if (isUserSub) {
                console.log('The subscription belongs to the logged-in user.');
            } else if (isUserSub === false) {
                console.log('The subscription does not belong to this user, ask him to subscribe');
                // Show modal to ask for subscription
            } else {
                console.log('Could not verify subscription');
                // Handle the case of subscription verification failure
            }
        }
    } catch (error) {
        console.error('Error getting subscription:', error);
        // Show modal to ask for subscription in case of error
        notificationModal.show();
    }
}

