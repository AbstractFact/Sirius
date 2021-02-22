import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.userID = params.userID;
        this.setTitle("Signup");
    }

    async getHtml() 
    {
        var html;

        html=`
        <form id="signup-form" method="post">
            <div class="container">
                <div class="inputitem">
                    <label for="uname"><b>Username</b></label>
                    <input type="text" placeholder="Enter Username" name="uname" id="login-username" required>
                </div>
                <div class="inputitem">
                    <label for="psw"><b>Password</b></label>
                    <input type="password" placeholder="Enter Password" name="psw" id="login-password" required>
                </div>
                <button class="inputitem" type="submit" signupbtn>Signup</button>
            </div>
        </form>`;
        
        return html;
    }

    async signup()
    {
        const signupForm = document.querySelector('#signup-form');
        const username = signupForm['login-username'].value;
        const password = signupForm['login-password'].value;  

        const response =  await fetch("https://localhost:44365/User", { method: "POST",
            headers: {
            "Content-Type": "application/json"
            },
            body: JSON.stringify({ "username": username, "password": password })
        });

        if (!response.ok) {
            alert("User already exists!");
            localStorage.userid=0;
            localStorage.logged=0;
            localStorage.username=0;
        }
        else
        {
            const json = await response.json();

            localStorage.userid=json["id"];
            localStorage.logged=1;
            localStorage.username=username;

            alert("Welcome to Sirius "+username); 
        }   
    }
}