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
                        <th scope="col">Sex</th>
                        <th scope="col">Birthplace</th>
                        <th scope="col">Birthday</th>
                        </tr>
                    </thead>
                    <tbody>`;

            data.forEach(d => {
                    const actor = new Actor(d["id"], d["name"], d["sex"], d["birthplace"], d["birthday"], d["biography"]);

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
                <p>
                    <a href="/series" data-link>View most watched series</a>.
                </p>

                <br/>
                
                <form id="addactor-form" style="width:50%">
                <div class="form-group col-md-10">
                    <div class="form-group col-md-10">
                    <label for="inputName">Name</label>
                    <input type="text" class="form-control" id="inputName" placeholder="Name">
                    </div>
                    <div class="form-group col-md-4">
                    <label for="inputSex">Sex</label>
                    <select id="inputSex" class="form-control">
                        <option selected>Select Sex</option>
                        <option>Male</option>
                        <option>Female</option>
                    </select>
                </div>
                    <div class="form-group col-md-10">
                    <label for="inputBirthplace">Birthplace</label>
                    <input type="text" class="form-control" id="inputBirthplace" placeholder="Birthplace">
                    </div>
                    <div class="form-group col-md-8">
                    <label for="inputBirthday">Birthday</label>
                    <input type="text" class="form-control" id="inputBirthday" placeholder="Birthday">
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group">
                    <label for="inputBiography">Biography</label>
                    <textarea type="text" class="form-control" id="inputBiography" placeholder=""></textarea>
                    </div>
                </div>
                <button type="submit" class="btn btn-primary" style="width:20%" addActorBtn>Add Actor</button>
                </form>`;
        }));

        return html;
    }

    async AddActor()
    {
        const addActorForm = document.querySelector('#addactor-form');
        const name = addActorForm['inputName'].value;
        const sex = addActorForm['inputSex'].value;
        const birthplace = addActorForm['inputBirthplace'].value;  
        const birthday = addActorForm['inputBirthday'].value;
        const biography = addActorForm['inputBiography'].value;  
        
        const response = await fetch("https://localhost:44365/Actor", { method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ "name": name, "sex": sex, "birthplace":birthplace , "birthday":birthday , "biography":biography})
            });

        if(response.ok)
        {
            addActorForm.reset();
            alert("Actor "+name+" added to database!");
        }
    }
}