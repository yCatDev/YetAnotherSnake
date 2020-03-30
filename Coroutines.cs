using System.Collections;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using YetAnotherSnake.Scenes;

namespace YetAnotherSnake
{
    public static class Coroutines
    {

        public static IEnumerator SpawnFood(Entity food)
        {
            var oldScale = food.Scale;
            food.Scale = Vector2.Zero;
            
            while (food.Scale!=oldScale)
            {
                food.Scale = Utils.Move(food.Scale, oldScale, 0.05f);
                yield return null;
            }
        }

        public static IEnumerator MoveTextToCenter(Entity t)
        {
            var bounds = t.Scene.Camera.Bounds;
            var to = new Vector2 {X = bounds.Right / 10, Y = bounds.Bottom / 10};
            while (t.Position!=to)
            {
                t.Position = Utils.Move(t.Position, to, 0.05f);
                yield return null;
            }
        }

        public static IEnumerator ReturnToMenu()
        {
            float t = 100;
            while (t > 0)
            {
                t--;
                yield return null;
            }
            
            Core.StartSceneTransition(new FadeTransition(() => new MenuScene()));
            
            yield return null;
        }
        
    }
}