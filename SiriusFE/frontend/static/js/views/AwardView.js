import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";
import {Award} from "../models/Award.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.setTitle("Viewing Award");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Award/GetAward/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
            const award = new Award(d["id"], d["name"], d["description"]);

            html=`
                <h1>Award: ${award.name}</h1>
                <br/>
                <p>
                    ${award.description}
                </p>
                <br/>`;
                    
                if(localStorage.username=="Admin" && localStorage.logged==1)
                html+=`<form id="editAward-form" style="width:50%;">
                <div class="form-group col-md-14">
                    <div class="form-group col-md-12">
                    <label for="inputName">Name</label>
                    <input type="text" class="form-control" id="inputName" value="${award.name}">
                    </div>
                    <div class="form-group col-md-12">
                    <label for="inputDescription">Description</label>
                    <textarea type="text" class="form-control" id="inputDescription">${award.description}</textarea>
                    </div>
                </div>
                </br>
                <button type="submit" class="btn btn-primary" style="width:30%;" editAwardBtn>Edit Award</button>
                <button type="submit" class="btn btn-danger" style="width:30%; float:right;" deleteAwardBtn>Delete Award</button>
                </form>
                </br>`;
        }));

        await fetch("https://localhost:44365/Awarded/GetAwardedSeries/"+this.postId, {method: "GET"})
        .then(p => p.json().then(d => {
                i=0;
                html+=`
                    <h2>Awarded series</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Series</th>
                            <th scope="col">Year</th>
                            </tr>
                        </thead>
                        <tbody>`;

                d.forEach(data => {

                    const series = new Series(data["series"]["id"], data["series"]["title"], data["series"]["year"], data["series"]["genre"], data["series"]["plot"], data["series"]["seasons"], data["series"]["rating"]);

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/series/${series.id}" data-link>${series.title}</a></td>
                        <td>${data["year"]}</td>
                        </tr>`;
                });

                html+=`</tbody></table>`;
        }));

        return html;
    }

    async EditAward()
    {
        const editAwardForm = document.querySelector('#editAward-form');
        const name = editAwardForm['inputName'].value;
        const description = editAwardForm['inputDescription'].value;

        const response =  await fetch("https://localhost:44365/Award/"+this.postId, { method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"id": parseInt(this.postId), "name": name, "description": description})
        });

        if (response.ok) 
        {
            alert("Award "+name+" edited!");
        }
        else
        {
            alert("Error!");
        }
    }

    async DeleteAward()
    {      
        const response = await fetch("https://localhost:44365/Award/"+this.postId, { method: "DELETE"});

        if (response.ok) 
        {
            alert("Award deleted!");
        }
        else
        {
            alert("Error!");
        }
    }
}
