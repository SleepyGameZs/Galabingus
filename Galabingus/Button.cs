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
            (string filename, ContentManager cm, Vector2 position, Menu menu) 
            : base(filename, cm, position, 5)
        {
            this.menu = menu;
            returnState = default(GameState);
        }

        public Button
            (string filename, ContentManager cm, Vector2 position, GameState returnState)
            : base(filename, cm, position, 5)
        {
            this.returnState = returnState;
            menu = null;
        }

        #endregion

        #region Methods

        public override void Update()
        {
            throw new NotImplementedException();

            mouseState = Mouse.GetState();

            if(uiPosition.Contains(mouseState.Position))
            {
                if(mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (menu == null)
                    {
                        UIManager.UserInterface.UIEvent(this, returnState);
                    }
                    else
                    {
                        UIManager.UserInterface.UIEvent(menu);
                    }
                    
                }
                else
                {
                    clearColor = Color.LightGray;
                }
            }
            else
            {
                clearColor = Color.White;
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
