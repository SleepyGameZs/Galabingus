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
    internal class Slider : UIElement
    {
        #region Events

        public event EventDelegate OnSlide;

        #endregion

        #region Fields

        //the current mouseState
        private MouseState currentMS;
        private MouseState prevMS;

        Texture2D knotchTexture;
        Rectangle knotchPosition;
        float scale;
        float unitLength;
        float returnPercentage;

        int difference;

        bool hover;

        #endregion

        #region Properties

        public float ReturnPercentage
        {
            get { return returnPercentage; }
        }

        #endregion

        #region Constructor

        public Slider(Texture2D background, Texture2D knotch, Vector2 position, float scale)
            :base(background, position, scale)
        {
            knotchTexture = knotch;

            this.scale = scale;

            //sets the length and width of the object
            int length = (int)(knotchTexture.Width / scale);
            int width = (int)(knotchTexture.Height / scale);

            unitLength = (float)(length * 1.1);
            difference = (int)(uiPosition.X * 1.1 - uiPosition.X);

            //creates its position rectangle
            knotchPosition =
                new Rectangle(
                    uiPosition.X,
                    uiPosition.Y,
                    length,
                    width
                );

            hover = false;

        }

        #endregion

        #region Methods

        public override void Update()
        {

            if (uiPosition.Y == UIManager.Instance.ButtonSelection && (UIManager.Instance.SingleKeyPress(Keys.D) || UIManager.Instance.SingleKeyPress(Keys.Right)))
            {
                knotchPosition.X = knotchPosition.X + 20;

                if (knotchPosition.X + unitLength >= uiPosition.X + (uiTexture.Width / scale) * 1.1)
                {
                    knotchPosition.X = (int)(uiPosition.X + ((int)(uiTexture.Width / scale)) * 1.1) - (int)unitLength;
                    returnPercentage = 1;
                }
                else
                {
                    returnPercentage = (currentMS.Position.X - prevMS.Position.X) / unitLength;
                }

                if (OnSlide != null)
                    OnSlide(this);
            }
            else if (uiPosition.Y == UIManager.Instance.ButtonSelection && (UIManager.Instance.SingleKeyPress(Keys.A) || UIManager.Instance.SingleKeyPress(Keys.Left)))
            {
                knotchPosition.X = knotchPosition.X - 20;

                if (knotchPosition.X <= uiPosition.X)
                {
                    knotchPosition.X = uiPosition.X;
                    returnPercentage = 0;
                }
                else
                {
                    returnPercentage = knotchPosition.X - uiPosition.X / (uiPosition.Width - unitLength);
                }

                if (OnSlide != null)
                    OnSlide(this);
            }

            else if(uiPosition.Y == UIManager.Instance.ButtonSelection)
            {
                if(!hover)
                {
                    knotchPosition.Width = (int)(knotchPosition.Width * 1.1);
                    knotchPosition.Height = (int)(knotchPosition.Height * 1.1);

                    uiPosition.Width = (int)(uiPosition.Width * 1.1);
                    uiPosition.Height = (int)(uiPosition.Height * 1.1);

                    knotchPosition.X = knotchPosition.X + (int)((unitLength / 1.1) * returnPercentage);

                    hover = true;
                }
            }
            else
            {
                if (hover)
                {
                    knotchPosition.Width = (int)(knotchPosition.Width / 1.1);
                    knotchPosition.Height = (int)(knotchPosition.Height / 1.1);

                    uiPosition.Width = (int)(uiPosition.Width / 1.1);
                    uiPosition.Height = (int)(uiPosition.Height / 1.1);

                    knotchPosition.X = knotchPosition.X - (int)((difference / 1.1) * returnPercentage);

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

            sb.Draw(
                knotchTexture,
                knotchPosition,
                clearColor);
        }

        #endregion

    }
}
