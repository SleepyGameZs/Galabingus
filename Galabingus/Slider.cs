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

        Texture2D knotchTexture; //the knotch's texture
        Rectangle knotchPosition; //the knotch's position


        float scale; //the scale factor
        float unitLength; //the knotch's length
        float returnPercentage; //the slider's percentage

        //the knotch's base and hover width and height
        Vector2 knotchBase;
        Vector2 knotchHover;

        //the background's base and hover width and height
        Vector2 backBase;
        Vector2 backHover;

        //whether the button is currently being hovered over
        bool hover;

        #endregion

        #region Properties

        /// <summary>
        /// returns the sliders percentage value
        /// </summary>
        public float ReturnPercentage
        {
            get { return returnPercentage; }
        }

        #endregion

        #region Constructor

        public Slider(Texture2D background, Texture2D knotch, Vector2 position, float scale)
            :base(background, position, scale)
        {
            //sets the knotches texture
            knotchTexture = knotch;

            //sets the scale
            this.scale = scale;

            //sets the length and width of the object
            int length = (int)(knotchTexture.Width / scale);
            int width = (int)(knotchTexture.Height / scale);

            //the length of the knotch (when being clicked)
            unitLength = (float)(length * 1.1);

            //creates its position rectangle
            knotchPosition =
                new Rectangle(
                    (uiPosition.Width / 2) + uiPosition.X - (int)(unitLength / 2),
                    uiPosition.Y,
                    length,
                    width
                );

            //set the knotch's base and hover width and height
            knotchBase = new Vector2(knotchPosition.Width, knotchPosition.Height);
            knotchHover = new Vector2(knotchPosition.Width * 1.1f, knotchPosition.Height * 1.1f);

            //set the background's base and hover width and height
            backBase = new Vector2(uiPosition.Width, uiPosition.Height);
            backHover = new Vector2(uiPosition.Width * 1.1f, uiPosition.Height * 1.1f);

            //set the starting return percentage
            returnPercentage = (knotchPosition.X - uiPosition.X) / (uiPosition.Width - unitLength);

            //set hover to false to start
            hover = false;

        }

        #endregion

        #region Methods

        /// <summary>
        /// updates the slider
        /// </summary>
        public override void Update()
        {
            //if the slider is selected and the right / d is being pressed
            if (uiPosition.Y == UIManager.Instance.ButtonSelection && (UIManager.Instance.SingleKeyPress(Keys.D) || UIManager.Instance.SingleKeyPress(Keys.Right)))
            {
                //set the knotch's x up 20 pixels
                knotchPosition.X = knotchPosition.X + 20;

                //if its X exceeds the bounds of the background set it to the edge
                if (knotchPosition.X + unitLength >= uiPosition.X + uiPosition.Width)
                {
                    knotchPosition.X = (int)(uiPosition.X + uiPosition.Width - unitLength);
                    returnPercentage = 1;
                }
                //otherwise just return the current percentage of the knotch
                else
                {
                    returnPercentage = (knotchPosition.X - uiPosition.X) / (uiPosition.Width - unitLength);
                }

                //run the on slide event
                if (OnSlide != null)
                    OnSlide(this);
            }
            //if the slider is selected and the left / a is being pressed
            else if (uiPosition.Y == UIManager.Instance.ButtonSelection && (UIManager.Instance.SingleKeyPress(Keys.A) || UIManager.Instance.SingleKeyPress(Keys.Left)))
            {
                //set the knotch's x down 20 pixels
                knotchPosition.X = knotchPosition.X - 20;

                //if its X exceeds the bounds of the background set it to the edge
                if (knotchPosition.X <= uiPosition.X)
                {
                    knotchPosition.X = uiPosition.X;
                    returnPercentage = 0;
                }
                //otherwise just return the current percentage of the knotch
                else
                {
                    returnPercentage = (knotchPosition.X - uiPosition.X) / (uiPosition.Width - unitLength);
                }

                //run the on slide event
                if (OnSlide != null)
                    OnSlide(this);
            }
            //else the slide is being hovered but not set
            else if(uiPosition.Y == UIManager.Instance.ButtonSelection)
            {
                //if we are not already hovering
                if(!hover)
                {
                    //set the knotch to its hover values
                    knotchPosition.Width = (int)knotchHover.X;
                    knotchPosition.Height = (int)knotchHover.Y;

                    //set the background to its hover values
                    uiPosition.Width = (int)backHover.X;
                    uiPosition.Height = (int)backHover.Y;

                    //scale the knotch position appropriately
                    knotchPosition.X = uiPosition.X + (int)((uiPosition.Width - unitLength) * returnPercentage);

                    //the set hover to true
                    hover = true;
                }
            }
            //then we are not hovering
            else
            {
                //if we are still hovering
                if (hover)
                {
                    //set the knotch to its base values
                    knotchPosition.Width = (int)knotchBase.X;
                    knotchPosition.Height = (int)knotchBase.Y;

                    //set the background to its base values
                    uiPosition.Width = (int)backBase.X;
                    uiPosition.Height = (int)backBase.Y;

                    //scale the knotches position appropriately
                    knotchPosition.X = uiPosition.X + (int)((uiPosition.Width - (unitLength / 1.1)) * returnPercentage);

                    //set hover to false
                    hover = false;
                }
            }
        }

        /// <summary>
        /// draw the slider
        /// </summary>
        /// <param name="sb">the games spriteBatch</param>
        public override void Draw(SpriteBatch sb)
        {
            //draw the background
            sb.Draw(
                uiTexture,
                uiPosition,
                clearColor);

            //draw the knotch
            sb.Draw(
                knotchTexture,
                knotchPosition,
                clearColor);
        }

        //set the sliders starting values
        public void SetSliderStart()
        {
            //run the on slide event
            if (OnSlide != null)
                OnSlide(this);
        }

        #endregion

    }
}
