
public class TermGraphics{
    private enum GameState{
        IDLE,
        RUNNING,
        SUBMIT_SCORE,
        GAMEOVER
    };

    private Rect boardPanel;
    private Rect scorePanel;
    private Rect shapePanel;

    public int width;
    public int height;
    public int border = 1;

    private struct DebugData{
        public bool enabled;
        public string message;
    }

    private Shape[] nextShapes;

    DebugData debug;

    private static readonly ConsoleColor[] colors = {
        ConsoleColor.Cyan,
        ConsoleColor.Yellow,
        ConsoleColor.Green,
        ConsoleColor.Red,
        ConsoleColor.Blue,
        ConsoleColor.Magenta,
        ConsoleColor.White,
    };

    private const string shapes = "IJLBSZT";
    public TermGraphics(int w, int h){
        this.scorePanel = new Rect(1, 1, 20, h);
        this.boardPanel = new Rect(22, 1, w, h);
        this.shapePanel = new Rect(23+w, 1, 10, h);
        this.width = w + 34;
        this.height = h + 2;
        this.debug.enabled = false;

        this.nextShapes = new Shape[3];
    }

    public void clear(){
        Console.Clear();
    }

    public void setShapes(Shape[] shapes){
        this.nextShapes = shapes;
    }

    public void setDebugMessage(string message){
        this.debug.enabled = !(message.Equals(""));
        this.debug.message = message;
    }

    public void renderRunning(char[,] board, int score, Scoreboard scoreboard){
        int[] borders = {
            0,
            boardPanel.x+border,
            shapePanel.x+2*border,
            this.width
        };

        Rect[] shapePanelShapes = {
            new Rect(shapePanel.x + 2, shapePanel.y + 1, 4, 4),
            new Rect(shapePanel.x + 2, shapePanel.y + 6, 4, 4), 
            new Rect(shapePanel.x + 2, shapePanel.y + 11, 4, 4),
        };

        string scoreStr = "   YOU " + score.ToString();

        for(int i = 0; i < this.height; i++){
            for(int j = 0; j < this.width; j++){
                if(
                        scorePanel.contains(j, i) ||
                        boardPanel.contains(j, i) ||
                        shapePanel.contains(j, i)
                  ) switch(j){
                    case int n when(scorePanel.contains(n, i)):
                        switch(i){
                            case 2:
                                Console.ForegroundColor = (score > scoreboard.at(0).Key)? ConsoleColor.Yellow
                                    : (score > scoreboard.at(1).Key)? ConsoleColor.Red
                                    : (score > scoreboard.at(2).Key)? ConsoleColor.Blue
                                    : ConsoleColor.White;
                                if(n >= 2 && n < scoreStr.Length+2)
                                    Console.Write(scoreStr[n-2]);
                                else 
                                    Console.Write(' ');
                                break;
                            case int m when(m >= 4 && m < scoreboard.size() + 4):
                                Console.ForegroundColor = (m == 4) ? ConsoleColor.Yellow
                                    : (m == 5) ? ConsoleColor.Red
                                    : (m == 6) ? ConsoleColor.Blue
                                    : ConsoleColor.DarkGray;
                                     
                                KeyValuePair<int, string> entry = scoreboard.at(scoreboard.size() - (m-4) - 1);
                                string scoreText = entry.Key + " " + entry.Value.ToString();
                                if(n >= 2 && n < scoreText.Length+2)
                                    Console.Write(scoreText[n-2]);
                                else 
                                    Console.Write(' ');
                                break;
                            default:
                                Console.Write(' ');
                                break;
                        }
                        break;
                    case int n when(boardPanel.contains(n, i)):
                        int x = n - boardPanel.x;
                        int y = i - 1;
                        char value = board[x, y];
                        int index = shapes.IndexOf(value);
                        Console.ForegroundColor = (index != -1)
                            ? colors[index]
                            : ConsoleColor.DarkGray;
                        Console.Write(value);
                        break;
                    case int n when(shapePanel.contains(n, i)):
                        char[,] shapeData;
                        int shapeIndex
                            = shapePanelShapes[0].contains(n, i) ? 0
                            : shapePanelShapes[1].contains(n, i) ? 1
                            : shapePanelShapes[2].contains(n, i) ? 2
                            : -1; 

                        if(shapeIndex == -1){
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(' ');
                        } else {
                            Rect rect = shapePanelShapes[shapeIndex];
                            Shape shape = nextShapes[shapeIndex];
                            shapeData = shape.getShape();
                            char nextChar = shapeData[n - rect.x, i - rect.y];

                            Console.ForegroundColor = (nextChar != '-')
                                ? colors[shapes.IndexOf(shape.value)]
                                : ConsoleColor.DarkGray;
                            Console.Write(nextChar);
                        }

                        break;
                } else {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write('#');
                }
            }

            Console.WriteLine();
        }

        if(this.debug.enabled){
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine(this.debug.message);
            Console.SetCursorPosition(0, Console.CursorTop - 2);
        }
        Console.SetCursorPosition(0, Console.CursorTop - this.height);
    }

    private string getSubmitNameRender(SubmitData data){
        string res = "";
        foreach(char c in data.name.getName())
            res += c + " ";
        return res;
    }

    public void renderSubmitScore(SubmitData submitData){
        Console.WriteLine();
        Console.WriteLine("\tG A M E O V E R");
        Console.WriteLine();
        Console.WriteLine("\tScore: " + submitData.getScore().ToString());
        Console.WriteLine("\tName: " + this.getSubmitNameRender(submitData));

        Console.SetCursorPosition(0, Console.CursorTop-5);
    }

    public void renderGameOver(){
        Console.WriteLine();
        Console.WriteLine("\tG A M E O V E R");
        Console.WriteLine();
        Console.WriteLine("\t R to restart");
        Console.WriteLine("\t Q to quit");

        Console.SetCursorPosition(0, Console.CursorTop - 5);

    }
}
