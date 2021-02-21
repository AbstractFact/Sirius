
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
            const actor = new Actor(d["id"], d["name"], d["sex"], d["birthplace"], d["birthday"], d["biography"]);

            html=`
                <h1>Actor: ${actor.name}</h1>
                <br/>
                <table class="table table-striped">
                    <thead>
                        <tr>
                        <th scope="col">Sex</th>
                        <th scope="col">Birthplace</th>
                        <th scope="col">Birthday</th>
                        </tr>
                    </thead>
                    <tbody>`;

                html+=`
                    <tr>
                    <td>${actor.sex}</td>
                    <td>${actor.birthplace}</td>
                    <td>${actor.birthday}</td>
                    </tr>`;

                html+=`
                    </tbody>
                    </table>
                    <p>
                        ${actor.biography}
                    </p>

                    <br/>
                    
                    <form id="addactor-form" style="width:50%;">
                    <div class="form-group col-md-10">
                        <div class="form-group col-md-10">
                        <label for="inputName">Name</label>
                        <input type="text" class="form-control" id="inputName" value="${actor.name}">
                        </div>
                        <div class="form-group col-md-4">
                        <label for="inputSex">Sex</label>
                        <select id="inputSex" class="form-control">
                            <option selected>${actor.sex}</option>
                            <option>Male</option>
                            <option>Female</option>
                        </select>
                    </div>
                        <div class="form-group col-md-10">
                        <label for="inputBirthplace">Birthplace</label>
                        <input type="text" class="form-control" id="inputBirthplace" value="${actor.birthplace}">
                        </div>
                        <div class="form-group col-md-8">
                        <label for="inputBirthday">Birthday</label>
                        <input type="text" class="form-control" id="inputBirthday" value="${actor.birthday}">
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                        <label for="inputBiography">Biography</label>
                        <textarea type="text" class="form-control" id="inputBiography">${actor.biography}</textarea>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-primary" style="width:20%;" editActorBtn>Edit Actor</button>
                    <button type="submit" class="btn btn-danger" style="width:20%; float:right;" deleteActorBtn>Delete Actor</button>
                    </form>`;
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

                html+=`</tbody></table>`;
        }));

        return html;
    }

    async EditActor()
    {
        const addActorForm = document.querySelector('#addactor-form');
        const name = addActorForm['inputName'].value;
        const sex = addActorForm['inputSex'].value;
        const birthplace = addActorForm['inputBirthplace'].value;  
        const birthday = addActorForm['inputBirthday'].value;
        const biography = addActorForm['inputBiography'].value; 

        const response =  await fetch("https://localhost:44365/Actor/"+this.postId, { method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"id": parseInt(this.postId), "name": name, "sex": sex, "birthplace":birthplace , "birthday":birthday , "biography":biography})
        });

        if (response.ok) 
        {
            alert("Actor "+name+" edited!");
        }
    }

    async DeleteActor()
    {      
        const response = await fetch("https://localhost:44365/Actor/"+this.postId, { method: "DELETE"});

        if (response.ok) 
        {
            alert("Actor deleted!");
        }
    }
}
