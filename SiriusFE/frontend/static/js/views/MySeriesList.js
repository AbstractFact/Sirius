import AbstractView from "./AbstractView.js";
import {Series} from "../models/Series.js"

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("My Series List");
    }

    async getHtml() 
    {
        var html,i;
    
        if(localStorage.userid!=null)

        await fetch("https://localhost:44365/UserSeriesList/GetUserSeriesList/"+localStorage.userid, {method: "GET"})
        .then(p => p.json().then(data => {
                i=0;
                html=`
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
                            </tr>
                        </thead>
                        <tbody>`;

            data.forEach(d => {
                    const series = new Series(d["series"]["id"], d["series"]["title"], d["series"]["year"], d["series"]["genre"], d["series"]["plot"], d["series"]["seasons"], d["series"]["rating"]);
                    const status = d["status"];

                    // UPARITI SA OCENAMA I KOMENTARIMA SERIJE TOG KORISNIKA

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/series/${series.id}" data-link>${series.title}</a></td>
                        <td>${series.genre}</td>
                        <td>${series.seasons}</td>
                        <td>`+ +(Math.round(series.rating + "e+1") + "e-1")+`</td>
                        <td>${status}</td>
                        </tr>`;
                 });


            // html+=`
            //     </tbody>
            //     </table>
            //     <p>
            //         <a href="/series" data-link>View most watched series</a>.
            //     </p>`;
        }));

        return html;
    }
}