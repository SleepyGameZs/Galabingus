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
    internal class Button : UIElement
    {
        #region Events

        public event EventDelegate OnClick;
        public event EventDelegate OnHover;
        public event EventDelegate OnRelease;

        #endregion

        #region Fields

        //the current mouseState
        private MouseState currentMS;
        private MouseState prevMS;

        #endregion

        #region Properties

        #endregion

        #region Constructor

        /// <summary>
        /// instantiates a basic button
        /// </summary>
        /// <param name="texture">its texure</param>
        /// <param name="position">its position rectangle</param>
        public Button
            (Texture2D texture, Vector2 position, int scale)
            : base(texture, position, scale) { }

        #endregion

        #region Methods

        public override void Update()
        {
            currentMS = Mouse.GetState();

            if (uiPosition.Contains(currentMS.Position))
            {
                if (currentMS.LeftButton == ButtonState.Pressed)
                {
                    if (OnClick != null)
                        OnClick(this);

                }
                else
                {
                    if (OnHover != null)
                        OnHover(this);
                }
            }
            else
            {
                if (OnRelease != null)
                    OnRelease(this);
            }

            prevMS = currentMS;
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
