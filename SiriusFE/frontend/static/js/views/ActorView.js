
import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";
import {Actor} from "../models/Actor.js";
import {Role} from "../models/Role.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.setTitle("Viewing Actor");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Actor/GetActor/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
                
                const actor = new Actor(d[0]["id"], d[0]["name"], d[0]["birthplace"], d[0]["birthday"], d[0]["biography"]);

                html=`
                    <h1>Actor: ${actor.name}</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">Birthplace</th>
                            <th scope="col">Birthday</th>
                            </tr>
                        </thead>
                        <tbody>`;

                    html+=`
                        <tr>
                        <td>${actor.birthplace}</td>
                        <td>${actor.birthday}</td>
                        </tr>`;

                    html+=`
                        </tbody>
                        </table>
                        <p>
                            <a href="/series" data-link>Add actor to your list <tmp></a>.
                        </p>
                        <p>
                            ${actor.biography}
                        </p>
                        <br/>`;
        }));


        await fetch("https://localhost:44365/Role/GetActorRoles/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
                i=0;

                html+=`
                    <h2>Roles</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Role</th>
                            <th scope="col">Series</th>
                            </tr>
                        </thead>
                        <tbody>`;

                d.forEach(data => {

                    const actor = new Actor(data["actor"]["id"], data["actor"]["name"], data["actor"]["birthplace"], data["actor"]["birthday"], data["actor"]["biography"]);
                    const series = new Series(data["series"]["id"], data["series"]["title"], data["series"]["year"], data["series"]["genre"], data["series"]["plot"], data["series"]["seasons"], data["series"]["rating"]);
                    const role = new Role(data["id"], actor, series, data["inRole"]);

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td>${role.inrole}</td>
                        <td><a href="/series/${series.id}" data-link>${role.series.title}</a></td>
                        </tr>`;
                });
        }));

        return html;

    }
}
