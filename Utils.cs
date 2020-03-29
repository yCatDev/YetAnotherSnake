using System;
using Microsoft.Xna.Framework;
using Nez;
using Random = Nez.Random;

namespace YetAnotherSnake
{
    public static class Utils
    {
        public static float Lerp(float firstFloat, float secondFloat, float by)
            => firstFloat * (1 - by) + secondFloat * by;
        
        public static Vector2 Move(Vector2 from, Vector2 to, float speed)
        {
            float retX = Lerp(from.X, to.X, speed);
            float retY = Lerp(from.Y, to.Y, speed);
            return new Vector2(retX, retY);
        }
        
        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        } 

        public static float LookAt(Transform a, Transform b)
        {
            var angle = Math.Atan2(b.LocalPosition.Y - a.LocalPosition.Y
                , b.LocalPosition.X - a.LocalPosition.X);

            angle = angle * (180/Math.PI);
            return (float) angle;
        }

        
        
    }
}