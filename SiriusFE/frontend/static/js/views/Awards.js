import AbstractView from "./AbstractView.js";
import {Award} from "../models/Award.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("All Awards");
    }

    async getHtml() 
    {
        var html,i;

        await fetch("https://localhost:44365/Award", {method: "GET"})
        .then(p => p.json().then(data => {
            i=0;
            html=`
                <h1>All Awards</h1>
                <br/>
                <div style="display:inline-block; width:100%;">
                    <form id="filter-form" style="width:100%">
                        <div style="display:inline-block; width:38%">
                            <div class="form-group col-md-10">
                            <label for="filterName">Name: </label>
                            <input type="text" style="width:70%" class="form-control" id="filterName" placeholder="Title">
                            </div>
                        </div>
                        <button type="submit" class="btn btn-primary" style="width:15%; float:right;" filterBtn>Filter</button>
                    </form>
                </div>
                <table class="table table-striped">
                    <thead>
                        <tr>
                        <th scope="col">#</th>
                        <th scope="col">Name</th>
                        <th scope="col">Desciption</th>
                        </tr>
                    </thead>
                    <tbody id="tcontent">`;

            data.forEach(d => {
                    const award = new Award(d["id"], d["name"], d["description"]);

                    html+=`
                    <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/awards/${award.id}" data-link>${award.name}</a></td>
                        <td>${award.description}</td>
                    </tr>`;
            });

            html+=`
                </tbody>
                </table>
                <br/>`;
                
                if(localStorage.username=="Admin" && localStorage.logged==1)
                html+=`<form id="addaward-form" style="width:50%">
                    <div class="form-group col-md-10">
                        <div class="form-group col-md-10">
                        <label for="inputName">Name</label>
                        <input type="text" class="form-control" id="inputName" placeholder="Name">
                        </div>
                        <div class="form-group col-md-8">
                        <label for="inputDescription">Description</label>
                        <textarea class="form-control" id="inputDescription" placeholder="Description"></textarea>
                        </div>
                    </div>
                    <button type="submit" class="btn btn-primary" style="width:20%" addAwardBtn>Add Award</button>
                </form>`;
        }));

        return html;
    }

    async AddAward()
    {
        const addAwardForm = document.querySelector('#addaward-form');
        const name = addAwardForm['inputName'].value;
        const description = addAwardForm['inputDescription'].value;  
        
        const response = await fetch("https://localhost:44365/Award", { method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ "name": name, "description": description})
            });

        if(response.ok)
        {
            addAwardForm.reset();
            alert("Award "+name+" added to database!");
        }
        else
        {
            alert("Error!");
        }
    }

    async Filter()
    {
        const filterForm = document.querySelector('#filter-form');
        const name = (filterForm['filterName'].value != "")? filterForm['filterName'].value : "All";
        const table = document.body.querySelector("#tcontent");
        
        await fetch("https://localhost:44365/Award/GetAwardsFiltered/"+name, {method: "POST"})
        .then(p => p.json().then(data => {
            table.innerHTML=``;
            var i=0;
            data.forEach(d => {
                table.innerHTML+=`
                <tr>
                <th scope="row">${++i}</th>
                <td><a href="/awards/${d["id"]}" data-link>${d["name"]}</a></td>
                <td>${d["description"]}</td>
                </tr>`;
            });
        }));    
    }
}