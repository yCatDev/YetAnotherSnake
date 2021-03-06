﻿using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;

namespace YetAnotherSnake.UI
{
    /// <summary>
    /// Specific UI animations based on coroutines
    /// </summary>
    public static class UIAnimations
    {
        
        
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