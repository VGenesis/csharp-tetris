public class Collision{
    public bool left, right, down;

    public Collision(){
        left = false;
        right = false;
        down = false;
    }
    
    public string toString(){
        return String.Format(
                format: "[left: {0}, right: {1}, down: {2}.]",
                this.left,
                this.right,
                this.down
                );
    }
}

