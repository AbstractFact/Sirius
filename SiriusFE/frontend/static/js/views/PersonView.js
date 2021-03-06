
import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";
import {Person} from "../models/Person.js";
import {Role} from "../models/Role.js";
import {Directed} from "../models/Directed.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.setTitle("Viewing Person");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Person/GetPerson/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
            const actor = new Person(d["id"], d["name"], d["sex"], d["birthplace"], d["birthday"], d["biography"]);

            html=`
                <h1>${actor.name}</h1>
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

                    <br/>`;
                    
                    if(localStorage.username=="Admin" && localStorage.logged==1)
                    html+=`<form id="addactor-form" style="width:50%;">
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
                    <button type="submit" class="btn btn-primary" style="width:20%;" editPersonBtn>Edit Person</button>
                    <button type="submit" class="btn btn-danger" style="width:20%; float:right;" deletePersonBtn>Delete Person</button>
                    </form>`;
        }));

        await fetch("https://localhost:44365/Role/GetActorRoles/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
            if(d.length!=0)
            {
                i=0;
                html+=`
                    </br>
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
                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td>${data.inRole}</td>
                        <td><a href="/series/${data.seriesID}" data-link>${data.title}</a></td>
                        </tr>`;
                });

                html+=`</tbody></table>`;
            }     
        }));

        await fetch("https://localhost:44365/Directed/GetDirectorSeries/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
            if(d.length!=0)
            {
                i=0;
                html+=`
                    </br>
                    <h2>Directed series</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Series</th>
                            <th scope="col">Genre</th>
                            <th scope="col">Year</th>
                            <th scope="col">Rating</th>
                            </tr>
                        </thead>
                        <tbody>`;

                d.forEach(data => {
                    
                    const series = new Series(data["seriesID"], data["title"], data["year"], data["genre"], data["plot"], data["seasons"], data["rating"]);

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/series/${series.id}" data-link>${series.title}</a></td>
                        <td>${series.genre}</td>
                        <td>${series.year}</td>
                        <td>${(series.rating === 0)? "Not rated" : series.rating}</td>
                        </tr>`;
                });

                html+=`</tbody></table>`;
            }
        }));

        return html;
    }

    async EditPerson()
    {
        const addActorForm = document.querySelector('#addactor-form');
        const name = addActorForm['inputName'].value;
        const sex = addActorForm['inputSex'].value;
        const birthplace = addActorForm['inputBirthplace'].value;  
        const birthday = addActorForm['inputBirthday'].value;
        const biography = addActorForm['inputBiography'].value; 

        const response =  await fetch("https://localhost:44365/Person/"+this.postId, { method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"id": parseInt(this.postId), "name": name, "sex": sex, "birthplace":birthplace , "birthday":birthday , "biography":biography})
        });

        if (response.ok) 
        {
            alert("Actor "+name+" edited!");
        }
        else
        {
            alert("Error!");
        }
    }

    async DeletePerson()
    {      
        const response = await fetch("https://localhost:44365/Person/"+this.postId, { method: "DELETE"});

        if (response.ok) 
        {
            alert("Actor deleted!");
        }
        else
        {
            alert("Error!");
        }
    }
}
