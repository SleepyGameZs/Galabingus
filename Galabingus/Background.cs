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
    internal class Background : UIElement
    {

        #region Constructor

        /// <summary>
        /// creates a new instance of the background
        /// </summary>
        /// <param name="texture">the backgrounds texture</param>
        /// <param name="position">the backgrounds position</param>
        /// <param name="scale">the scale to be applied</param>
        public Background(Texture2D texture, Vector2 position, float scale)
            : base(texture, position, scale) { }

        #endregion

        #region Methods

        public override void Update()
        {
            //backgrounds don't really get updated, they might but not in this version
        }

        /// <summary>
        /// draws the background
        /// </summary>
        /// <param name="sb">the games spriteBatch</param>
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
