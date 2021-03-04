export default class Connection 
{
    constructor()
    {
        this.connection = this.createConnection();
    }

    createConnection()
    {
        return new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:44365/sirius")
            .configureLogging(signalR.LogLevel.Information)
            .build();
    }
    
    async start()
    {
        await this.connection
        .start()
        .then(()=>
        { 
            console.log('Connection started');
        })
        .catch(err => console.log('Error while starting connection: '+err))   
    }

    listen()
    {
        this.connection.on("ReceiveFriendRequests", (message) =>
        {
            const msg = JSON.parse(JSON.stringify(message));
            alert("Friend request from: " + msg.requestDTO.request.username); 
            if(window.location.href=="http://localhost:5001/friends")
                window.location.reload();

        });

        this.connection.on("FriendRequestAccepted", (message) =>
        {
            alert(message); 
            if(window.location.href=="http://localhost:5001/friends")
                window.location.reload();

        });

        this.connection.on("ReceiveRecommendations", (message) =>
        {
            const msg = JSON.parse(JSON.stringify(message));
            alert("Series recommendation: " + msg.title); 
            if(window.location.href=="http://localhost:5001/myserieslist")
                window.location.reload();
        });
    }

    register(userID)
    {
        this.connection.invoke("StartReceivingRequests", userID);
    }

    subscribe(genre)
    {
        this.connection.invoke("StartReceivingRecommendations", genre);
    }

    unsubscribe(genre)
    {
        this.connection.invoke("StopReceivingRecommendations", genre);
    }    
}

