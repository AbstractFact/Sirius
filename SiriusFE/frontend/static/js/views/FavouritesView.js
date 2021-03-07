import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) 
    {
        super(params);
        this.postId = params.id;
        this.setTitle("Friend's favourites");
    }

    async getHtml() 
    {
        var html,i;
        await fetch("https://localhost:44365/UserSeriesList/GetUserFavourites/"+this.postId, {method: "GET"})
        .then(p => p.json().then(data => {
            if(data.length!=0)
            {
                i=0;
                const username = data[0]["username"];
                html=`
                    <h1>${username}'s favourites</h1>
                    <br/>
                    <table class="table table-striped">
                        <thead>
                            <tr>
                            <th scope="col">#</th>
                            <th scope="col">Title</th>
                            <th scope="col">Genre</th>
                            <th scope="col">Seasons</th>
                            <th scope="col">Rating</th>
                            </tr>
                        </thead>
                        <tbody>`;

            data.forEach(d => {

                    html+=`
                        <tr>
                        <th scope="row">${++i}</th>
                        <td><a href="/series/${d.seriesID}" data-link>${d.title}</a></td>
                        <td>${d.genre}</td>
                        <td>${d.seasons}</td>
                        <td>`+ +(Math.round(d.rating + "e+1") + "e-1")+`</td>
                        </tr>`;
                 });

            html+=`
                </tbody>
                </table>`;
            }
            else
                html=`<h1>User has no favourites!</h1>`;
        }));

        return html;
    }
}
