export default class Connection 
{
    constructor()
    {
        this.connection= new signalR.HubConnectionBuilder()
            .withUrl("https://localhost:44365/sirius")
            .configureLogging(signalR.LogLevel.Information)
            .build();
    }

    // copy(connection)
    // {
    //     this.connection=connection;
    // }

    copy(obj)
    {
        console.log(this.connection);
        this.connection = obj.connection;
        console.log(this.connection);
    }

    async getConnection()
    {
        await this.start();

        return this.connection;
    }
    
    async start()
    {
        await this.connection
        .start()
        .then(()=>
        { 
            console.log('Connection started');
            
            // this.connection.on("ReceiveMessage", (message) =>
            // {
            //     console.log("stigla poruka:"+message); 
            // });

            //localStorage.signalRConnection=JSON.stringify(this);
            //console.log(localStorage.signalRConnection);
        })
        .catch(err => console.log('Error while starting connection: '+err))   
    }

    listen()
    {
        this.connection.on("ReceiveMessage", (message) =>
        {
            console.log("stigla poruka:" + message); 
        });
    }

    sendMessage(mess)
    {
        this.connection.invoke('SendMessage', mess).catch(err => console.error(err));
    }
}


//     async function start() {
//         try {
//             await connection.start();
//             console.log("SignalR Connected.");
//         } catch (err) {
//             console.log(err);
//             setTimeout(start, 5000);
//         }
//     };

//     return {
//         getInstance: async function () 
//         {
//             connection = createInstance();
//             connection.onclose(start);
//             await start();
            
//                 connection.on("ReceiveMessage", (message) =>
//                 {
//                     console.log("stigla poruka:"+message); 
//                 });
//                 console.log('Connection started');
            
//             //.catch(err => console.log('Error while starting connection: '+err));

//             return connection;
//         }
//     };
// })();
 
// export default Connection;




// export class signalRService {​​​​
 
//     public hubConnection: signalR.HubConnection;
  
//     constructor(private http:HttpClient) {​​​​
   
//       this.startConnection();
//      }​​​​
  
//     public startConnection =()=>{​​​​
//         this.hubConnection = new signalR.HubConnectionBuilder()
//         .withUrl(URL+"/chat")
//         .build()
   
//         this.hubConnection
//         .start()
//         .then(()=> console.log('Connection started'))
//         .catch(err => console.log('Error while starting connection: '+err))
//     }​​​​
   
//     public sendMessage(mess:Message): void {​​​​
//       this.hubConnection
//         .invoke('sendToAll', mess)
//         .catch(err => console.error(err));
//     }​​​​
//   }​​​​