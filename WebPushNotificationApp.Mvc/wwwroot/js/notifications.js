
//Checks if the current subscription is actually the logged in users subscription:
export async function isUserSubscription(subscription) {
    try {
        const response = await fetch('/Notifications/CheckUserSubscriptionAsync', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(subscription)
        });

        if (response.ok) {
            const data = await response.json();
            return data.isUserSubscription; // true or false based on server response
        } else {
            console.error('Failed to check user subscription:', response.statusText);
            return null; // Indeterminate state (error occurred)
        }
    } catch (error) {
        console.error('Error verifying subscription:', error);
        return null; // Indeterminate state (error occurred)
    }
}


//unsubscribing and removing subscription from db:
export async function removeSubscriptionFromDatabase(existingSubscription) {

        // Remove the subscription from the database
        const response = await fetch('/Notifications/RemoveSubscriptionAsync', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(existingSubscription)
        });

        if (response.ok) {
            console.log('Subscription removed from the database.');
        } else {
            console.error('Failed to remove subscription from the database.');
        }
}


// If not present, the function registers the service worker.
//If allready registered in the browser, it uses the existing one.
//It also updates the service worker if there are changes in the code.
export async function registerServiceWorker() {
    if ('serviceWorker' in navigator) {
        try {
            const registration = await navigator.serviceWorker.register('/sw.js');
            console.log('Service Worker registered with scope');
        } catch (error) {
            console.error('Service Worker registration failed:', error);
        }
    } else {
        console.error("Service workers are not supported.");
    }
}

// Function to convert VAPID public key from base64 URL format to Uint8Array
export function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    return new Uint8Array([...rawData].map(char => char.charCodeAt(0)));
}
export async function ManagingSubscriptionState() {

    const pushButton = document.getElementById('push-button');
    const registration = await navigator.serviceWorker.ready;
    const existingSubscription = await registration.pushManager.getSubscription();

    if (existingSubscription) {
        console.log('Subscription exists');
        const isUserSub = await isUserSubscription(existingSubscription);
        if (isUserSub) {
            pushButton.disabled = false;
            document.getElementById('notification-switch').checked = true;
            pushButton.classList.remove('disabled');
            document.getElementById('status-message').textContent = 'Notifications are enabled.';
        } else {
            console.log('The subscription does not belong to this user');
            await registerServiceWorker();
            pushButton.disabled = true;
            document.getElementById('notification-switch').checked = false;
            pushButton.classList.add('disabled');
            document.getElementById('status-message').textContent = 'Notifications are disabled.';
        }
    } else {
        await registerServiceWorker();
        console.log('No subscription found');
        pushButton.disabled = true;
        document.getElementById('notification-switch').checked = false;
        pushButton.classList.add('disabled');
        document.getElementById('status-message').textContent = 'Notifications are disabled.';
    }
}