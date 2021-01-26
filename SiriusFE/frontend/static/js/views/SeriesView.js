import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";
import {Actor} from "../models/Actor.js";
import {Role} from "../models/Role.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.setTitle("Viewing Series");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Series/GetSeries/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
                
                const series = new Series(d[0]["id"], d[0]["title"], d[0]["year"], d[0]["genre"], d[0]["plot"], d[0]["seasons"], d[0]["rating"]);
                html=`
                    <h1>Series: ${series.title}</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">Year</th>
                            <th scope="col">Genre</th>
                            <th scope="col">Plot</th>
                            <th scope="col">Seasons</th>
                            <th scope="col">Rating</th>
                            </tr>
                        </thead>
                        <tbody>`;

                    html+=`
                        <tr>
                        <td>${series.year}</td>
                        <td>${series.genre}</td>
                        <td>${series.plot}</td>
                        <td>${series.seasons}</td>
                        <td>`+ +(Math.round(series.rating + "e+1") + "e-1")+`</td>
                        </tr>`;

                    html+=`
                        </tbody>
                        </table>
                        <p>
                            <a href="/series" data-link>Add series to your list</a>.
                        </p>
                        <p>
                            ${series.plot}
                        </p>
                        <br/>
                        
                        <br/>

                        <div style="display:block">
                        <form id="addseries-form" style="width:50%; float:left;">
                        <div class="form-group col-md-8">
                            <div class="form-group col-md-8">
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

                        console.log(localStorage.username);
                        if(localStorage.username!=0)
                            html+=`<form id="addseriestolist-form" style="width:50%; float:right;">
                            <div class="form-group col-md-8">
                                <label for="inputStatus">Status</label>
                                <select id="inputStatus" class="form-control">
                                    <option selected>Select Status</option>
                                    <option>Watching</option>
                                    <option>Plan to Watch</option>
                                    <option>On Hold</option>
                                    <option>Dropped</option>
                                    <option>Completed</option>
                                </select>
                            </div>
                            <button type="submit" class="btn btn-primary" style="width:30%" addSeriesToListBtn>Add Series to List</button>
                            </form></div>`; 
                        else
                            html+=`</div>`;    
        }));


        await fetch("https://localhost:44365/Role/GetSeriesRoles/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
                i=0;

                html+=`
                <br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/><br/>
                <div style="display:block">
                    <h2>Cast</h2>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Role</th>
                            <th scope="col">Actor</th>
                            </tr>
                        </thead>
                        <tbody>
                </div>`;

                d.forEach(data => {

                    const actor = new Actor(data["actor"]["id"], data["actor"]["name"], data["actor"]["sex"], data["actor"]["birthplace"], data["actor"]["birthday"], data["actor"]["biography"]);
                    const series = new Series(data["series"]["id"], data["series"]["title"], data["series"]["year"], data["series"]["genre"], data["series"]["plot"], data["series"]["seasons"], data["series"]["rating"]);
                    const role = new Role(data["id"], actor, series, data["inRole"]);

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td>${role.inrole}</td>
                        <td><a href="/actors/${role.actor.id}" data-link>${role.actor.name}</a></td>
                        </tr>`;
                });
        }));

        return html;
    }

    EditSeries()
    {
        const addSeriesForm = document.querySelector('#addseries-form');
        const title = addSeriesForm['inputTitle'].value;
        const year = parseInt(addSeriesForm['inputYear'].value);  
        const genre = addSeriesForm['inputGenre'].value;
        const plot = addSeriesForm['inputPlot'].value;  
        const seasons = parseInt(addSeriesForm['inputSeasons'].value);
        
        fetch("https://localhost:44365/Series/"+this.postId, { method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({"title": title, "year": year, "genre":genre , "plot":plot , "seasons":seasons})
            }).then(p => {
            if (p.ok) {
                alert("Series "+title+" edited!");
            }
            }
        );
    }

    DeleteSeries()
    {
        fetch("https://localhost:44365/Series/"+this.postId, { method: "DELETE"}).then(p => {
            if (p.ok) 
            {
                alert("Series "+title+" deleted!");
            }
        });
    }

    async AddSeriesToList()
    {
        const addSeriesToListForm = document.querySelector('#addseriestolist-form');
        const userid = localStorage.userid;
        const seriesid = this.postId;  
        const status = addSeriesToListForm['inputStatus'].value; 
        
        fetch("https://localhost:44365/UserSeriesList/AddSeriesToList/"+userid+"/"+seriesid+"/"+status, { method: "POST"}).then(p => 
        {
            if (p.ok) {
                alert("Series added to your list!");
            }
        });
    }
}
