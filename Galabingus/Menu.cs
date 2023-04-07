using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    internal class Menu : UIObject
    {
        #region Fields

        #endregion

        #region Constructor
        public Menu(Texture2D texture, Vector2 position)
            : base(texture, position, 1) { }
        #endregion

        #region Methods

        public override int Update()
        {
            if(UIManager.Instance.SingleKeyPress(Keys.Back))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                uiTexture,
                uiPosition,
                Color.White
            );
        }
        #endregion
    }
}
