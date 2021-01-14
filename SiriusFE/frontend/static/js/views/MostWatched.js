import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Most Watched");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Series", {method: "GET"})
        .then(p => p.json().then(data => {
                i=0;
                html=`
                    <h1>Most watched series</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">ID</th>
                            <th scope="col">Title</th>
                            <th scope="col">Year</th>
                            </tr>
                        </thead>
                        <tbody>`;

            data.forEach(d => {
                    const series = new Series(d["id"], d["title"], d["year"]);

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td>${series.id}</td>
                        <td>${series.title}</td>
                        <td>${series.year}</td>
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