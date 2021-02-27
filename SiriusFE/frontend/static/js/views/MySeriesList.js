import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js"
import {MySeriesList} from "../models/MySeriesList.js"

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("My Series List");
        this.entries=new Array();
    }

    async getHtml() 
    {
        var html,i;

        if(localStorage.userid!=0)
        {
            html=`
                <h3>Subscription settings</h3>
                <div>
                    <input type="checkbox" id="Drama">
                    <label>Drama</label>
                </div>
                <div>
                    <input type="checkbox" id="Comedy">
                    <label>Comedy</label>
                </div>
                <div>
                    <input type="checkbox" id="Crime">
                    <label>Crime</label>
                </div>
                <div>
                    <input type="checkbox" id="Fantasy">
                    <label>Fantasy</label>
                </div>
                <div>
                    <input type="checkbox" id="Sci-fi">
                    <label>Sci-fi</label>
                </div>
                <button class="btn btn-success" style="width:10%" btnSaveSubscriptionChanges>Save changes</button>
                `;

            await fetch("https://localhost:44365/UserSeriesList/GetUserSeriesList/"+localStorage.userid, {method: "GET"})
            .then(p => p.json().then(data => {
                    i=0;
                    html+=`
                        <h1>My Series List</h1>
                        <br/>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                <th scope="col">#</th>
                                <th scope="col">Title</th>
                                <th scope="col">Genre</th>
                                <th scope="col">Seasons</th>
                                <th scope="col">Rating</th>
                                <th scope="col">Status</th>
                                <th scope="col">My Rating</th>
                                <th scope="col">Comment</th>
                                <th scope="col">Favourite</th>
                                <th scope="col"></th>
                                <th scope="col"></th>
                                </tr>
                            </thead>
                            <tbody>`;

                data.forEach(d => {
                        const series = new Series(d["series"]["id"], d["series"]["title"], d["series"]["year"], d["series"]["genre"], d["series"]["plot"], d["series"]["seasons"], d["series"]["rating"]);
                        const status = d["status"];
                        const stars = d["stars"];
                        const comment = d["comment"];
                        const favourite = d["favourite"];

                        const checked= favourite?`checked`:``;

                        const entry = new MySeriesList(d["id"], series, status, stars, comment, favourite);
                        this.entries.push(entry);

                        html+=`
                            <tr id="${entry.id}">
                            <th scope="row">${++i}</th>
                            <td><a href="/series/${series.id}" class="serid" id="${series.id}" data-link>${series.title}</a></td>
                            <td>${series.genre}</td>
                            <td>${series.seasons}</td>
                            <td>`+ +(Math.round(series.rating + "e+1") + "e-1")+`</td>
                            <td>
                                <select id="inputStatus" class="form-control">
                                    <option selected>${status}</option>
                                    <option>Watching</option>
                                    <option>Plan to Watch</option>
                                    <option>On Hold</option>
                                    <option>Dropped</option>
                                    <option>Completed</option>
                                </select>
                            </td>
                            <td>
                                <select id="inputStars" class="form-control">
                                    <option selected>${stars}</option>
                                    <option>1</option>
                                    <option>2</option>
                                    <option>3</option>
                                    <option>4</option>
                                    <option>5</option>
                                </select>
                            </td>
                            <td>
                                <textarea type="text" class="form-control" id="inputComment">${comment}</textarea>
                            </td>
                            <td>
                                <input type="checkbox" id="inputFav" name="fav" ${checked}>
                            </td>
                            <td>
                                <button type="submit" class="btn btn-primary" style="width:60%" id="${entry.id}">Save Changes</button>
                            </td>
                            <td>
                                <button type="submit" class="btn btn-danger" style="width:100%" id="R${entry.id}">X</button>
                            </td>
                            </tr>`;
                    });
                
                    html+=`</tbody>
                    </table>`;
            }));
        }

        return html;
    }

    GetEntries()
    {
        return this.entries;
    }

    async EditEntry(id)
    {
        const row = document.getElementById(id);
        const serid = row.querySelector('.serid').id;
        const status = row.querySelector('#inputStatus').value;
        const stars = parseInt(row.querySelector('#inputStars').value);  
        const comment = row.querySelector('#inputComment').value;
        const fav = row.querySelector('#inputFav').checked;

        const response = await fetch("https://localhost:44365/UserSeriesList/"+id+"/"+serid, { method: "PUT",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({"status":status, "stars":stars, "comment":comment, "favourite":fav})
        });

        if (response.ok){
            alert("Entry "+serid+" edited!");
        };
    }

    DeleteEntry(id)
    {
        fetch("https://localhost:44365/UserSeriesList/"+id, { method: "DELETE"}).then(p => {
            if (p.ok) {
                alert("Entry deleted!");
            }
        });
    }

    async Subscribe(connection)
    {
        var subscribe = new Array();
        var unsubscribe = new Array();

        if(document.getElementById("Drama").checked) 
        {
            subscribe.push("Drama");
            await connection.subscribe("Drama");
        } 
        else
        {
            unsubscribe.push("Drama");
            await connection.unsubscribe("Drama");
        }
        if(document.getElementById("Comedy").checked) 
        {
            subscribe.push("Comedy");
            await connection.subscribe("Comedy");
        } 
        else
        {
            unsubscribe.push("Comedy");
            await connection.unsubscribe("Comedy");
        }
        if(document.getElementById("Crime").checked)
        {
            subscribe.push("Crime");
            await connection.subscribe("Crime");
        }
        else
        {
            unsubscribe.push("Crime");
            await connection.unsubscribe("Crime");
        }
        if(document.getElementById("Fantasy").checked)
        {
            subscribe.push("Fantasy");
            await connection.subscribe("Fantasy");
        }
        else
        {
            unsubscribe.push("Fantasy");
            await connection.unsubscribe("Fantasy");
        }
        if(document.getElementById("Sci-fi").checked) 
        {
            subscribe.push("Sci-fi");
            await connection.subscribe("Sci-fi");
        }
        else
        {
            unsubscribe.push("Sci-fi");
            await connection.unsubscribe("Sci-fi");
        } 
      
        fetch("https://localhost:44365/User/Subsribe/"+localStorage.userid, { method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({"subList":subscribe, "unsubList":unsubscribe})
        }).then(p => {
            if (p.ok) {
                alert("Changes saved!");
            }
        });
    }

    async DeleteRecommendation()
    {
        fetch("https://localhost:44365/User/DeleteRecommendation/"+localStorage.userid, { method: "DELETE",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({"seriesID":status, "title":stars, "comment":comment, "genre":fav})
        }).then(p => {
            if (p.ok) {
                alert("You are unsubscribed from Drama!");
            }
        });
    }
}