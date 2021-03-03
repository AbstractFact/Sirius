import AbstractView from "./AbstractView.js";
import {Person} from "../models/Person.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("All Actors");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Person/GetAllActors", {method: "GET"})
        .then(p => p.json().then(data => {
            i=0;
            html=`
                <h1>All Actors</h1>
                <br/>
                <div style="display:inline-block; width:100%;">
                    <form id="filter-form" style="width:100%">
                        <div style="display:inline-block; width:38%">
                            <div class="form-group col-md-10">
                            <label for="filterName">Name: </label>
                            <input type="text" style="width:70%" class="form-control" id="filterName" placeholder="Name">
                            </div>
                        </div>
                        <div style="display:inline-block; width:38%">
                            <label for="filterSex">Sex: </label>
                            <select id="filterSex" class="form-control">
                                    <option selected>All</option>
                                    <option>Male</option>
                                    <option>Female</option>
                            </select>
                        </div>
                        <button type="submit" class="btn btn-primary" style="width:15%; float:right;" filterBtn>Filter</button>
                    </form>
                </div>
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
                    <tbody id="tcontent">`;

            data.forEach(d => {
                    const actor = new Person(d["actor"]["id"], d["actor"]["name"], d["actor"]["sex"], d["actor"]["birthplace"], d["actor"]["birthday"], d["actor"]["biography"]);

                    html+=`
                    <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/actors/${actor.id}" data-link>${actor.name}</a></td>
                        <td>${actor.sex}</td>
                        <td>${actor.birthplace}</td>
                        <td>${actor.birthday}</td>
                    </tr>`;
            });

            html+=`
                </tbody>
                </table>

                <br/>`;
        }));

        return html;
    }

    async Filter()
    {
        const filterForm = document.querySelector('#filter-form');
        const name = filterForm['filterName'].value;
        const sex = filterForm['filterSex'].value;
        const table = document.body.querySelector("#tcontent");
        table.innerHTML=``;
        var i=0;

        await fetch("https://localhost:44365/Person/GetActorsFiltered", {method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ "name": name, "sex": sex })
        })
        .then(p => p.json().then(data => {
            data.forEach(d => {
                table.innerHTML+=`
                <tr>
                <th scope="row">${++i}</th>
                <td><a href="/actors/${d["id"]}" data-link>${d["name"]}</a></td>
                <td>${d["sex"]}</td>
                <td>${d["birthplace"]}</td>
                <td>${d["birthday"]}</td>
                </tr>`;
            });
        }));
          
    }

}