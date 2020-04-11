using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace YetAnotherSnake
{
    public static class Coroutines
    {


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

       
        
        public static IEnumerator MoveToX(Element el, float x)
        {
            Coroutine.StopLast();
            while (Math.Abs(el.GetX() - x) > 0.1f)
            {
                el.SetX(MathHelper.Lerp(el.GetX(), x, 0.1f));
                yield return null;
            }
            el.SetX(x);
        } 
        public static IEnumerator MoveToY(Element el, float y, Action after = null)
        {
            Coroutine.StopLast();
            while (Math.Abs(el.GetY() - y) > 0.1f)
            {
                
                el.SetY(MathHelper.Lerp(el.GetY(), y, 0.1f));
                yield return null;
            }
            el.SetY(y);
            after?.Invoke();
        }
        
    }
}