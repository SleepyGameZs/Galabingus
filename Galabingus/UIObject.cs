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

        protected ContentManager cm;
        protected Texture2D uiTexture;
        protected Rectangle uiPosition;
        protected Color clearColor;

        #endregion

        public UIObject(string filename, ContentManager cm, Vector2 position, int scale)
        {
            uiTexture = cm.Load<Texture2D>(filename);
            uiPosition =
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    (uiTexture.Width / scale),
                    (uiTexture.Height / scale));
        }

        #region Methods

        public abstract void Update();

        public abstract void Draw(SpriteBatch sb);
        #endregion
    }
}
