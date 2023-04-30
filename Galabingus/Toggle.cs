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

        bool enabled;
        bool hover;

        Texture2D enabledTexture;
        Texture2D disabledTexture;

        Vector2 sizeBase;
        Vector2 sizeHover;

        #endregion

        #region CTOR

        public Toggle(Texture2D disabledTexture, Texture2D enabledTexture, Vector2 position, float scale)
            : base(disabledTexture, position, scale)
        {
            enabled = false;

            this.disabledTexture = disabledTexture;
            this.enabledTexture = enabledTexture;

            sizeBase = new Vector2(uiPosition.Width, uiPosition.Height);
            sizeHover = new Vector2(uiPosition.Width * 1.1f, uiPosition.Height * 1.1f);
        }

        #endregion

        #region Methods

        public override void Update()
        {
            if (uiPosition.Y == UIManager.Instance.ButtonSelection && (UIManager.Instance.SingleKeyPress(Keys.Enter) || UIManager.Instance.SingleKeyPress(Keys.Space)))
            {
                if (enabled == true)
                {
                    enabled = false;
                    uiTexture = disabledTexture;
                    if (OnDisable != null)
                        OnDisable(this);
                }
                else
                {
                    enabled = true;
                    uiTexture = enabledTexture;
                    if (OnEnable != null)
                        OnEnable(this);
                }

            }
            else if (uiPosition.Y == UIManager.Instance.ButtonSelection)
            {
                if (!hover)
                {
                    uiPosition.Width = (int)(sizeHover.X);
                    uiPosition.Height = (int)(sizeHover.Y);

                    hover = true;
                }
            }
            else
            {
                if(hover)
                {
                    uiPosition.Width = (int)(sizeBase.X);
                    uiPosition.Height = (int)(sizeBase.Y);

                    hover = false;
                }
            }
        }

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
