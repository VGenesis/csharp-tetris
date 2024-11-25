public class Shape{
    private const string shapeNames = "IJLBSZT";
    private static readonly char[,,] shapes = {
        {
            {'-', 'I', '-', '-'},
            {'-', 'I', '-', '-'},
            {'-', 'I', '-', '-'},
            {'-', 'I', '-', '-'}
        },
        {
            {'-', '-', 'J', '-'},
            {'-', '-', 'J', '-'},
            {'-', 'J', 'J', '-'},
            {'-', '-', '-', '-'}
        },
        {
            {'-', 'L', '-', '-'},
            {'-', 'L', '-', '-'},
            {'-', 'L', 'L', '-'},
            {'-', '-', '-', '-'}
        },
        {
            {'-', '-', '-', '-'},
            {'-', 'B', 'B', '-'},
            {'-', 'B', 'B', '-'},
            {'-', '-', '-', '-'}
        },
        {
            {'-', 'S', '-', '-'},
            {'-', 'S', 'S', '-'},
            {'-', '-', 'S', '-'},
            {'-', '-', '-', '-'}
        },
        {
            {'-', '-', 'Z', '-'},
            {'-', 'Z', 'Z', '-'},
            {'-', 'Z', '-', '-'},
            {'-', '-', '-', '-'}
        },
        {
            {'-', '-', 'T', '-'},
            {'-', 'T', 'T', '-'},
            {'-', '-', 'T', '-'},
            {'-', '-', '-', '-'}
        },
    };
    private static readonly Rect[] shapeRects = {
        new Rect(1, 0, 1, 4),
        new Rect(1, 0, 2, 3),
        new Rect(1, 0, 2, 3),
        new Rect(1, 1, 2, 2),
        new Rect(1, 0, 2, 3),
        new Rect(1, 0, 2, 3),
        new Rect(1, 0, 2, 3),
    };

    public char value;
    private char[,] shape;
    public Rect rect;
    public int x, y;
    public readonly int w = 4, h = 4;

    public Collision collision;

    public Shape(char shapeName){
        this.value = shapeName;

        int shapeIndex = shapeNames.IndexOf(shapeName);
        this.shape = new char[w, h];
        for(int i = 0; i < w; i++)
            for(int j = 0; j < h; j++)
                shape[i, j] = shapes[shapeIndex, i, j];

        this.rect = shapeRects[shapeIndex];
        this.collision = new Collision();

        this.x = 0;
        this.y = 0;
    }

    public static Shape random(){
        Random rng = new Random();
        int index = (int)(rng.NextInt64() % shapeNames.Length);
        return new Shape(shapeNames[index]);
    }

    public void setPos(int x, int y){
        this.x = x;
        this.y = y;
    }

    public char[,] getShape(){ return shape; }

    public Vector[] getBlocks(){
        Vector[] blocks = new Vector[4];
        int index = 0;
        for(int j = 0; j < h; j++)
            for(int i = 0; i < w; i++)
                if(this.shape[j, i] != '-')
                    blocks[index++] = new Vector(i, j);

        return blocks;
    }

    public bool fallDown(){
        if(!this.collision.down) this.y += 1;
        return !this.collision.down;
    }
    public bool moveLeft(){
        if(!this.collision.left) this.x -= 1;
        return !this.collision.left;

    }
    public bool moveRight(){
        if(!this.collision.right) this.x += 1;
        return !this.collision.right;
    }

    public void setCollision(bool left, bool right, bool down){
        this.collision.left = left;
        this.collision.right = right;
        this.collision.down = down;
    }

    public void rotate(){
        char[,] rotatedShape = new char[w, h];
        for(int j = 0; j < h; j++)
            for(int i = 0; i < w; i++)
                rotatedShape[i, j] = shape[w-j-1, i];

        Rect rect = new Rect(w, h, 0, 0);
        for(int j = 0; j < h; j++)
            for(int i = 0; i < w; i++)
                if(rotatedShape[j, i] != '-'){
                    rect.x = Math.Min(rect.x, i);
                    rect.y = Math.Min(rect.y, j);
                    rect.w = Math.Max(rect.w, i);
                    rect.h = Math.Max(rect.h, j);
                }

        this.rect = rect;
        this.shape = rotatedShape;
    }
}
