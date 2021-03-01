import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Home");
    }

    async getHtml() 
    {
        var html,i=0;

        html=`
        <h1>Most Popular Series</h1>
        <br/>
        <table class="table table-striped">
            <thead>
                <tr>
                <th scope="col">#</th>
                <th scope="col">Title</th>
                <th scope="col">Year</th>
                <th scope="col">Genre</th>
                <th scope="col">Rating</th>
                <th scope="col">Popularity</th>
                </tr>
            </thead>
            <tbody>`;

        await fetch("https://localhost:44365/UserSeriesList/GetMostPopularSeries", {method: "GET"})
        .then(p => p.json().then(data => {
            data.forEach(d => {
                html+=`
                <tr>
                <th scope="row">${++i}</th>
                <td><a href="/series/${d.seriesID}" data-link>${d.title}</a></td>
                <td>${d.year}</td>
                <td>${d.genre}</td>
                <td>${(d.rating === 0)? "Not rated" : d.rating}</td>
                <td>${d.popularity}</td>
                </tr>`;
            });
        }));

        html+=`
        </tbody>
        </table>

        <br/>

        <h1>Best Rated Series</h1>
        <br/>
        <table class="table table-striped">
            <thead>
                <tr>
                <th scope="col">#</th>
                <th scope="col">Title</th>
                <th scope="col">Year</th>
                <th scope="col">Genre</th>
                <th scope="col">Rating</th>
                </tr>
            </thead>
            <tbody>`;

        i=0;
        await fetch("https://localhost:44365/Series/GetBestRatedSeries", {method: "GET"})
        .then(p => p.json().then(data => {
            data.forEach(d => {
                html+=`
                <tr>
                <th scope="row">${++i}</th>
                <td><a href="/series/${d.seriesID}" data-link>${d.title}</a></td>
                <td>${d.year}</td>
                <td>${d.genre}</td>
                <td>${d.rating}</td>
                </tr>`;
            });
        }));

        html+=`
        </tbody>
        </table>

        <br/>`;

        return html;
    }
}