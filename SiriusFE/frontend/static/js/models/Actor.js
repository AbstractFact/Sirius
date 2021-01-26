
export class Actor {
    constructor(id, name, sex, birthplace, birthday, biography) {
        this.id = id;
        this.name = name;
        this.sex=sex;
        this.birthplace = birthplace;
        this.birthday = birthday;
        this.biography = biography;
    }

    print() {
        console.log(this.id + "\n"+this.name+ "\n"+this.birthplace + "\n"+this.birthday + "\n"+this.biography + "\n");
    }
}