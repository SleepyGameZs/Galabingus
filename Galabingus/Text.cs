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
    internal class Text : UIElement
    {

        #region CTOR

        /// <summary>
        /// instantiates a basic button
        /// </summary>
        /// <param name="position">its position rectangle</param>
        public Text
            (SpriteFont font, string text, Vector2 position)
            : base(font, text, position) { }

        public Text
            (SpriteFont font, string text, Vector2 position, Color clearColor)
            : base(font, text, position)
        { 
            this.clearColor = clearColor;
        }

        #endregion

        #region Methods

        public override void Update()
        {
            //don't really need to update text for the most part, but hey who knows,
            //maybe a scrolling text box will be in order some day
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.DrawString(
                uiFont,
                uiText,
                uiTextPosition,
                clearColor);
        }

        #endregion
    }
}
