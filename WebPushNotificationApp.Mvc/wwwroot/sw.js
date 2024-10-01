self.addEventListener('push', function (event) {
    const data = event.data.json(); //parse the recieved JSON payload into a js object.
    const options = {
        body: data.message,
    };
    event.waitUntil(
        self.registration.showNotification(data.title, options)
    );
});
