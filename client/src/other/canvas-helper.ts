export default class CanvasHelper {
    public canvas!: HTMLCanvasElement;
    public context!: CanvasRenderingContext2D;
    public globalMouseX: number = 0;
    public globalMouseY: number = 0;
    public mouseX: number = 0;
    public mouseY: number = 0;

    public currentMouseDown: boolean = false;
    public previousMouseDown: boolean = false;

    private eventMouseState: boolean = false;

    public setup(canvas: HTMLCanvasElement): void {
        this.canvas = canvas;
        let context = canvas.getContext("2d");

        if (context !== null)
            this.context = context;

        document.addEventListener("mousemove", (evt) => this.updateMouseCoordinates(evt));
        document.addEventListener("mousemove", (evt) => this.updateMouseButtonState(evt));
        document.addEventListener("mousedown", (evt) => this.updateMouseButtonState(evt));
        document.addEventListener("mouseup", (evt) => this.updateMouseButtonState(evt));
    }

    public destroy(): void {
        document.removeEventListener("mousemove", (evt) => this.updateMouseCoordinates(evt));
        document.removeEventListener("mousemove", (evt) => this.updateMouseButtonState(evt));
        document.removeEventListener("mousedown", (evt) => this.updateMouseButtonState(evt));
        document.removeEventListener("mouseup", (evt) => this.updateMouseButtonState(evt));
    }

    public update(): void {
        this.previousMouseDown = this.currentMouseDown;
        this.currentMouseDown = this.eventMouseState;

        this.mouseX = this.globalMouseX - this.canvas.getBoundingClientRect().left;
        this.mouseY = this.globalMouseY - this.canvas.getBoundingClientRect().top;

        this.canvas.style.cursor = "default";
    }

    public isHover(x: number, y: number, w: number, h: number): boolean {
        return (this.mouseX > x && this.mouseX < x+w && this.mouseY > y && this.mouseY < y+h);
    }

    public isMouseDown(): boolean {
        return this.currentMouseDown;
    }

    public isMouseUp(): boolean {
        return !this.currentMouseDown;
    }

    public isMousePressed(): boolean {
        console.log(this.currentMouseDown, this.previousMouseDown);
        return this.currentMouseDown && !this.previousMouseDown;
    }

    public isMouseReleased(): boolean {
        return !this.currentMouseDown && this.previousMouseDown;
    }

    private updateMouseCoordinates(evt: MouseEvent): void {
        this.globalMouseX = evt.clientX;
        this.globalMouseY = evt.clientY;
    }

    private updateMouseButtonState(evt: MouseEvent): void {
        this.eventMouseState = (evt.buttons & 1) === 1;
    }
}