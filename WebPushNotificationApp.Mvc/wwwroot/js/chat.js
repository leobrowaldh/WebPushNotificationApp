class Message {
    constructor(username, text, when) {
        this.userName = username;
        this.text = text;
        this.when = when;
    }
}

// userName is declared in razor page.
const username = userName;
const textInput = document.getElementById('messageText');
const whenInput = document.getElementById('when');
const chat = document.getElementById('chat');
const messagesQueue = [];

const chats = document.getElementById('chat');
chats.scrollTop = chats.scrollHeight;

document.getElementById('submitButton').addEventListener('click', () => {
    var currentdate = new Date();
    when.innerHTML =
        (currentdate.getMonth() + 1) + "/"
        + currentdate.getDate() + "/"
        + currentdate.getFullYear() + " "
        + currentdate.toLocaleString('en-US', { hour: 'numeric', minute: 'numeric', hour12: true })
});

function clearInputField() {
    messagesQueue.push(textInput.value);
    textInput.value = "";
}

function sendMessage() {
    let text = messagesQueue.shift() || "";
    if (text.trim() === "") return;
    
    let when = new Date();
    let message = new Message(username, text);
    sendMessageToHub(message);
}

function addMessageToChat(message) {
    let isCurrentUserMessage = message.userName === username;

    // Skapa en container för meddelandet
    let container = document.createElement('div');
    container.className = "chat-container"; // Använd "chat-container"

    // Skapa en inner-container för att gruppera avsändarens namn, text och tid
    let messageContainer = document.createElement('div');
    messageContainer.className = isCurrentUserMessage ? "sender" : "receiver"; // Justera klassen för meddelandet

    // Skapa avsändarens namn
    let sender = document.createElement('p');
    sender.className = "sender-name"; // Klassiskt för avsändarens namn
    sender.innerHTML = message.userName;

    // Skapa meddelandets text
    let text = document.createElement('p');
    text.className = "message-text"; // Klassiskt för meddelandets text
    text.innerHTML = message.text;

    let when = document.createElement('span');
    when.className = "message-time"; // Klassiskt för tidsangivelsen
    var currentdate = new Date();

    // Formatera datum till YYYY-MM-DD
    let formattedDate = currentdate.getFullYear() + '-' +
        String(currentdate.getMonth() + 1).padStart(2, '0') + '-' +
        String(currentdate.getDate()).padStart(2, '0');

    // Hämta timmar, minuter och sekunder
    let hours = String(currentdate.getHours()).padStart(2, '0');
    let minutes = String(currentdate.getMinutes()).padStart(2, '0');
    let seconds = String(currentdate.getSeconds()).padStart(2, '0');

    // Kombinera datum och tid
    when.innerHTML = formattedDate + ' ' + hours + ':' + minutes + ':' + seconds; // Sätt det formaterade datumet och tiden som innehåll




    // Lägg till avsändarens namn, text och tid i inner-containern
    messageContainer.appendChild(sender);
    messageContainer.appendChild(text);
    messageContainer.appendChild(when);

    // Lägg till inner-containern i chatten
    container.appendChild(messageContainer);
    document.getElementById('chat').appendChild(container);

    const chats = document.getElementById('chat');
    chats.scrollTop = chats.scrollHeight;
}
