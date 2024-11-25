public class Vector{
    public int x, y;
    public Vector(int x, int y) => (this.x, this.y) = (x, y);
    public static Vector zero => new Vector(0, 0);
}
