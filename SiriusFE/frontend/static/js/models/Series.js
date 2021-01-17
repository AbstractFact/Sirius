//import { Role } from "./Role.js"

export class Series {
    constructor(id, title, year, genre, plot, seasons, rating) {
        this.id = id;
        this.title = title;
        this.year = year;
        this.genre = genre;
        this.plot = plot;
        this.seasons = seasons;
        this.rating = rating;
    }

    // print() {
    //     console.log(this.id + "\n"+this.title+ "\n"+this.year + "\n");
    // }

//     dodajProizvod(newProizvod) {
//          this.proizvodi.push(newProizvod);
//     }
//     crtaj(host) {
//         if (!host)
//             throw new Error("Host nije pronadjen");
//         this.kontejner = document.createElement("div");
//         host.appendChild(this.kontejner)
//         this.kontejner.className = "GlavniDiv";

//         const divUnos = document.createElement("div");
//         divUnos.className = "divUnos";
//         this.kontejner.appendChild(divUnos);
//         this.crtajFormuZaUnos(divUnos);

//         const divTabela = document.createElement("div");
//         divTabela.className = "divTabela";
//         this.kontejner.appendChild(divTabela);
//         const labnaslov = document.createElement("h2");
//         labnaslov.innerHTML = "Lista dostupnih proizvoda (" + this.ime + ")";
//         divTabela.appendChild(labnaslov);

//         const divFilter = document.createElement("div");
//         divFilter.className = "divFilter";

//         const lab1 = document.createElement("label");
//         lab1.innerHTML = "Cena od";
//         divFilter.appendChild(lab1);

//         const input1 = document.createElement("input");
//         input1.type = "number";
//         input1.className = "filterOd";
//         divFilter.appendChild(input1);

//         const lab2 = document.createElement("label");
//         lab2.innerHTML = "Cena do";
//         divFilter.appendChild(lab2);

//         const input2 = document.createElement("input");
//         input2.type = "number";
//         input2.className = "filterDo";
//         divFilter.appendChild(input2);

//         const button = document.createElement("button");
//         button.type = "button";
//         button.innerHTML = "Filter";
//         button.onclick = (ev) => {
//             var od = this.kontejner.querySelector(".filterOd").value;
//             var doo = this.kontejner.querySelector(".filterDo").value;

//             if (od == undefined || od == null || od == "") {
//                 od = -1;
//             }

//             if (doo == undefined || doo == null || doo == "") {
//                 doo = -1;
//             }
//             this.obrisiPrikaz();

//             // FiltrirajProizvode je običan GET zahtev kao i u main
//             fetch("https://localhost:5001/Series/FiltrirajProizvode/" + this.id + "/" + od + "/" + doo)
//                 .then(p => p.json().then(data => {
//                     // Samo što ovde moramo da obrišemo sve što je bilo u tabeli da bi se ponovo upisali rezultati
//                     this.proizvodi = [];

//                     data.forEach(d => {
//                         this.proizvodi.push(new Proizvod(d["id"], d["sifra"], d["ime"], d["cena"], d["kolicina"]));
//                     });

//                     this.proizvodi.forEach((proiz, index) => {
//                         const r = proiz.crtajRed(tbody);
//                         this.dodajDeleteDugme(r, proiz, index);
//                     });
//                 }));
//         }

//         divFilter.appendChild(button);

//         divTabela.appendChild(divFilter);

//         const tabela = document.createElement("table");
//         divTabela.appendChild(tabela);
//         this.crtajHeder(tabela);
//         let tbody = document.createElement("tbody");
//         tabela.appendChild(tbody);
//         this.proizvodi.forEach((proiz, index) => {
//             const r = proiz.crtajRed(tbody);
//             this.dodajDeleteDugme(r, proiz, index);
//         });
//     }
//     obrisiPrikaz() {
//         this.kontejner.querySelector("tbody").innerHTML = "";
//     }

//     dodajDeleteDugme(r, proiz, index) {
//         const delDugme = document.createElement("button");
//         delDugme.innerHTML = "Obrisi";
//         r.appendChild(delDugme);
//         delDugme.onclick = (ev) => {
//             this.obrisiProizvod(proiz, index, r);
//         }
//     }
//     obrisiProizvod(proizvodZaBrisanje, index, poljeUTabeli) {
//         // DELETE metoda putem fetch-a, potpuno isto kao recimo PUT, samo je method drugačiji
//         fetch("https://localhost:5001/Series/BrisiProizvod/" + proizvodZaBrisanje.id, {
//             method: "DELETE"
//         }).then(p => {
//             if (p.ok) {
//                 this.proizvodi.splice(index, 1);
//                 const red = poljeUTabeli.parentNode;
//                 poljeUTabeli.parentNode.parentNode.removeChild(red);
//             }
//         });
//     }
//     crtajFormuZaUnos(divUnos) {
//         const listaPolja = ["Sifra", "Naziv", "Cena", "Kolicina"];

//         let red = null;
//         let lab = null;
//         let d = null;
//         red = document.createElement("div");
//         divUnos.appendChild(red);

//         const labnaslov = document.createElement("h4");
//         labnaslov.innerHTML = "Unos novog proizvoda";
//         red.appendChild(labnaslov);

//         listaPolja.forEach(el => {
//             red = document.createElement("div");
//             divUnos.appendChild(red);

//             lab = document.createElement("label");
//             lab.innerHTML = el;
//             red.appendChild(lab);

//             d = document.createElement("input");
//             d.className = el;
//             red.appendChild(d);
//         })
//         const btn = document.createElement("button");
//         btn.innerHTML = "Dodaj novi proizvod";
//         divUnos.appendChild(btn);
//         btn.onclick = (ev) => this.dodajNoviProzivod();

//     }
//     dodajNoviProzivod() {
//         const proizvod = new Proizvod();
//         proizvod.ime = this.kontejner.querySelector(".Naziv").value;
//         proizvod.sifra = this.kontejner.querySelector(".Sifra").value;
//         proizvod.cena = parseInt(this.kontejner.querySelector(".Cena").value);
//         proizvod.kolicina = parseInt(this.kontejner.querySelector(".Kolicina").value);

//         // I POST metoda
//         fetch("https://localhost:5001/Series/DodajProizvod/" + this.id, {
//             method: "POST",
//             // Neophodno je reći serveru koji je tip podataka koji se šalje, u ovom slučaju json
//             // headers ima Content-Type = application/json
//             headers: {
//                 "Content-Type": "application/json"
//             },
//             // I u body se smešta string reprezentacija objekta koji se šalje
//             body: JSON.stringify({
//                 "ime": proizvod.ime, "sifra": proizvod.sifra,
//                 "cena": proizvod.cena, "kolicina": proizvod.kolicina
//             })
//         }).then(p => {
//             if (p.ok) {
//                 this.dodajProizvod(proizvod);
//                 const r = proizvod.crtajRed(this.kontejner.querySelector("tbody"));
//                 this.dodajDeleteDugme(r, proizvod, this.proizvodi.length - 1);
//             }
//         });

//     }
//     crtajHeder(tabela) {
//         const header = document.createElement("thead");
//         tabela.appendChild(header)
//         const red = document.createElement("tr");
//         header.appendChild(red);

//         let el = document.createElement("th");
//         el.innerHTML = "Šifra";
//         red.appendChild(el);

//         el = document.createElement("th");
//         el.innerHTML = "Naziv";
//         red.appendChild(el);

//         el = document.createElement("th");
//         el.innerHTML = "Cena";
//         red.appendChild(el);

//         el = document.createElement("th");
//         el.innerHTML = "Kolicina";
//         red.appendChild(el);

//         el = document.createElement("th");
//         el.innerHTML = "Ukupna cena";
//         red.appendChild(el);

//         el = document.createElement("th");
//         red.appendChild(el);
//     }
}