import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";

export default class extends AbstractView {
    constructor(params) 
    {
        super(params);
        this.setTitle("All Series");
    }

    async getHtml() 
    {
        var html,i;
        await fetch("https://localhost:44365/Series", {method: "GET"})
        .then(p => p.json().then(data => {
                i=0;
                html=`
                    <h1>All Series</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Title</th>
                            <th scope="col">Year</th>
                            <th scope="col">Genre</th>
                            <th scope="col">Plot</th>
                            <th scope="col">Seasons</th>
                            <th scope="col">Rating</th>
                            </tr>
                        </thead>
                        <tbody>`;

            data.forEach(d => {
                    const series = new Series(d["id"], d["title"], d["year"], d["genre"], d["plot"], d["seasons"], d["rating"]);

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/series/${series.id}" data-link>${series.title}</a></td>
                        <td>${series.year}</td>
                        <td>${series.genre}</td>
                        <td>${series.plot}</td>
                        <td>${series.seasons}</td>
                        <td>`+ +(Math.round(series.rating + "e+1") + "e-1")+`</td>
                        </tr>`;
                 });

            html+=`
                </tbody>
                </table>
                <p>
                    <a href="/series" data-link>View most watched series</a>.
                </p>

                <br/>

                <form id="addseries-form" style="width:50%">
                <div class="form-group col-md-8">
                    <div class="form-group col-md-8">
                    <label for="inputTitle">Title</label>
                    <input type="text" class="form-control" id="inputTitle" placeholder="Title">
                    </div>
                    <div class="form-group col-md-3">
                    <label for="inputYear">Year</label>
                    <input type="number" class="form-control" id="inputYear" placeholder="Year">
                    </div>
                </div>
                <div class="form-group col-md-4">
                    <label for="inputGenre">Genre</label>
                    <select id="inputGenre" class="form-control">
                        <option selected>Select Genre</option>
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
                    <textarea type="text" class="form-control" id="inputPlot" placeholder=""></textarea>
                    </div>
                    <div class="form-group col-md-2">
                    <label for="inputZip">Seasons</label>
                    <input type="number" class="form-control" id="inputSeasons">
                    </div>
                </div>
                <button type="submit" class="btn btn-primary" style="width:20%" addSeriesBtn>Add Series</button>
                </form>`;
        }));

        return html;
    }

    AddSeries()
    {
        const addSeriesForm = document.querySelector('#addseries-form');
        const title = addSeriesForm['inputTitle'].value;
        const year = parseInt(addSeriesForm['inputYear'].value);  
        const genre = addSeriesForm['inputGenre'].value;
        const plot = addSeriesForm['inputPlot'].value;  
        const seasons = parseInt(addSeriesForm['inputSeasons'].value);
        
        fetch("https://localhost:44365/Series", { method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ "title": title, "year": year, "genre":genre , "plot":plot , "seasons":seasons})
            }).then(p => {
            if (p.ok) {
                addSeriesForm.reset();
                alert("Series "+title+" added to database!");
            }
            }
        );
    }
}


