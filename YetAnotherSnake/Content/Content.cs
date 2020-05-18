using Microsoft.Xna.Framework.Graphics;
using Nez;

namespace YetAnotherSnake
{
    
    /// <summary>
    /// Static class that contains resource paths and some string values
    /// </summary>
    public static class Content
    {
        public const string Save = @"Content/save.file";
        
        public const string Blank = @"Sprites/blank";
        public const string White = @"Sprites/white";
        public const string Circle = @"Sprites/circle";
        public const string SnakeBody = @"Sprites/snake_body";
        public const string SnakeBody2 = @"Sprites/snake_bodyV2";

        public const string BackgroundMusic = @"Audio/bg";
        public const string SoundPickUp = @"Audio/snake_pickup";
        public const string SoundDeath = @"Audio/snake_death";

        public const string UISkin = @"uiskin";
        
        public const string OswaldTitleFont = @"Content/Fonts/oswald_title.fnt";
        public const string DefaultTitleFont = @"Content/Fonts/inria.fnt";
        public const string HowToPlayText = "Use the left and right arrow to rotate the snake head.#The more you eat, the more you become!";
        public const string HowToPlayClassicText = "Just eat food and try not to crash.#The more you eat, the bigger the score.#Try to score the highest score!";
        public const string HowToPlayTimeAttackText = "Eat food as fast as you can.#Avoid stones which do not move.#Try to survive as long as you can!";

    }
}