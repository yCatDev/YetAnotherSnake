using System.Collections;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;

namespace YetAnotherSnake
{
    public static class Animations
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


        
    }
}