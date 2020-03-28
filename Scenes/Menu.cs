using Nez;

namespace YetAnotherSnake.Scenes
{
    public class Menu: Scene
    {
        public override void Initialize()
        {
            base.Initialize();
            SetDesignResolution(800, 600, SceneResolutionPolicy.ShowAllPixelPerfect);
            Screen.SetSize(800,600);
            
        }
    }
}