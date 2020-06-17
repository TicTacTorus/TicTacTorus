"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/connectionHub").build();
connection.start();


function listeners() {
    document.getElementById("messageInput").addEventListener("keyup", event => {
        alert(event.key)
        if(event.key === "Enter") {
            alert("Enter")
            var user = "Marco";
            var message = document.getElementById("messageInput").value;
            connection.invoke("SendMessage2", user, message).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        }
    });
}

function clientListeners() {
    connection.on("ReceiveMessage2", function (user, message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = user + " says " + msg + "\n";
        alert("Received: " + msg);
        document.getElementById("messagesList").innerHTML += encodedMsg;
    });
}





