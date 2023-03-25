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

        private bool visible;

        #endregion

        public Menu(string filename, ContentManager cm, Vector2 position)
            : base(filename, cm, position, 5)
        {
            visible = false;
        }

        #region Methods

        public override void Update()
        {
            throw new NotImplementedException();

            if(UIManager.UserInterface.SingleKeyPress(Keys.Back))
            {
                visible = false;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            throw new NotImplementedException();

            if(visible)
            {
                sb.Draw(
                    uiTexture,
                    uiPosition,
                    clearColor
                );
            }
        }
        #endregion
    }
}
