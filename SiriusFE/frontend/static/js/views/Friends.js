import AbstractView from "./AbstractView.js";
import {User} from "../models/User.js"

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("My Series List");
        this.friends=new Array();
    }

    async getHtml() 
    {
        var html,i;

        if(localStorage.logged!=0)

        await fetch("https://localhost:44365/User/GetAllFriends/"+localStorage.userid, {method: "GET"})
        .then(p => p.json().then(data => {
                i=0;
                html=`
                    <h1>My Series List</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Username</th>
                            
                            <th scope="col"></th>
                            </tr>
                        </thead>
                        <tbody>`;

            data.forEach(d => {
                if(d!=null)
                {
                const friend = new User(d["id"], d["username"], d["password"]);
                this.friends.push(friend);

                html+=`
                    <tr id="${friend.id}">
                    <th scope="row">${++i}</th>
                    <td>${friend.username}</td>
                    
                    <td>
                        <button type="submit" class="btn btn-danger" style="width:50%" id="R${friend.id}">Unfriend</button>
                    </td>
                    </tr>`;
                }
            });

            html+=`
                </tbody>
                </table>
                <p>
                    <a href="/favourites" data-link>View recommended series</a>.
                </p>

                <br/>

                <form id="addfriend-form" style="width:40%">
                <div class="form-group col-md-8">
                    <div class="form-group col-md-10">
                    <label for="inputUsername">Friend's Username</label>
                    <input type="text" class="form-control" style="width:100%;" id="inputUsername" placeholder="Enter friend's username">
                    </div>
                </div>
                <button type="submit" class="btn btn-primary" style="width:30%" addFriendBtn>Add Friend</button>
                </form>`;
        }));

        return html;
    }

    GetFriends()
    {
        return this.friends;
    }

    async Befriend()
    {
        const addFriendForm = document.querySelector('#addfriend-form');
        const username = addFriendForm['inputUsername'].value;

        const response =  await  fetch("https://localhost:44365/User/Befriend/"+localStorage.userid+"/"+username, { method: "POST"});

        if (response.ok) {
            addFriendForm.reset();
            alert("User "+username+" added as friend!");
        }   
    }

    Unfriend(id)
    {
        fetch("https://localhost:44365/User/Unfriend/"+localStorage.userid+"/"+id, { method: "DELETE"}).then(p => {
            if (p.ok) 
            {
                alert("User unfriended!");
            }
        });
    }
}