import Home from "./views/Home.js";
import Series from "./views/Series.js";
import SeriesView from "./views/SeriesView.js";
import Actors from "./views/Actors.js";
import Directors from "./views/Directors.js";
import PersonView from "./views/PersonView.js";
import MySeriesList from "./views/MySeriesList.js";
import Friends from "./views/Friends.js";
import Favourites from "./views/Favourites.js";
import FavouritesView from "./views/FavouritesView.js";
import Awards from "./views/Awards.js";
import AwardView from "./views/AwardView.js";
import Login from "./views/Login.js";
import Signup from "./views/Signup.js";
import Connection from "./signalR/hubConnection.js"; 

var view;

const connection = new Connection();

async function connect()
{
    if(localStorage.logged==1)
    {
        await connection.start();
        connection.listen();
        try {
            await connection.register(parseInt(localStorage.userid));   
        } catch (err) {
            console.error(err);
        }
    }
  
}

if(localStorage.logged==0 || !localStorage.logged)
{
    var html=document.body.querySelector(".topnav").innerHTML;
    html+=
    `<a href="/login" id="login" class="nav__link" data-link>Login</a>
    <a href="/signup" id="signup" class="nav__link" data-link>Signup</a>`;
    document.body.querySelector(".topnav").innerHTML=html;
}
else
{
    var html=document.body.querySelector(".topnav").innerHTML;
    html+=
    `<a href="/myserieslist" id="mylist" class="nav__link" data-link>My Series List</a>
    <a href="/friends" id="friends" class="nav__link" data-link>Friends</a>
    <a href="/" id="logout" class="nav__link" logout>Logout</a>
    <a href="#profile-form" id="profile" class="nav__link" style="float:right" data-toggle="modal" data-target="#profile-form" profile>${localStorage.username}</a>`;
    
    document.body.querySelector(".topnav").innerHTML=html;
}

const pathToRegex = path => new RegExp("^" + path.replace(/\//g, "\\/").replace(/:\w+/g, "(.+)") + "$");

const getParams = match => {
    const values = match.result.slice(1);
    const keys = Array.from(match.route.path.matchAll(/:(\w+)/g)).map(result => result[1]);

    return Object.fromEntries(keys.map((key, i) => {
        return [key, values[i]];
    }));
};

const navigateTo = url => {
    history.pushState(null, null, url);
    router();
};

const router = async () => {
    const routes = [
        { path: "/", view: Home },
        { path: "/series", view: Series },
        { path: "/series/:id", view: SeriesView },
        { path: "/actors", view: Actors },
        { path: "/actors/:id", view: PersonView },
        { path: "/directors", view: Directors },
        { path: "/directors/:id", view: PersonView },
        { path: "/myserieslist", view: MySeriesList },
        { path: "/friends", view: Friends },
        { path: "/favourites", view: Favourites },
        { path: "/favourites/:id", view: FavouritesView },
        { path: "/awards", view: Awards },
        { path: "/awards/:id", view: AwardView },
        { path: "/login", view: Login },
        { path: "/signup", view: Signup }
    ];

    // Test each route for potential match
    const potentialMatches = routes.map(route => {
        return {
            route: route,
            result: location.pathname.match(pathToRegex(route.path))
        };
    });

    let match = potentialMatches.find(potentialMatch => potentialMatch.result !== null);

    if (!match) {
        match = {
            route: routes[0],
            result: [location.pathname]
        };
    }

    view = new match.route.view(getParams(match));

    document.querySelector("#app").innerHTML = await view.getHtml();
    if(window.location.href.includes("myserieslist"))
    {
        handleSubscribe(connection);
    }
};

window.addEventListener("popstate", router);

document.addEventListener("DOMContentLoaded", async () => {
    connect();
    document.body.addEventListener("click", e => {
        if (e.target.matches("[data-link]")) {
            e.preventDefault();
            navigateTo(e.target.href);
        }

        if (e.target.matches("[loginbtn]")) {
            e.preventDefault();
            handleLogin();
        }

        if (e.target.matches("[signupbtn]")) {
            e.preventDefault();
            handleSignup();
        }

        if (e.target.matches("[logout]")) {
            e.preventDefault();
            logout();
            navigateTo(e.target.href);
        }

        if (e.target.matches("[addSeriesBtn]")) {
            e.preventDefault();
            handleAddSeries();
        }

        if (e.target.matches("[editSeriesBtn]")) {
            e.preventDefault();
            handleEditSeries();
        }

        if (e.target.matches("[deleteSeriesBtn]")) {
            e.preventDefault();
            handleDeleteSeries();
        }

        if (e.target.matches("[addSeriesToListBtn]")) {
            e.preventDefault();
            handleAddSeriesToList();
        }

        if (e.target.matches("[addPersonBtn]")) {
            e.preventDefault();
            handleAddPerson();
        }

        if (e.target.matches("[editPersonBtn]")) {
            e.preventDefault();
            handleEditPerson();
        }

        if (e.target.matches("[deletePersonBtn]")) {
            e.preventDefault();
            handleDeletePerson();
        }

        if (e.target.matches("[editMyListBtn]")) {
            e.preventDefault();
            view.EditList();
            navigateTo("/myserieslist");
        }

        if (e.target.matches("[addRoleBtn]")) {
            e.preventDefault();
            handleAddRole();
        }

        if (e.target.matches("[sendFriendRequestBtn]")) {
            e.preventDefault();
            handleSendFriendRequest();
        }

        if (e.target.matches("[confirmRequestBtn]")) {
            e.preventDefault();
            var array = e.target.id.split(" ");
            const requestID=array[0];
            const senderID=array[1];
            handleConfirmRequest(requestID, senderID);
        }

        if (e.target.matches("[removeRequestBtn]")) {
            e.preventDefault();
            var array = e.target.id.substring(1).split(" ");
            const requestID=array[0];
            const senderID=array[1];
            handleRemoveRequest(requestID, senderID);
        }

        if (e.target.matches("[btnSaveSubscriptionChanges]")) {
            e.preventDefault();
            handleSubscribeClick(connection);
        }

        if (e.target.matches("[btnAcceptRecommendation]")) {
            e.preventDefault();
            view.AcceptRecommendation(e.target.id);
        }

        if (e.target.matches("[btnRemoveRecommendation]")) {
            e.preventDefault();
            view.DeleteRecommendation(e.target.id);
        }

        if (e.target.matches("[profile]")) {
            e.preventDefault();
            handleFillProfileForm();
        }

        if (e.target.matches("[editprofilebtn]")) {
            e.preventDefault();
            handleEditProfile();
        }

        if (e.target.matches("[closeEditForm]")) {
            e.preventDefault();
            jQuery("#profile-form").modal('hide');
        }

        if (e.target.matches("[addAwardBtn]")) {
            e.preventDefault();
            handleAddAward();
        }

        if (e.target.matches("[editAwardBtn]")) {
            e.preventDefault();
            handleEditAward();
        }

        if (e.target.matches("[deleteAwardBtn]")) {
            e.preventDefault();
            handleDeleteAward();
        }

        if (e.target.matches("[giveAwardBtn]")) {
            e.preventDefault();
            handleGiveAward();
        }

        if (e.target.matches("[changeAwardBtn]")) {
            e.preventDefault();
            handleChangeAward();
        }

        if (e.target.matches("[removeAwardBtn]")) {
            e.preventDefault();
            handleRemoveAward();
        }

        if (e.target.matches("[filterBtn]")) {
            e.preventDefault();
            handleFilter();
        }

        if(window.location.href=="http://localhost:5001/myserieslist")
        {
            if (e.target.id.includes("SS")) {
                e.preventDefault();
                handleEditEntry(e.target.id.split(" ")[1]);
            };

            if (e.target.id.includes("RS")) {
                e.preventDefault();
                handleDeleteEntry(e.target.id.split(" ")[1]);
            };
        }

        if(window.location.href.includes("http://localhost:5001/series/"))
        {
            if (e.target.id.includes("SR")) {
                e.preventDefault();
                handleEditRole(e.target.id.split(" ")[1]);
            };

            if (e.target.id.includes("RR")) {
                e.preventDefault();
                handleDeleteRole(e.target.id.split(" ")[1]);
            };

            if (e.target.id.includes("SA")) {
                e.preventDefault();
                handleChangeAward(e.target.id.split(" ")[1]);
            };

            if (e.target.id.includes("RA")) {
                e.preventDefault();
                handleRemoveAward(e.target.id.split(" ")[1]);
            };

        }

        if(window.location.href.includes("http://localhost:5001/friends"))
        {
            if (e.target.id.includes("RF")) {
                e.preventDefault();
                handleUnfriend(e.target.id.split(" ")[1]);
            };
        }
    });

    router();
});

async function handleLogin()
{
    await view.login();
    if(localStorage.logged!=0)
    {
        navigateTo("/");
        location.reload();
    } 
}

async function handleSignup()
{
    await view.signup();
    if(localStorage.logged!=0)
    {
        navigateTo("/");
        location.reload();
    } 
}

async function handleAddSeries()
{
    await view.AddSeries();
    location.reload();
}

async function handleEditSeries()
{
    await view.EditSeries();
    location.reload();
}

async function handleDeleteSeries()
{
    await view.DeleteSeries();
    navigateTo("/series");
}

async function handleAddSeriesToList()
{
    await view.AddSeriesToList();
    navigateTo("/series");
}

async function handleAddPerson()
{
    await view.AddPerson();
    location.reload();
}

async function handleEditPerson()
{
    await view.EditPerson();
    location.reload();
}

async function handleDeletePerson()
{
    await view.DeletePerson();
    navigateTo("/");
}

async function handleAddRole()
{
    await view.AddRole();
    location.reload();
}

async function handleDeleteRole(id)
{
    await view.DeleteRole(id);
    location.reload();
}

async function handleEditRole(id)
{
    await view.EditRole(id);
    location.reload();
}

async function handleSendFriendRequest()
{
    await view.SendFriendRequest();
}

async function handleConfirmRequest(requestID, senderID)
{
    await view.ConfirmRequest(requestID, senderID);
    location.reload();
}

async function handleRemoveRequest(requestID, senderID)
{
    await view.RemoveRequest(requestID, senderID);
    location.reload();
}

async function handleEditEntry(id)
{
    await view.EditEntry(id);
    location.reload();
}

async function handleDeleteEntry(id)
{
    await view.DeleteEntry(id);
    location.reload();
}

async function handleUnfriend(id)
{
    await view.Unfriend(id);
    location.reload();
}

async function handleSubscribe(connection)
{
    await view.Subscribe(connection);
}

async function handleSubscribeClick(connection)
{
    await view.Subscribe(connection);
    alert("Changes saved!");
}

async function handleFillProfileForm()
{
    await fillEditProfileForm();
}

async function handleEditProfile()
{
    const name = document.querySelector('#edit-name').value;
    const email = document.querySelector('#edit-email').value;
    const username = document.querySelector('#edit-username').value;
    const password = document.querySelector('#edit-password').value;

    const response =  await fetch("https://localhost:44365/User/"+localStorage.userid, { method: "PUT",
        headers: {
        "Content-Type": "application/json"
        },
        body: JSON.stringify({"id":parseInt(localStorage.userid), "name": name, "email": email, "username": username, "password": password })
    });

    if (!response.ok) {
        alert("Error!");
    }
    else
    {
        const json = await response.json();

        localStorage.userid=json["id"];
        localStorage.username=username;

        alert("Changes saved!"); 
        jQuery("#profile-form").modal('hide');
    }     
}

async function handleAddAward()
{
    await view.AddAward();
    window.location.reload();
}

async function handleEditAward()
{
    await view.EditAward();
    window.location.reload();
}

async function handleDeleteAward()
{
    await view.DeleteAward();
    navigateTo("/awards");
}

async function handleGiveAward()
{
    await view.GiveAward();
    window.location.reload();
}

async function handleChangeAward(id)
{
    await view.ChangeAward(id);
    window.location.reload();
}

async function handleRemoveAward(id)
{
    await view.RemoveAward(id);
    window.location.reload();
}

async function handleFilter()
{
    await view.Filter();
}

async function fillEditProfileForm()
{
    const response =  await fetch("https://localhost:44365/User/"+localStorage.userid, { method: "GET"});
    if (response.ok) {
        const data = await response.json();
        document.querySelector('#edit-name').value = data.name;
        document.querySelector('#edit-email').value = data.email;
        document.querySelector('#edit-username').value = data.username;
        document.querySelector('#edit-username').disabled = true;
        document.querySelector('#edit-password').value = data.password;

        jQuery("#profile-form").modal('toggle');
    }
    else
    {
        alert("Error"); 
    }         
}

function logout()
{
    localStorage.userid=0;
    localStorage.logged=0;
    navigateTo("/");
    location.reload();
}