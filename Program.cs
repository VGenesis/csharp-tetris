try{
    Console.Clear();
    Console.CursorVisible = false;
    App app = new App(15, 25);

    app.init();

    while(app.running)
        app.tick();

    Console.Clear();
    Console.CursorVisible = true;
    app.exit();
} catch( Exception e ) {
    Console.WriteLine(e.Message);
    Console.CursorVisible = true;
}

