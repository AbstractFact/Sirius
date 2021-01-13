import AbstractView from "./AbstractView.js";

export default class extends AbstractView {
    constructor(params) {
        super(params);
        this.postId = params.id;
        this.setTitle("Viewing Series");
    }

    async getHtml() {
        return `
            <h1>Series #${this.postId}</h1>
            <p>You are viewing series #${this.postId}.</p>
        `;
    }
}
