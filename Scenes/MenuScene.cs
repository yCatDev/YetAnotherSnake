using Microsoft.Xna.Framework;
using Nez;
using YetAnotherSnake.Components;

namespace YetAnotherSnake.Scenes
{
    public class MenuScene: Scene
    {
        public override void Initialize()
        {
            base.Initialize();
            
            SetDesignResolution(1280, 720, SceneResolutionPolicy.ShowAll);
            ClearColor = Color.Black;
            var gridEntity = CreateEntity("grid");
            gridEntity.AddComponent(new SpringGrid(new Rectangle(0, 0, 1280, 720), new Vector2(30))
            {
                GridMinorThickness = 0f,
                GridMajorThickness = 3,
                GridMajorColor = Color.Purple
            });
            var test = CreateEntity("Test", new Vector2(0, 0))
                .AddComponent<TextComponent>().AddComponent(new GridModifier());
            
            var text = test.GetComponent<TextComponent>();
            text.SetText("Hello World");
            text.Color = Color.White;
            text.Transform.Scale = new Vector2(10,10);
            
            AddPostProcessor(new VignettePostProcessor(1){Power = 0.25f});
            AddPostProcessor(new BloomPostProcessor(3)).Settings = BloomSettings.PresetSettings[0];
        }
    }
}