using Microsoft.Xna.Framework;
using Nez;

namespace YetAnotherSnake.Scenes
{
    
    /// <summary>
    /// Empty scene for creating animated transition of other scenes
    /// </summary>
    public class BlankScene: Scene
    {
        public override void OnStart()
        {
            base.OnStart();
            ClearColor = Color.Black;
            //Load menu scene
            var b = MyGame.GameInstance.SaveSystem.SaveFile.IsFullScreen;
            if (b)
                Screen.SetSize(Screen.MonitorWidth, Screen.MonitorHeight);
            else
            {
                Screen.SetSize((int) (Screen.MonitorWidth * 0.75f), (int) (Screen.MonitorHeight * 0.75f));
            }
            
            Screen.IsFullscreen = b;

            b = MyGame.GameInstance.SaveSystem.SaveFile.IsVignette;
            MyGame.GameInstance.VignettePostProcessor.Enabled = b;

            b = MyGame.GameInstance.SaveSystem.SaveFile.IsBloom;
            MyGame.GameInstance.BloomPostProcessor.Enabled = b;
            Core.StartSceneTransition(new FadeTransition(() => new MenuScene()));
        }
    }
}