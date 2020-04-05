using System;

namespace YetAnotherSnake
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Reflection.Assembly.Load("Nez.ImGui");
            var game = new MyGame();
            game.Run();
        }
    }
}
