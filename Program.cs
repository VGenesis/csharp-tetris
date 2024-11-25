try{
    Console.Clear();
    Console.CursorVisible = false;
    App app = new App(15, 25);

    app.init();

    while(app.running)
        if(!app.gameover)
            app.tick();
        else {
            Console.Clear();
            Console.WriteLine("#############");
            Console.WriteLine("# GAME OVER #");
            Console.WriteLine("#############");
            Console.WriteLine();
            Console.WriteLine("  R to restart");
            Console.WriteLine("  Q to quit");

            while(!app.handleInputBasic());
        }

    Console.Clear();
    Console.CursorVisible = true;
    app.exit();
} catch( Exception e ) {
    Console.WriteLine(e.Message);
    Console.CursorVisible = true;
}

