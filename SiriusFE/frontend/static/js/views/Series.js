import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";
import {Person} from "../models/Person.js";

export default class extends AbstractView {
    constructor(params) 
    {
        super(params);
        this.setTitle("All Series");
    }

    async getHtml() 
    {
        var html,i=0;

        html=`
        <h1>All Series</h1>
        <br/>
        <div style="display:inline-block; width:100%;">
        <form id="filter-form" style="width:100%">
            <div style="display:inline-block; width:38%">
                <div class="form-group col-md-10">
                <label for="filterTitle">Title: </label>
                <input type="text" style="width:70%" class="form-control" id="filterTitle" placeholder="Title">
                </div>
            </div>
            <div style="display:inline-block; width:38%">
                <label for="filterGenre">Genre: </label>
                <select id="filterGenre" class="form-control">
                        <option selected>All</option>
                        <option>Drama</option>
                        <option>Comedy</option>
                        <option>Crime</option>
                        <option>Fantasy</option>
                        <option>Sci-fi</option>
                </select>
            </div>
            <button type="submit" class="btn btn-primary" style="width:15%; float:right;" filterBtn>Filter</button>
        </form>
        </div>
        </br>
        </br>
        <table class="table table-striped">
            <thead>
                <tr>
                <th scope="col">#</th>
                <th scope="col">Title</th>
                <th scope="col">Year</th>
                <th scope="col">Genre</th>
                <th scope="col">Seasons</th>
                <th scope="col">Rating</th>
                </tr>
            </thead>
            <tbody id="tcontent">`;

        await fetch("https://localhost:44365/Series", {method: "GET"})
        .then(p => p.json().then(data => {

            data.forEach(d => {
                const series = new Series(d["id"], d["title"], d["year"], d["genre"], d["plot"], d["seasons"], d["rating"]);

                html+=`
                    <tr>
                    <th scope="row">${++i}</th>
                    <td><a href="/series/${series.id}" data-link>${series.title}</a></td>
                    <td>${series.year}</td>
                    <td>${series.genre}</td>
                    <td>${series.seasons}</td>
                    <td>${(series.rating === 0)? "Not rated" : +(Math.round(series.rating + "e+1") + "e-1")}</td>
                    </tr>`;
            });
        }));

        html+=`
            </tbody>
            </table>
            <br/>`;

            if(localStorage.username=="Admin" && localStorage.logged==1)
            {
                html+=`<form id="addseries-form" style="width:50%">
                    <div class="form-group col-md-10">
                        <div class="form-group col-md-10">
                        <label for="inputTitle">Title</label>
                        <input type="text" class="form-control" id="inputTitle" placeholder="Title">
                        </div>
                        <div class="form-group col-md-3">
                        <label for="inputYear">Year</label>
                        <input type="number" class="form-control" id="inputYear" placeholder="Year">
                        </div>
                    </div>
                    <div class="form-group col-md-6">
                        <label for="inputGenre">Select Genre</label>
                        <select id="inputGenre" class="form-control">
                            <option selected>Drama</option>
                            <option>Comedy</option>
                            <option>Crime</option>
                            <option>Fantasy</option>
                            <option>Sci-fi</option>
                        </select>
                    </div>
                    <div class="form-group col-md-6">
                        <label for="inputDirector">Director</label>
                        <select id="inputDirector" class="form-control">`;
                    
                            await fetch("https://localhost:44365/Person", {method: "GET"})
                            .then(p => p.json().then(data => {
                                data.forEach(d => {
                                    const director = new Person(d["id"], d["name"], d["sex"], d["birthplace"], d["birthday"], d["biography"]);
        
                                    html+=`<option value="${director.id}">${director.name}, ${director.birthplace}</option>`;
                                });
                        }));

                    html+=`</select>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label for="inputPlot">Plot</label>
                            <textarea type="text" class="form-control" id="inputPlot" placeholder=""></textarea>
                        </div>
                        <div class="form-group col-md-2">
                            <label for="inputZip">Seasons</label>
                            <input type="number" class="form-control" id="inputSeasons" min=1 value=1>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-primary" style="width:20%" addSeriesBtn>Add Series</button>
                </form>`;
            }

        return html;
    }

    async AddSeries()
    {
        const addSeriesForm = document.querySelector('#addseries-form');
        const title = addSeriesForm['inputTitle'].value;
        const year = parseInt(addSeriesForm['inputYear'].value);  
        const genre = addSeriesForm['inputGenre'].value;
        const plot = addSeriesForm['inputPlot'].value;  
        const seasons = parseInt(addSeriesForm['inputSeasons'].value);
        const directorID = addSeriesForm["inputDirector"].value;

        const response =  await  fetch("https://localhost:44365/Series/"+parseInt(directorID), { method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ "title": title, "year": year, "genre":genre , "plot":plot , "seasons":seasons})
        });

        if (response.ok) {
            addSeriesForm.reset();
            alert("Series "+title+" added to database!");
        } 
        else
        {
            alert("Error!");
        }  
    }

    async Filter()
    {
        const filterForm = document.querySelector('#filter-form');
        const title = filterForm['filterTitle'].value;
        const genre = filterForm['filterGenre'].value;
        const table = document.body.querySelector("#tcontent");

        await fetch("https://localhost:44365/Series/GetSeriesFiltered", {method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ "title": title, "genre": genre })
        })
        .then(p => p.json().then(data => {
            table.innerHTML=``;
            var i=0;
            data.forEach(d => {
                table.innerHTML+=`
                <tr>
                <th scope="row">${++i}</th>
                <td><a href="/series/${d["id"]}" data-link>${d["title"]}</a></td>
                <td>${d["year"]}</td>
                <td>${d["genre"]}</td>
                <td>${d["seasons"]}</td>
                <td>${(d["rating"] === 0)? "Not rated" : +(Math.round(d["rating"] + "e+1") + "e-1")}</td>
                </tr>`;
            });
        }));   
    }
}


