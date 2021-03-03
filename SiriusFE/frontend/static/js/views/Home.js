import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Home");
    }

    async getHtml() 
    {
        var html,i=0;

        html=`
        <h1>Most Popular Series</h1>
        <br/>
        <table class="table table-striped">
            <thead>
                <tr>
                <th scope="col">#</th>
                <th scope="col">Title</th>
                <th scope="col">Year</th>
                <th scope="col">Genre</th>
                <th scope="col">Rating</th>
                <th scope="col">Popularity</th>
                </tr>
            </thead>
            <tbody>`;

        await fetch("https://localhost:44365/UserSeriesList/GetMostPopularSeries", {method: "GET"})
        .then(p => p.json().then(data => {
            data.forEach(d => {
                html+=`
                <tr>
                <th scope="row">${++i}</th>
                <td><a href="/series/${d.seriesID}" data-link>${d.title}</a></td>
                <td>${d.year}</td>
                <td>${d.genre}</td>
                <td>${(d.rating === 0)? "Not rated" : d.rating}</td>
                <td>${d.popularity}</td>
                </tr>`;
            });
        }));

        html+=`
        </tbody>
        </table>

        <br/>

        <h1>Best Rated Series</h1>
        <br/>
        <table class="table table-striped">
            <thead>
                <tr>
                <th scope="col">#</th>
                <th scope="col">Title</th>
                <th scope="col">Year</th>
                <th scope="col">Genre</th>
                <th scope="col">Rating</th>
                </tr>
            </thead>
            <tbody>`;

        i=0;
        await fetch("https://localhost:44365/Series/GetBestRatedSeries", {method: "GET"})
        .then(p => p.json().then(data => {
            data.forEach(d => {
                html+=`
                <tr>
                <th scope="row">${++i}</th>
                <td><a href="/series/${d.seriesID}" data-link>${d.title}</a></td>
                <td>${d.year}</td>
                <td>${d.genre}</td>
                <td>${d.rating}</td>
                </tr>`;
            });
        }));

        html+=`
        </tbody>
        </table>

        <br/>`;

        if(localStorage.username=="Admin" && localStorage.logged==1)
                html+=`
                    <h3>Add Person</h3>
                    <form id="addperson-form" style="width:50%">
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
                    <button type="submit" class="btn btn-primary" style="width:20%" addPersonBtn>Add Person</button>
                </form>`;

        return html;
    }

    async AddPerson()
    {
        const addPersonForm = document.querySelector('#addperson-form');
        const name = addPersonForm['inputName'].value;
        const sex = addPersonForm['inputSex'].value;
        const birthplace = addPersonForm['inputBirthplace'].value;  
        const birthday = addPersonForm['inputBirthday'].value;
        const biography = addPersonForm['inputBiography'].value;  
        
        const response = await fetch("https://localhost:44365/Person", { method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ "name": name, "sex": sex, "birthplace":birthplace , "birthday":birthday , "biography":biography})
            });

        if(response.ok)
        {
            addPersonForm.reset();
            alert("Person "+name+" added to database!");
        }
    }
}