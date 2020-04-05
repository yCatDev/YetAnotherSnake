using Microsoft.Xna.Framework;
using Nez;
using Nez.BitmapFonts;
using Nez.Systems;
using Nez.UI;

namespace YetAnotherSnake
{
    public class GameSkin
    {
        public Skin Skin;

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

            var slider_style = SliderStyle.Create(Color.Yellow, new Color(61, 9, 107));
            
            slider_style.Knob.MinWidth *= 1.5f;
            slider_style.Knob.MinHeight *= 1.5f;
            slider_style.Background.MinWidth *= 0.5f;

            Skin.Add("slider", slider_style);

        }
    }
}