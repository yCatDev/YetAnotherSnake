using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Nez;
using Random = Nez.Random;

namespace YetAnotherSnake
{
    
    /// <summary>
    /// Any useful methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Classic lerp realization
        /// </summary>
        /// <param name="firstFloat">from value</param>
        /// <param name="secondFloat">to value</param>
        /// <param name="by">by</param>
        /// <returns>Interpolating value</returns>
        public static float Lerp(float firstFloat, float secondFloat, float by)
            => firstFloat * (1 - by) + secondFloat * by;
        
        /// <summary>
        /// Lerp analog fro vectors
        /// </summary>
        /// <param name="from">from vector</param>
        /// <param name="to">to vector</param>
        /// <param name="speed">by</param>
        /// <returns>Interpolating vector</returns>
        public static Vector2 Move(Vector2 from, Vector2 to, float speed)
        {
            float retX = Lerp(from.X, to.X, speed);
            float retY = Lerp(from.Y, to.Y, speed);
            return new Vector2(retX, retY);
        }
        
        /// <summary>
        /// Rotate vector around origin
        /// </summary>
        /// <param name="point">Vector position</param>
        /// <param name="origin">Rotation vector</param>
        /// <param name="rotation">Angle</param>
        /// <returns>Rotated vector position</returns>
        public static Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation) 
            => Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;

        /// <summary>
        /// Rotates one object to another
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Rotate angle</returns>
        public static float LookAt(Transform a, Transform b)
        {
            var angle = Math.Atan2(b.LocalPosition.Y - a.LocalPosition.Y
                , b.LocalPosition.X - a.LocalPosition.X);

            angle = angle * (180/Math.PI);
            return (float) angle;
        }
        
        
    }
}