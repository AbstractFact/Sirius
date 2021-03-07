import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) 
    {
        super(params);
        this.html=``;
        this.setTitle("All Friends Favourites");
    }

    async getHtml() 
    {
        this.html=`
            <h1>All Friends Favourites</h1>
            <br/>
            <table class="table table-striped">
                <thead>
                    <tr>
                    <th scope="col">#</th>
                    <th scope="col">From</th>
                    <th scope="col">Title</th>
                    <th scope="col">Genre</th>
                    <th scope="col">Seasons</th>
                    <th scope="col">Rating</th>
                    </tr>
                </thead>
                <tbody>`;

        var i=0;

        const res = await fetch("https://localhost:44365/UserSeriesList/GetRecommendations/"+localStorage.userid, {method: "GET"});

        if(res.ok)
        {
            const data = await res.json();

            if(data.length!=0)
            {
                data.forEach(d => 
                {
                    const username = d["username"];

                    this.html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td>${username}</td>
                        <td><a href="/series/${d.seriesID}" data-link>${d.title}</a></td>
                        <td>${d.genre}</td>
                        <td>${d.seasons}</td>
                        <td>`+ +(Math.round(d.rating + "e+1") + "e-1")+`</td>
                        </tr>`;
                })
            }
        }     
        
        this.html+=`
            </tbody>
            </table>`;

        return this.html;
    }
}
