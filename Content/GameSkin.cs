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
        }
    }
}