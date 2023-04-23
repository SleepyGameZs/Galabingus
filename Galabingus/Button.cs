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

        //a second texture for hover
        private Texture2D baseTexture;
        private Texture2D hoverTexture;

        //list of UIElements for new menu
        private List<UIElement> displayMenu;

        #endregion

        #region Properties

        public Texture2D BaseTexture
        {
            get { return baseTexture; }
            set { baseTexture = value; }
        }

        public Texture2D HoverTexture
        {
            get { return hoverTexture; }
            set { hoverTexture = value; }
        }

        public List<UIElement> DisplayMenu
        {
            get { return displayMenu; }
            set { displayMenu = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// instantiates a basic button
        /// </summary>
        /// <param name="texture">its texure</param>
        /// <param name="position">its position rectangle</param>
        public Button
            (Texture2D texture, Vector2 position, float scale)
            : base(texture, position, scale)
        {
            baseTexture = texture;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Determines if the button is selected by keyboard.
        /// </summary>
        private bool KeyboardTakeOver()
        {
            if (UIManager.Instance.KeyboardTakeOver && UIPosition.Y == UIManager.Instance.ButtonSelection)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Update()
        {
            currentMS = Mouse.GetState();

            if (uiPosition.Contains(currentMS.Position) && !KeyboardTakeOver())
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
            else if (KeyboardTakeOver())
            {
                if (OnHover != null)
                    OnHover(this);
            }
            else
            {
                if (UIPosition.Y == UIManager.Instance.ButtonSelection && Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if (OnClick != null)
                        OnClick(this);
                }

                if(uiTexture != baseTexture && UIPosition.Y != UIManager.Instance.ButtonSelection)
                    uiTexture = baseTexture;
                else if(clearColor != Color.White)
                    clearColor = Color.White;

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
