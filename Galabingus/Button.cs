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
        public event EventDelegate OnHold;
        public event EventDelegate OnRelease;
        public event EventDelegate OnDblClick;

        #endregion

        #region Fields

        //the current mouseState
        private MouseState mouseState;
        private bool hover;

        #endregion

        #region Properties

        public bool Hover
        {
            get { return hover; }
            set { hover = value; }
        }

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
            mouseState = Mouse.GetState();

            if (uiPosition.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    OnClick(this);
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
