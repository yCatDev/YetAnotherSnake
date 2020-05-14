using System;

namespace YetAnotherSnake
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var game = new MyGame();
            game.Run();
        }
    }
}
