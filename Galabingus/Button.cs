using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Galabingus
{
    internal class Button : UIObject
    {
        #region Fields

        //the current mouseState
        private MouseState mouseState;

        //objects which represent what the button will do
        //(show a menu, change a state, etc)
        private Menu menu;
        private GameState returnState;

        #endregion

        #region Constructor

        public Button
            (Texture2D texture, Vector2 position)
            : base(texture, position, 5) { }

        #endregion

        #region Methods

        public override int Update()
        {
            mouseState = Mouse.GetState();

            if(uiPosition.Contains(mouseState.Position))
            {
                if(mouseState.LeftButton == ButtonState.Pressed)
                {
                    return 1;

                }
                else
                {
                    clearColor = Color.LightGray;
                    return 0;
                }
            }
            else
            {
                clearColor = Color.White;
                return 0;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                uiTexture,
                uiPosition,
                clearColor
            );
        }

        #endregion
    }
}
