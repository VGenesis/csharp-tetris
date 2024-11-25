class App {
    private TermGraphics graphics;

    private DateTime startTime;
    private DateTime lastFrameTime;

    private readonly int width, height;
    private char[,] board;
    private char[,] view;

    private Shape currentShape;
    private Shape[] nextShapes;
    private int shapeFallTimer;
    private int shapeFallTime;
    private int shapeMinTime;

    private const string scoreboardFilename = "src/scoreboard.txt";
    private Dictionary<string, int> scoreboard;

    private int points = 0;
    private int level = 1;
    private int levelUpPoints = 500;
    private int speedUpLevel= 5;

    public const int FPS = 60;
    private float frameTime;
    private int frames;
    public bool running;
    public bool gameover;

    public App(int width, int height) {
        this.graphics = new TermGraphics(width, height);
        this.startTime = DateTime.Now;
        this.lastFrameTime = startTime;
        this.frameTime = 1000 / FPS;

        this.width = width;
        this.height = height;
        this.board = new char[width, height];
        this.view = new char[width, height];

        this.loadScoreboard();
    }

    public void loadScoreboard(){
        this.scoreboard = new Dictionary<string, int>();
        if(File.Exists(scoreboardFilename)){
            using (StreamReader sr = File.OpenText(scoreboardFilename)){
                string? line = sr.ReadLine();
                while(line != null){
                    string[] words = line.Split(":");
                    string name = words[0];
                    if(int.TryParse(words[1], out int score))
                        scoreboard.Add(name, score);

                    line = sr.ReadLine();
                }
            }
        }else{
            Console.WriteLine("no file");
        }
    }

    public void saveScoreboard(){
        using (StreamWriter sw = File.CreateText(scoreboardFilename)){
            foreach(KeyValuePair<string, int> entry in scoreboard){
                string line = entry.Key + ":" + entry.Value.ToString();
                sw.WriteLine(line);
            }
        }
    }

    public void init(){
        this.running = true;
        this.shapeFallTimer = 0;
        this.shapeFallTime = 40;
        this.shapeMinTime = 8;

        for(int i = 0; i < height; i++)
            for(int j = 0; j < width; j++){
                board[j, i] = '-';
                view[j, i] = '-';
            }

        this.currentShape = this.newShape();
        this.nextShapes = new Shape[3];
        for(int i = 0; i < 3; i++)
            this.nextShapes[i] = newShape();

        this.graphics.setShapes(this.nextShapes);
        this.gameover = false;
    }

    private Shape newShape(){
        Shape shape = Shape.random();
        shape.setPos(this.width/2-1, 0);
        return shape;
    }

    public void handleInput(){
        Shape shape = this.currentShape;
        int x = shape.x,
            y = shape.y;

        Rect rect = shape.rect;

        while(Console.KeyAvailable){
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch(key.Key){
                case ConsoleKey.Q: 
                    this.exit();
                    break;
                case ConsoleKey.A:
                    this.detectCollision();
                    shape.moveLeft();
                    break;
                case ConsoleKey.D:
                    this.detectCollision();
                    shape.moveRight();
                    break;
                case ConsoleKey.R:
                    shape.rotate();
                    rect = shape.rect;
                    while(shape.x + rect.x < 0) shape.x++;
                    while(shape.x + rect.x + rect.w >= width) shape.x--;
                    while(shape.y + rect.y + rect.h >= height) shape.y--;

                    break;
                case ConsoleKey.S:
                    while(shape.fallDown())
                        this.detectCollision();
                        this.onBlockFall();
                    break;
            }
        }
    }

    public bool handleInputBasic() {
        while(Console.KeyAvailable){
            ConsoleKeyInfo key = Console.ReadKey(true);
            switch(key.Key){
                case ConsoleKey.R:
                    this.init();
                    Console.Clear();
                    this.gameover = false;
                    return true;
                case ConsoleKey.Q:
                    this.exit();
                    return true;
            }
        }
        return false;
    }

    private void renderCurrentShape(bool overlay=true){
        int x = currentShape.x,
            y = currentShape.y,
            w = currentShape.w,
            h = currentShape.h;
        
        char[,] shape = currentShape.getShape();

        for(int j = 0; j < h && y+j < height; j++)
            for(int i = 0; i < w && x+i < width; i++){
                if(x+i < 0 || x+i >= width || y+j < 0 || y+j >= height) 
                    continue;

                if(overlay && (shape[j, i] == '-'))
                    continue;

                this.view[x+i, y+j] = shape[j, i];
            }
    }

    private void updateView(bool overlay=false){
        for(int j = 0; j < height; j++)
            for(int i = 0; i < width; i++)
                if(overlay || !overlay && board[i, j] != '-')
                    view[i, j] = board[i,j];
    }

    private void clearCurrentShape(){
        int x = currentShape.x,
            y = currentShape.y,
            w = currentShape.w,
            h = currentShape.h;
        
        char[,] shape = currentShape.getShape();

        for(int j = 0; j < h && y+j < height; j++)
            for(int i = 0; i < w && x+i < width; i++){
                if((shape[j, i] == '-'))
                    continue;

                this.view[x+i, y+j] = '-';
            }
    }

    private void render(){
        this.renderCurrentShape();
        this.updateView();

        this.graphics.render(view, points, scoreboard);

        this.clearCurrentShape();
    }

    public void awaitNextFrame(){
        DateTime currentFrameTime = DateTime.Now;
        TimeSpan deltaTime = currentFrameTime - lastFrameTime;
        if(deltaTime.Milliseconds < frameTime)
            Thread.Sleep((int)(frameTime - deltaTime.Milliseconds));

        lastFrameTime = currentFrameTime;
    }

    private Collision detectCollision(){
        Shape shape = this.currentShape;
        char[,] shapeMatrix = shape.getShape();
        int x = shape.x,
            y = shape.y,
            w = shape.w,
            h = shape.h;

        Vector[] shapeBlocks = shape.getBlocks();

        int x1 = width, x2 = 0;
        int y1 = height, y2 = 0;
        bool down = false, left = false, right = false;
        foreach(Vector block in shapeBlocks){
            int bx = x + block.x,
                by = y + block.y;
            x1 = Math.Min(x1, bx);
            x2 = Math.Max(x2, bx);
            y1 = Math.Min(y1, by);
            y2 = Math.Max(y2, by);
            down |= (by+1 == height) || (board[bx, by+1] != '-');
            left |= (bx == 0) || (board[bx-1, by] != '-');
            right |= (bx+1 == width) || (board[bx+1, by] != '-');
        }

        this.graphics.setDebugMessage("Shape Rect: (" + x1 + ", " + y1 + ") -> (" + x2 + ", " + y2 + ")");
        this.currentShape.setCollision(left, right, down);
        return currentShape.collision;
    }

    private bool onBoard(int x, int y){
        return (x >= 0 && x < width && y >= 0 && y < height);
    }

    private bool clearRow(int y){
        int filled = 0;
        for(int i = 0; i < width; i++)
            filled += (board[i, y] != '-')? 1 : 0;

        if(filled == width){
            this.points += 1000;
            for(int i = 0; i < width; i++)
                for(int j = y; j > 0; j--)
                    board[i, j] = board[i, j-1];

            return true;
        }else return false;
    }

    private void onBlockFall(){
        Shape shape = this.currentShape;
        int x = shape.x,
            y = shape.y;

        Rect rect = shape.rect;

        Collision collision = this.detectCollision();
        if(!collision.down) this.currentShape.fallDown();
        else {
            points += 100;
            Vector[] blocks = shape.getBlocks();
            foreach(Vector block in blocks)
                board[x+block.x, y+block.y] = shape.value;

            bool cleared = false;
            for(int i = height-1; i >= 0; i--)
                while(cleared = this.clearRow(i));

            this.updateView(true);

            this.currentShape = this.nextShapes[0];
            for(int i = 1; i < 3; i++)
                this.nextShapes[i-1] = this.nextShapes[i];
            this.nextShapes[2] = this.newShape();
            this.graphics.setShapes(this.nextShapes);

            for(int i = 0; i < width; i++){
                if(board[i, 0] != '-' || board[i, 1] != '-'){
                    this.gameover = true;
                    return;
                }
            }
        }
    }

    private void onLevelUp(){
        if(++level % speedUpLevel == 0){
            if(shapeFallTime < shapeMinTime)
                shapeFallTime--;
        }
    }

    public void tick(){
        this.handleInput();
        frames++;

        if(++shapeFallTimer == shapeFallTime){
            this.detectCollision();
            this.onBlockFall();
            shapeFallTimer = 0;
        }

        this.render();
        this.awaitNextFrame();
    }

    public void exit(){ this.running = false; }
}

