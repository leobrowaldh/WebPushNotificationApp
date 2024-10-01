self.addEventListener('push', function (event) {
    console.log('Push event received in service worker:', event); //DEBUG
    let data;

    try {
        // Attempt to parse the received data as JSON
        data = event.data.json();
    } catch (e) {
        // If parsing fails, treat it as plain text
        data = { title: 'Notification', message: event.data.text() };
    }

    const options = {
        body: data.message,
    };

    event.waitUntil(
        self.registration.showNotification(data.title, options)
    );
});
