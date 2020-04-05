using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.UI;
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

       
        
        public static IEnumerator MoveTo(Element el, float x)
        {
            Coroutine.StopLast();
            while (Math.Abs(el.GetX() - x) > 0.1f)
            {
                el.SetX(MathHelper.Lerp(el.GetX(), x, 0.1f));
                yield return null;
            }
            el.SetX(x);
        }
        
    }
}