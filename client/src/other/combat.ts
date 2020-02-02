class CombatRender {
    public static rect(ct: CanvasRenderingContext2D, mx: number, my: number, x: number, y: number, size:number): void {
        let hover = (mx > x && mx < x+size && my > y && my < y+size);
    }
}