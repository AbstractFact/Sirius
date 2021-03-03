import AbstractView from "./AbstractView.js";
import {Person} from "../models/Person.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("All Directors");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Person/GetAllDirectors", {method: "GET"})
        .then(p => p.json().then(data => {
            i=0;
            html=`
                <h1>All Directors</h1>
                <br/>
                <table class="table table-striped">
                    <thead>
                        <tr>
                        <th scope="col">#</th>
                        <th scope="col">Name</th>
                        <th scope="col">Sex</th>
                        <th scope="col">Birthplace</th>
                        <th scope="col">Birthday</th>
                        </tr>
                    </thead>
                    <tbody>`;

            data.forEach(d => {
                    const director = new Person(d["director"]["id"], d["director"]["name"], d["director"]["sex"], d["director"]["birthplace"], d["director"]["birthday"], d["director"]["biography"]);

                    html+=`
                    <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/directors/${director.id}" data-link>${director.name}</a></td>
                        <td>${director.sex}</td>
                        <td>${director.birthplace}</td>
                        <td>${director.birthday}</td>
                    </tr>`;
            });

            html+=`
                </tbody>
                </table>

                <br/>`;
        }));

        return html;
    }

}