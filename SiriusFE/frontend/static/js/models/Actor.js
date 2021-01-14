

export class Actor {
    constructor(id, name, birthplace, birthday, biography) {
        this.id = id;
        this.name = name;
        this.birthplace = birthplace;
        this.birthday = birthday;
        this.biography = biography;
    }

    print() {
        console.log(this.id + "\n"+this.name+ "\n"+this.birthplace + "\n"+this.birthday + "\n"+this.biography + "\n");
    }
}