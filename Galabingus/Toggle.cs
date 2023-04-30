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
    internal class Toggle : UIElement
    {

        #region Events

        public event EventDelegate OnEnable;
        public event EventDelegate OnDisable;

        #endregion

        #region Fields

        //bool if the toggle is enabled
        bool enabled;

        //bool if the toggle is being hovered over
        bool hover;

        Texture2D enabledTexture; //enabled texture
        Texture2D disabledTexture; //disabled texture

        Vector2 sizeBase; //the base size
        Vector2 sizeHover; //the hover size

        #endregion

        #region CTOR

        /// <summary>
        /// creates a new instance of toggle
        /// </summary>
        /// <param name="disabledTexture">the texture for when the toggle is disabled</param>
        /// <param name="enabledTexture">the texture if the toggle is enabled</param>
        /// <param name="position"></param>
        /// <param name="scale"></param>
        public Toggle(Texture2D disabledTexture, Texture2D enabledTexture, Vector2 position, float scale)
            : base(disabledTexture, position, scale)
        {
            //set being enabled to false
            enabled = false;

            //set the local values of enabled and disabled texture
            this.disabledTexture = disabledTexture;
            this.enabledTexture = enabledTexture;

            //set the base and hover sizes
            sizeBase = new Vector2(uiPosition.Width, uiPosition.Height);
            sizeHover = new Vector2(uiPosition.Width * 1.1f, uiPosition.Height * 1.1f);
        }

        #endregion

        #region Methods

        /// <summary>
        /// updates the toggle
        /// </summary>
        public override void Update()
        {
            //if the enter or space key is hit on the toggle and it is selected
            if (uiPosition.Y == UIManager.Instance.ButtonSelection && (UIManager.Instance.SingleKeyPress(Keys.Enter) || UIManager.Instance.SingleKeyPress(Keys.Space)))
            {
                if (enabled == true)
                {
                    //set enabled to false and set its texture to disabled
                    enabled = false;
                    uiTexture = disabledTexture;

                    //run the OnDisable event
                    if (OnDisable != null)
                        OnDisable(this);
                }
                else
                {
                    //set enabled to true and set the texture to enabled
                    enabled = true;
                    uiTexture = enabledTexture;

                    //run the OnEnable event
                    if (OnEnable != null)
                        OnEnable(this);
                }

            }

            //if the toggle is currently selected
            else if (uiPosition.Y == UIManager.Instance.ButtonSelection)
            {
                //if it is not already set to hover
                if (!hover)
                {
                    //change the uiPosition to its hover size
                    uiPosition.Width = (int)(sizeHover.X);
                    uiPosition.Height = (int)(sizeHover.Y);

                    //set hover to true
                    hover = true;
                }
            }
            else
            {
                if(hover)
                {
                    //change the uiPosition to its base size
                    uiPosition.Width = (int)(sizeBase.X);
                    uiPosition.Height = (int)(sizeBase.Y);

                    //set hover to false
                    hover = false;
                }
            }
        }

        /// <summary>
        /// draws the toggle
        /// </summary>
        /// <param name="sb">the games spriteBatch</param>
        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(
                uiTexture,
                uiPosition,
                clearColor);
        }

        #endregion
    }
}
