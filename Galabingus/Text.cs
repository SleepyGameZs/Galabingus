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

    public delegate string TextEvent();

    internal class Text : UIElement
    {

        #region Events

        public event TextEvent UpdateText;

        #endregion

        #region CTOR

        /// <summary>
        /// instantiates a basic button
        /// </summary>
        /// <param name="font">the font to be used</param>
        /// <param name="text">the text to be displayed</param>
        /// <param name="position">the x y position</param>
        public Text
            (SpriteFont font, string text, Vector2 position)
            : base(font, text, position) { }

        /// <summary>
        /// instantiates a button with colored text
        /// </summary>
        /// <param name="font">the font to be used</param>
        /// <param name="text">the text to be displayed</param>
        /// <param name="position">the x y position</param>
        /// <param name="clearColor">the color of the text</param>
        public Text
            (SpriteFont font, string text, Vector2 position, Color clearColor)
            : base(font, text, position)
        { 
            this.clearColor = clearColor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// updates the texts content
        /// </summary>
        public override void Update()
        {
            if(UpdateText != null)
                UpdateText();
        }

        /// <summary>
        /// draws the text to screen
        /// </summary>
        /// <param name="sb">the spriteBatch of the game</param>
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
