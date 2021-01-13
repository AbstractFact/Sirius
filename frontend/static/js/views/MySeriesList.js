import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.setTitle("My Series List");
    }

    async getHtml() {
        return `
            <h1>My series list</h1>
            <p>Manage your series list</p>
        `;
    }
}