"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/uploaderHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    li.innerHTML = `<b>${user} says:</b> ${message}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var messageElement = document.getElementById("messageInput");
    connection.invoke("SendMessage", user, messageElement.value).catch(function (err) {
        return console.error(err.toString());
    });
    messageElement.value = "";
    event.preventDefault();
});

connection.on("broadcastMessage", function (name, item) {
    // Html encode display name and message.
    var encodedName = $('<div />').text(name).html();
    var encodedItem = $('<div />').text(item.statusInfo).html();
    // Add the message to the page.
    $('#discussion').append('<li><strong>' + encodedName
        + '</strong>:&nbsp;&nbsp;' + encodedItem + '</li>');
});
