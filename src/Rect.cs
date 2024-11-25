public class Rect{
    public int x, y, w, h;
    public Rect(int x, int y, int w, int h){
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }

    public static Rect zero = new Rect(0, 0, 0, 0);

    public bool contains(int x, int y){
        x -= this.x;
        y -= this.y;
        return(x >= 0 && x < this.w && y >= 0 && y < this.h); 
    }
}
