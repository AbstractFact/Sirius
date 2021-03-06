import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";
import {Person} from "../models/Person.js";
import {Role} from "../models/Role.js";
import {Award} from "../models/Award.js"
import {Awarded} from "../models/Awarded.js"

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.setTitle("Viewing Series");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Directed/GetSeriesWithDirector/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
                const series = new Series(d["seriesID"], d["title"], d["year"], d["genre"], d["plot"], d["seasons"], d["rating"]);
                html=`
                    <h1>Series: ${series.title}</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">Year</th>
                            <th scope="col">Genre</th>
                            <th scope="col">Director</th>
                            <th scope="col">Seasons</th>
                            <th scope="col">Rating</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                            <td>${series.year}</td>
                            <td>${series.genre}</td>
                            <td><a href="/directors/${d["directorID"]}" data-link>${d["name"]}</a></td>
                            <td>${series.seasons}</td>
                            <td>${(series.rating === 0)? "Not rated" : +(Math.round(series.rating + "e+1") + "e-1")}</td>
                            </tr>
                        </tbody>
                    </table>

                    <p>
                        ${series.plot}
                    </p>
                    <br/>

                    <div style="display:block">`;

                    if(localStorage.username=="Admin" && localStorage.logged==1)
                    html+=`<form id="addseries-form" style="width:50%; float:left;">
                    <div class="form-group col-md-8">
                        <div class="form-group col-md-10">
                        <label for="inputTitle">Title</label>
                        <input type="text" class="form-control" id="inputTitle" value="${series.title}">
                        </div>
                        <div class="form-group col-md-3">
                        <label for="inputYear">Year</label>
                        <input type="number" class="form-control" id="inputYear" value="${series.year}">
                        </div>
                    </div>
                    <div class="form-group col-md-4">
                        <label for="inputGenre">Genre</label>
                        <select id="inputGenre" class="form-control">
                            <option selected>${series.genre}</option>
                            <option>Drama</option>
                            <option>Comedy</option>
                            <option>Crime</option>
                            <option>Fantasy</option>
                            <option>Sci-fi</option>
                        </select>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                        <label for="inputPlot">Plot</label>
                        <textarea type="text" class="form-control" id="inputPlot">${series.plot}</textarea>
                        </div>
                        <div class="form-group col-md-2">
                        <label for="inputZip">Seasons</label>
                        <input type="number" class="form-control" id="inputSeasons" value="${series.seasons}">
                        </div>
                    </div>
                    <button type="submit" class="btn btn-primary" style="width:20%" editSeriesBtn>Edit Series</button>
                    <button type="submit" class="btn btn-danger" style="width:30%; float:right" deleteSeriesBtn>Delete Series</button>
                    </form>`;

                    if(localStorage.logged!=0)
                    {
                        if(localStorage.username!="Admin")
                            html+=`<form id="addseriestolist-form" style="width:50%;">`;
                        else
                            html+=`<form id="addseriestolist-form" style="width:50%; float:right;">`;
                        html+=`<div class="form-group col-md-8">
                            <label for="inputStatus">Status</label>
                            <select id="inputStatus" class="form-control">
                                <option selected>Plan to Watch</option>
                                <option>Watching</option>
                                <option>On Hold</option>
                                <option>Dropped</option>
                                <option>Completed</option>
                            </select>
                            <input type="checkbox" id="inputFav" name="fav">
                            <label for="inputFav"> Favourite</label><br>
                        </div>
                        <button type="submit" class="btn btn-primary" style="width:30%" addSeriesToListBtn>Add Series to List</button>
                        </form></div><br/>`; 
                    }
                    else
                        html+=`</div><br/>`;    
        }));


        await fetch("https://localhost:44365/Role/GetSeriesRoles/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
                i=0;
                
                if(localStorage.username=="Admin" && localStorage.logged==1)
                    html+=`
                    <br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/>`;
                html+=`
                <div style="display:block">
                    <h2>Cast</h2>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Role</th>
                            <th scope="col">Actor</th>`;
                            if(localStorage.username=="Admin" && localStorage.logged==1)
                                html+=`<th scope="col"></th>
                                <th scope="col"></th>`;
                            html+=`</tr>
                        </thead>
                        <tbody>`;

                d.forEach(data => {
                    html+=`
                    <tr id="${data.id}">
                        <th scope="row">${++i}</th>`;

                        if(localStorage.username=="Admin" && localStorage.logged==1)
                            html+=`
                            <td><input type="text" class="form-control" id="editRole" value="${data.inRole}"></td>`;
                        else
                            html+=`
                            <td>${data.inRole}</td>`;

                        html+=`
                        <td><a href="/actors/${data.actorID}" data-link>${data.name}</a></td>`;

                        if(localStorage.username=="Admin" && localStorage.logged==1)
                        html+=`<td>
                            <button type="submit" class="btn btn-primary" style="width:60%" id="SR ${data.id}">Save Changes</button>
                        </td>
                        <td>
                            <button type="submit" class="btn btn-danger" style="width:100%" id="RR ${data.id}">X</button>
                        </td>`;
                        html+=`</tr>`;
                });
        }));

            html+=`</div></tbody></table>`;

            if(localStorage.username=="Admin" && localStorage.logged==1)
            {
                html+=`<form id="addrole-form" style="width:40%">
                <div class="form-group col-md-8">
                    <label for="inputActor">Actors</label>
                    <select id="inputActor" class="form-control">`;
                        
                        
                await fetch("https://localhost:44365/Person", {method: "GET"})
                    .then(p => p.json().then(data => {
                        data.forEach(d => {
                            const actor = new Person(d["id"], d["name"], d["sex"], d["birthplace"], d["birthday"], d["biography"]);

                            html+=`<option value="${actor.id}">${actor.name}, ${actor.birthplace}</option>`;
                        });
                }));

                html+=`</select>
                <label for="inputRole">Role</label>
                <input type="text" class="form-control" id="inputRole" placeholder="Enter role">
                </div>
                <button type="submit" class="btn btn-primary" style="width:30%" addRoleBtn>Add Role</button>
                </form>
                <br/>`;
            }

            await fetch("https://localhost:44365/Awarded/GetSeriesAwards/"+this.postId, {method: "GET"})
            .then(p => p.json().then(d => {
                i=0;

                html+=`
                <div style="display:block">
                    <h2>Awards</h2>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Name</th>
                            <th scope="col">Year</th>`;
                            if(localStorage.username=="Admin" && localStorage.logged==1)
                                html+=`
                                <th scope="col"></th>
                                <th scope="col"></th>`;
                html+=`
                        </tr>
                        </thead>
                        <tbody>`;

                d.forEach(data => {

                    //const award = new Awarded(data["id"], data["award"], data["series"], data["year"]);

                    html+=`
                    <tr id="${data.id}">
                        <th scope="row">${++i}</th>
                        <td><a href="/awards/${data.awardID}" data-link>${data.name}</a></td>`;

                    if(localStorage.username=="Admin" && localStorage.logged==1)
                        html+=`
                        <td><input type="number" class="form-control" id="editYear1" value="${data.year}" style="width:50%"></td>`;
                    else
                        html+=`
                        <td>${data.year}</td>`;

                    if(localStorage.username=="Admin" && localStorage.logged==1)
                        html+=`
                        <td>
                            <button type="submit" class="btn btn-primary" style="width:60%" id="SA ${data.id}">Save Changes</button>
                        </td>
                        <td>
                            <button type="submit" class="btn btn-danger" style="width:100%" id="RA ${data.id}">X</button>
                        </td>`; 

                    html+=`</tr>`;
                });
        }));

            html+=`</div></tbody></table>`;

            if(localStorage.username=="Admin" && localStorage.logged==1)
            {
                html+=`<form id="addaward-form" style="width:40%">
                <div class="form-group col-md-8">
                    <label for="inputAward">Awards</label>
                    <select id="inputAward" class="form-control">`;
                        
                        
                await fetch("https://localhost:44365/Award", {method: "GET"})
                    .then(p => p.json().then(data => {
                        data.forEach(d => {
                            const awa = new Award(d["id"], d["name"], d["description"]);

                            html+=`<option value="${awa.id}">${awa.name}</option>`;
                        });
                }));

                html+=`</select>
                <label for="inputYear1">Year</label>
                <input type="number" class="form-control" id="inputYear1" style="width:40%">
                </div>
                <button type="submit" class="btn btn-primary" style="width:30%" giveAwardBtn>Add Award</button>
                </form>`;
            }

        return html;
    }

    async EditSeries()
    {
        const addSeriesForm = document.querySelector('#addseries-form');
        const title = addSeriesForm['inputTitle'].value;
        const year = parseInt(addSeriesForm['inputYear'].value);  
        const genre = addSeriesForm['inputGenre'].value;
        const plot = addSeriesForm['inputPlot'].value;  
        const seasons = parseInt(addSeriesForm['inputSeasons'].value);

        const response =  await fetch("https://localhost:44365/Series/"+this.postId, 
        { method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"id": parseInt(this.postId), "title": title, "year": year, "genre":genre , "plot":plot , "seasons":seasons})
        });

        if (response.ok) 
        {
            alert("Series "+title+" edited!");
        }
        else
        {
            alert("Error!");
        }
    }

    async DeleteSeries()
    {
        const response =  await fetch("https://localhost:44365/Series/"+this.postId, { method: "DELETE"});

        if (response.ok) {
            alert("Series " + "deleted!");
        }
        else
        {
            alert("Error!");
        }
    }

    async AddSeriesToList()
    {
        const addSeriesToListForm = document.querySelector('#addseriestolist-form');
        const userid = localStorage.userid;
        const seriesid = this.postId;  
        const status = addSeriesToListForm['inputStatus'].value; 
        const fav = addSeriesToListForm['inputFav'].checked;
        
        const response = await fetch("https://localhost:44365/UserSeriesList/AddSeriesToList/"+userid+"/"+seriesid+"/"+status+"/"+fav, { method: "POST"});

        if (response.ok)
            alert("Series added to your list!");
        else
            alert("Series is already in your list!");
    }

    async AddRole()
    {
        const addroleform = document.querySelector("#addrole-form");
        const actorid = addroleform["inputActor"].value;
        const role = addroleform["inputRole"].value;

        const response = await fetch("https://localhost:44365/Role/AddRole/"+actorid+"/"+role+"/"+this.postId, { method: "POST"});

        if(response.ok)
        {
            alert("Role added!");
        }
        else
        {
            alert("Error!");
        }
    }

    async EditRole(id)
    {
        const row = document.getElementById(id);
        const role = row.querySelector('#editRole').value;

        const response = await fetch("https://localhost:44365/Role/"+id+"/"+role, { method: "PUT"});

        if(response.ok)
        {
            alert("Role edited!");
        }
        else
        {
            alert("Error!");
        }
    }

    async DeleteRole(id)
    {
        const response = await fetch("https://localhost:44365/Role/"+id, { method: "DELETE"});
        if(response.ok)
        {
            alert("Role added!");
        }
        else
        {
            alert("Error!");
        }
    }

    async GiveAward()
    {
        const addawardform = document.querySelector("#addaward-form");
        const awardid = addawardform["inputAward"].value;
        const year = addawardform["inputYear1"].value;

        const response = await fetch("https://localhost:44365/Awarded/AddAwardSeries/"+awardid+"/"+year+"/"+this.postId, { method: "POST"});

        if(response.ok)
        {
            alert("Award added!");
        }
        else
        {
            alert("Error!");
        }
    }

    async ChangeAward(id)
    {
        const row = document.getElementById(id);
        const year = parseInt(row.querySelector('#editYear1').value);

        const response = await fetch("https://localhost:44365/Awarded/"+parseInt(id)+"/"+year, { method: "PUT"});

        if(response.ok)
        {
            alert("Award edited!");
        }
        else
        {
            alert("Error!");
        }
    }

    async RemoveAward(id)
    {
        const response = await fetch("https://localhost:44365/Awarded/"+id, { method: "DELETE"});
        if(response.ok)
        {
            alert("Award edited!");
        }
        else
        {
            alert("Error!");
        }
    }
}
