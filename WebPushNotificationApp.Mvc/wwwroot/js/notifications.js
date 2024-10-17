
//Checks if the current subscription is actually the logged in users subscription:
async function isUserSubscription(subscription) {
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
async function removeSubscriptionFromDatabase(existingSubscription) {

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
async function registerServiceWorker() {
    if ('serviceWorker' in navigator) {
        try {
            registration = await navigator.serviceWorker.register('sw.js');
            console.log('Service Worker registered with scope:', registration.scope);
        } catch (error) {
            console.error('Service Worker registration failed:', error);
        }
    } else {
        console.error("Service workers are not supported.");
    }
}

// Function to convert VAPID public key from base64 URL format to Uint8Array
function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    return new Uint8Array([...rawData].map(char => char.charCodeAt(0)));
}
