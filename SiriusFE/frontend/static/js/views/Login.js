import AbstractView from "./AbstractView.js";
import {User} from "../models/User.js"

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.setTitle("Login");
    }

    async getHtml() 
    {
       var html;

       html=`
       <form id="login-form">
            <div class="container">
                <div class="inputitem">
                    <label for="uname"><b>Username</b></label>
                    <input type="text" placeholder="Enter Username" name="uname" id="login-username" required>
                </div>

                <div class="inputitem">
                    <label for="psw"><b>Password</b></label>
                    <input type="password" placeholder="Enter Password" name="psw" id="login-password" required>
                </div>
            
                <button class="inputitem" type="submit" loginbtn>Login</button>
            </div>
        </form>`;

        return html;
    }

    async login()
    {
        const loginForm = document.querySelector('#login-form');
        const username = loginForm['login-username'].value;
        const password = loginForm['login-password'].value;


        const response =  await fetch("https://localhost:44365/User/Login", { method: "POST",
                                    headers: {
                                    "Content-Type": "application/json"
                                    },
                                    body: JSON.stringify({ "username": username, "password": password })
                                });
        if (!response.ok) {
            alert("User not found!");
            localStorage.userid=0;
            localStorage.logged=0;
        }
        else
        {
            const json = await response.json();

            localStorage.userid=json["id"];
            localStorage.logged=1;

            alert("Welcome to Sirius "+username); 
        }                                                   
    }     
}