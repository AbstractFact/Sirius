import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js";

export default class extends AbstractView {
    constructor(params) 
    {
        super(params);
        this.html=``;
        this.setTitle("All Recommendations");
    }

    async getHtml() 
    {
        var friends = new Array();
                this.html=`
                    <h1>All Recommendations</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">From</th>
                            <th scope="col">Title</th>
                            <th scope="col">Year</th>
                            <th scope="col">Genre</th>
                            <th scope="col">Plot</th>
                            <th scope="col">Seasons</th>
                            <th scope="col">Rating</th>
                            </tr>
                        </thead>
                        <tbody>`;

        const res1 = await fetch("https://localhost:44365/User/GetAllFriends/"+localStorage.userid, {method: "GET"});

        await res1.json().then(data=>{
            data.forEach(d => 
            {
                friends.push(d["id"]);
            });
        });
        
        await this.GetRecommendations(friends);
        
        this.html+=`
            </tbody>
            </table>`;

        return this.html;
    }

    async GetRecommendations(friends)
    {
        var i=0;
        for(var j=0; j<friends.length; j++)
        {
            const res = await fetch("https://localhost:44365/UserSeriesList/GetUserFavourites/"+friends[j], {method: "GET"});

            await res.json().then(data=>{
                if(data.length!=0)
                {
                    const username = data[0]["username"];
                    data.forEach(d => 
                    {
                        const series = new Series(d["series"]["id"], d["series"]["title"], d["series"]["year"], d["series"]["genre"], d["series"]["plot"], d["series"]["seasons"], d["series"]["rating"]);

                        this.html+=`
                            <tr>
                            <th scope="row">${++i}</th>
                            <td>${username}</td>
                            <td><a href="/series/${series.id}" data-link>${series.title}</a></td>
                            <td>${series.year}</td>
                            <td>${series.genre}</td>
                            <td>${series.plot}</td>
                            <td>${series.seasons}</td>
                            <td>`+ +(Math.round(series.rating + "e+1") + "e-1")+`</td>
                            </tr>`;
                    });
                }
            });
        };
    }
}
