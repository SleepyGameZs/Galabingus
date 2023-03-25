using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        private Texture2D uiTexture;
        private Rectangle uiPosition;
        Color clearColor;

        #endregion

        #region Methods

        public abstract void Update();

        public abstract void Draw();
        #endregion
    }
}
