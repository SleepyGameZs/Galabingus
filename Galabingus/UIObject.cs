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

        protected Texture2D uiTexture; //its texture
        protected Rectangle uiPosition; //its position rect
        protected Color clearColor; //its tint

        #endregion

        #region Constructor

        /// <summary>
        /// creates a basic UIObject
        /// </summary>
        /// <param name="uiTexture">the objects texture</param>
        /// <param name="position">the objects position</param>
        /// <param name="scale">the size it needs to be scaled to</param>
        public UIObject(Texture2D uiTexture, Vector2 position, float scale)
        {
            //sets the texture to this classes texture
            this.uiTexture = uiTexture;

            //sets the length and width of the object
            int length = (int)(uiTexture.Width / scale);
            int width = (int)(uiTexture.Height / scale);

            //creates its position rectangle
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
