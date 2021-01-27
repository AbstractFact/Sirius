import Home from "./views/Home.js";
import Series from "./views/Series.js";
import SeriesView from "./views/SeriesView.js";
import Actors from "./views/Actors.js";
import ActorView from "./views/ActorView.js";
import MySeriesList from "./views/MySeriesList.js";
import Login from "./views/Login.js";
import Signup from "./views/Signup.js";

var view;

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
        { path: "/actors/:id", view: ActorView },
        { path: "/myserieslist", view: MySeriesList },
        { path: "/login", view: Login },
        { path: "/signup", view: Signup },
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

    //const view = new match.route.view(getParams(match));
    view = new match.route.view(getParams(match));

    document.querySelector("#app").innerHTML = await view.getHtml();
};

window.addEventListener("popstate", router);

document.addEventListener("DOMContentLoaded", () => {
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
            view.AddSeriesToList();
            navigateTo("/series");
        }

        if (e.target.matches("[addActorBtn]")) {
            e.preventDefault();
            handleAddActor();
        }

        if (e.target.matches("[editActorBtn]")) {
            e.preventDefault();
            handleEditActor();
        }

        if (e.target.matches("[deleteActorBtn]")) {
            e.preventDefault();
            handleDeleteActor();
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

        if(window.location.href=="http://localhost:5060/myserieslist")
        {
            const entries = view.GetEntries();
            entries.forEach(entrie => {
                const id = entrie.id;

                if (e.target.id==id) {
                    e.preventDefault();
                    view.EditEntry(id);
                };

                if (e.target.id=="R"+id) {
                    e.preventDefault();
                    view.DeleteEntry(id);
                };
            });
        }

        if(window.location.href.includes("http://localhost:5060/series/"))
        {
            const roles = view.GetRoles();
            roles.forEach(role => {
                const id = role.id;

                if (e.target.id==id) {
                    e.preventDefault();
                    view.EditRole(id);
                    location.reload();
                };

                if (e.target.id=="R"+id) {
                    e.preventDefault();
                    view.DeleteRole(id);
                    location.reload();
                };
            });
        }
    });

    router();
});

async function handleLogin()
{
    await view.login();
    if(localStorage.username!=0)
    {
        navigateTo("/");
    } 
}

async function handleSignup()
{
    await view.signup();
    if(localStorage.username!=0)
    {
        navigateTo("/");
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

async function handleAddActor()
{
    await view.AddActor();
    location.reload();
}

async function handleEditActor()
{
    await view.EditActor();
    location.reload();
}

async function handleDeleteActor()
{
    await view.DeleteActor();
    navigateTo("/actors");
}

async function handleAddRole()
{
    await view.AddRole();
    location.reload();
}

function logout()
{
    document.querySelector("#login").style.display="block";
    document.querySelector("#signup").style.display="block";
    document.querySelector("#mylist").style.display="none";
    document.querySelector("#logout").style.display="none";
    localStorage.username=0;
    localStorage.userid=null;
}