import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";
import {Actor} from "../models/Actor.js";
import {User} from "../models/User.js";


export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Home");
    }

    getContent()
    {
        fetch("https://localhost:44365/Series", {method: "GET"})
        .then(p => p.json().then(data => {
                data.forEach(d => {
                    const series = new Series(d["id"], d["title"], d["year"]);
                    series.print();

                //     const prodavnica = new Prodavnica(d["ime"], d["id"]);
                //     d["proizvodi"].forEach(pr =>{
                //         prodavnica.dodajProizvod(new Proizvod(pr["id"],pr["sifra"],
                //         pr["ime"],pr["cena"],pr["kolicina"]));
                //     });
                //     prodavnica.crtaj(document.body);

            });
        }));

            fetch("https://localhost:44365/Actor", {method: "GET"})
        .then(p => p.json().then(data => {
                data.forEach(d => {
                    const actor = new Actor(d["id"], d["name"], d["birthplace"], d["birthday"], d["biography"]);
                    actor.print();

                //     const prodavnica = new Prodavnica(d["ime"], d["id"]);
                //     d["proizvodi"].forEach(pr =>{
                //         prodavnica.dodajProizvod(new Proizvod(pr["id"],pr["sifra"],
                //         pr["ime"],pr["cena"],pr["kolicina"]));
                //     });
                //     prodavnica.crtaj(document.body);
                });
            }));

            fetch("https://localhost:44365/User", {method: "GET"})
            .then(p => p.json().then(data => {
                    data.forEach(d => {
                        const user = new User(d["id"], d["username"], d["password"]);
                        user.print();
    
                    //     const prodavnica = new Prodavnica(d["ime"], d["id"]);
                    //     d["proizvodi"].forEach(pr =>{
                    //         prodavnica.dodajProizvod(new Proizvod(pr["id"],pr["sifra"],
                    //         pr["ime"],pr["cena"],pr["kolicina"]));
                    //     });
                    //     prodavnica.crtaj(document.body);
                    });
                }));
    }

    async getHtml() 
    {
        this.getContent();
        return `
            <h1>Welcome to Sirius</h1>
            <br/>
            <table class="table table-striped">
                <thead>
                    <tr>
                    <th scope="col">#</th>
                    <th scope="col">First</th>
                    <th scope="col">Last</th>
                    <th scope="col">Handle</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                    <th scope="row">1</th>
                    <td>Mark</td>
                    <td>Otto</td>
                    <td>@mdo</td>
                    </tr>
                    <tr>
                    <th scope="row">2</th>
                    <td>Jacob</td>
                    <td>Thornton</td>
                    <td>@fat</td>
                    </tr>
                    <tr>
                    <th scope="row">3</th>
                    <td colspan="2">Larry the Bird</td>
                    <td>@twitter</td>
                    </tr>
                </tbody>
                </table>
            <p>
                <a href="/series" data-link>View most watched series</a>.
            </p>
        `;
    }
}