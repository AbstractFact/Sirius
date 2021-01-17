
export class User {
    constructor(id, username, password) {
        this.id = id;
        this.username = username;
        this.password = password;
    }

    print() {
        console.log(this.id + "\n"+this.username+ "\n"+this.password+"\n");
    }
}