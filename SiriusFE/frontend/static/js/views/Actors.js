import AbstractView from "./AbstractView.js";
import {Actor} from "../models/Actor.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("All Actors");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Actor", {method: "GET"})
        .then(p => p.json().then(data => {
                i=0;
                html=`
                    <h1>All Actors</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Name</th>
                            <th scope="col">Birthplace</th>
                            <th scope="col">Birthday</th>
                            </tr>
                        </thead>
                        <tbody>`;

            data.forEach(d => {
                    const actor = new Actor(d["id"], d["name"], d["birthplace"], d["birthday"], d["biography"]);

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/actors/${actor.id}" data-link>${actor.name}</a></td>
                        <td>${actor.birthplace}</td>
                        <td>${actor.birthday}</td>
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
    }
}