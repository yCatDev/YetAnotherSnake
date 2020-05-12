using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Nez.Systems;
using Nez.UI;

namespace YetAnotherSnake
{
    
    /// <summary>
    /// Class that creates base skin style for game ui
    /// </summary>
    public class GameSkin
    {
        /// <summary>
        /// Nez skin
        /// </summary>
        public readonly Skin Skin;

        /// <summary>
        /// Creates skin styles
        /// </summary>
        /// <param name="contentManager">Content manager</param>
        public GameSkin(NezContentManager contentManager)
        {
            Skin = new Skin();

            Skin.Add("title-label", new LabelStyle()
            {
              Font = contentManager.LoadBitmapFont(Content.OswaldTitleFont)
            });

            Skin.Add("label", new LabelStyle()
            {
                Font = contentManager.LoadBitmapFont(Content.DefaultTitleFont)
            });

            var inputCursor = new PrimitiveDrawable(Color.Black);
            inputCursor.MinHeight = 100;
            inputCursor.MinWidth = 10;
            Skin.Add("inputfield", new TextFieldStyle()
            {
                Font = contentManager.LoadBitmapFont(Content.DefaultTitleFont),
                FontColor = Color.Black,
                Cursor = inputCursor,
                FocusedBackground = new PrimitiveDrawable(Color.Gray),
                Background = new PrimitiveDrawable(Color.White)
            });
            
            Skin.Add("regular-button", TextButtonStyle.Create(Color.Black, new Color(61, 9, 85), new Color(61, 9, 107)));
            
            
            var sliderStyle = SliderStyle.Create(Color.Yellow, new Color(61, 9, 107));
            
            sliderStyle.Knob.MinWidth *= 1.5f;
            sliderStyle.Knob.MinHeight *= 1.5f;
            sliderStyle.Background.MinWidth *= 0.5f;

            Skin.Add("slider", sliderStyle);

        }
    }
}