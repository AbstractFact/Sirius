var Connection = (function () {
    var connection;
 
    function createInstance() {
        return new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:44365/sirius")
        .configureLogging(signalR.LogLevel.Information)
        .build();
    }

    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    };

    return {
        getInstance: function () {
            if (!connection) {
                connection = createInstance();
                connection.onclose(start);
                start();

                connection.on("ReceiveMessage", (message) => {
                    console.log("stigla poruka:"+message);
                    // const li = document.createElement("li");
                    // li.textContent = `${user}: ${message}`;
                    // document.getElementById("messageList").appendChild(li);
                });
            }
            return connection;
        }
    };
})();
 
export default Connection;