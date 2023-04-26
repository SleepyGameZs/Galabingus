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

            unitLength = length;

            //creates its position rectangle
            knotchPosition =
                new Rectangle(
                    ((int)uiPosition.X) - (length / 2),
                    ((int)uiPosition.Y) - (width / 2),
                    length,
                    width
                );

        }

        #endregion

        #region Methods

        public override void Update()
        {
            currentMS = Mouse.GetState();

            if (knotchPosition.Contains(currentMS.Position))
            {
                if(currentMS.Position.X != knotchPosition.X)
                {
                    if (knotchPosition.X <= uiPosition.X)
                    {
                        knotchPosition.X = uiPosition.X;
                        returnPercentage = 0;
                    }
                    else if (knotchPosition.X + unitLength >= uiPosition.X + uiTexture.Width / scale)
                    {
                        knotchPosition.X = uiPosition.X - (int)unitLength;
                        returnPercentage = 1;
                    }
                    else
                    {
                        returnPercentage = (currentMS.Position.X - prevMS.Position.X) / unitLength;
                        knotchPosition.X = currentMS.Position.X;
                    }

                    if (OnSlide != null)
                        OnSlide(this);

                }
            }

            prevMS = Mouse.GetState();
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
