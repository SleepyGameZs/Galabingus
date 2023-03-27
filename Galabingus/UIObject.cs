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
    abstract class UIObject
    {
        #region Fields

        protected Texture2D uiTexture;
        protected Rectangle uiPosition;
        protected Color clearColor;

        #endregion

        #region Constructor
        public UIObject(Texture2D uiTexture, Vector2 position, int scale)
        {
            this.uiTexture = uiTexture;

            int length = (uiTexture.Width / scale);
            int width = (uiTexture.Height / scale);

            uiPosition =
                new Rectangle(
                    ((int)position.X) - (length / 2),
                    ((int)position.Y) - (width / 2),
                    length,
                    width
                );
        }
        #endregion

        #region Methods

        public abstract int Update();

        public abstract void Draw(SpriteBatch sb);
        #endregion
    }
}
