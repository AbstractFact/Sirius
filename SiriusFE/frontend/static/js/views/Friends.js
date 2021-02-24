import AbstractView from "./AbstractView.js";
import {User} from "../models/User.js"

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Friends");
        this.friends=new Array();
    }

    async getHtml() 
    {
        var html,i;

        if(localStorage.logged!=0)
        {
            i=0;
            await fetch("https://localhost:44365/User/GetFriendRequests/"+localStorage.userid, { method: "GET"})
            .then(p => p.json().then(data => {
                if(data.length!=0)
                { 
                    html=`
                    <h1>Friend requests</h1>
                    <table class="table table-striped">
                    <thead>
                        <tr>
                        <th scope="col">#</th>
                        <th scope="col">Username</th>
                        <th scope="col"></th>
                        <th scope="col"></th>
                        </tr>
                    </thead>
                    <tbody>`;

                    data.forEach(d => {
                        if(d!=null)
                        {
                            const requestID = d["id"];
                            const request=d["request"];

                            html+=`
                                <tr id="${requestID}">
                                <th scope="row">${++i}</th>
                                <td>${request.username}</td>
                                <td>
                                    <button type="submit" class="btn btn-success" style="width:50%" id="${requestID} ${request.id}" confirmRequestBtn>Confirm</button>
                                </td>
                                <td>
                                    <button type="submit" class="btn btn-danger" style="width:50%" id="R${requestID} ${request.id}" removeRequestBtn>Remove</button>
                                </td>
                                </tr>`;
                        }
                    });

                    html+=`
                    </tbody>
                    </table>`;
                }
                else
                    html=`<h2>No new friend requests</h2>`;
            }));

           html+=`
                <br/>
                <h1>My Friends</h1>
                <br/>
                <table class="table table-striped">
                    <thead>
                        <tr>
                        <th scope="col">#</th>
                        <th scope="col">Username</th>
                        <th scope="col">Recommendations</th>
                        <th scope="col"></th>
                        </tr>
                    </thead>
                    <tbody>`;
                i=0;
                await fetch("https://localhost:44365/User/GetAllFriends/"+localStorage.userid, {method: "GET"})
                .then(p => p.json().then(data => {
                    data.forEach(d => {
                        if(d!=null)
                        {
                        const friend = new User(d["id"], d["username"], d["password"]);
                        this.friends.push(friend);

                        html+=`
                            <tr id="${friend.id}">
                            <th scope="row">${++i}</th>
                            <td>${friend.username}</td>
                            <td><a href="/favourites/${friend.id}" data-link>View</a></td>
                            <td>
                                <button type="submit" class="btn btn-danger" style="width:50%" id="R${friend.id}">Unfriend</button>
                            </td>
                            </tr>`;
                        }
                    });
                }));
        
                html+=`
                </tbody>
                </table>
                <p>
                    <a href="/favourites" data-link>View all recommended series</a>
                </p>
                <form id="addfriend-form" style="width:40%">
                <div class="form-group col-md-8">
                    <div class="form-group col-md-10">
                    <label for="inputUsername">Friend's Username</label>
                    <input type="text" class="form-control" style="width:100%;" id="inputUsername" placeholder="Enter friend's username">
                    </div>
                </div>
                <button type="submit" class="btn btn-primary" style="width:30%" sendFriendRequestBtn>Send Friend Request</button>
                </form>`;
        }

        return html;
    }

    GetFriends()
    {
        return this.friends;
    }

    async SendFriendRequest()
    {
        const addFriendForm = document.querySelector('#addfriend-form');
        const username = addFriendForm['inputUsername'].value;
        const response =  await  fetch("https://localhost:44365/User/SendFriendRequest/"+username, { method: "POST", 
        headers: {
            "Content-Type": "application/json"
            },
            body: JSON.stringify({ "id": parseInt(localStorage.userid), "username": localStorage.username })
        });

        if (response.status==200) {
            addFriendForm.reset();
            alert("Request sent!");
        } 
        else if (response.status==404)
        {
            alert("User does not exist!");
        }
        else if (response.status==400)
        {
            alert("User is already your friend!");
        }
        else
        {
            addFriendForm.reset();
            alert("Request already sent, or you already have request from specified user!");
        }  
    }

    async ConfirmRequest(requestID, senderID)
    {
        const response =  await  fetch("https://localhost:44365/User/Befriend/"+senderID+"/"+localStorage.userid+"/"+requestID, { method: "POST"});

        if (response.ok) {
            alert("Friend added!");
        }   
    }

    async RemoveRequest(requestID, senderID)
    {
        const response =  await  fetch("https://localhost:44365/User/DeleteFriendRequest/"+localStorage.userid+"/"+requestID+"/"+senderID, { method: "DELETE"});

        if (response.ok) {
            alert("Request removed!");
        }   
    }

    async Unfriend(id)
    {
        await fetch("https://localhost:44365/User/Unfriend/"+localStorage.userid+"/"+id, { method: "DELETE"}).then(p => {
            if (p.ok) 
            {
                alert("User unfriended!");
            }
        });
    }
}