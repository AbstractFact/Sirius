import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("Most Watched");
    }

    async getHtml() {
        return `
            <h1>Most watched series</h1>
            <p>You are viewing the most popular series right now!</p>
        `;
    }
}