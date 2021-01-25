import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("All Series");
    }

    async getHtml() 
    {
        var html,i;
        console.log(localStorage.username);
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
                </p>`;
        }));

        return html;


        // //     const prodavnica = new Prodavnica(d["ime"], d["id"]);
        // //     d["proizvodi"].forEach(pr =>{
        // //         prodavnica.dodajProizvod(new Proizvod(pr["id"],pr["sifra"],
        // //         pr["ime"],pr["cena"],pr["kolicina"]));
        // //     });
        // //     prodavnica.crtaj(document.body);
        // });
    }
}