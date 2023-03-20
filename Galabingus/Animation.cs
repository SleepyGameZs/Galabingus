using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using static System.Formats.Asn1.AsnWriter;

// Matthew Rodriguez
// 2023, 3, 6
// Animation
// Empty - Empty Animation
// AnimationDuration - Duration of one frame in the animation
// Width - Total width of the anmation sprite
// Height - Total height of the animation sprite
// Play - Plays the animation

namespace Galabingus
{
    /// <summary>
    ///  Provides required 
    ///  information for a animation
    /// </summary>
    internal class Animation
    {
        private static Animation empty = null; // Singelton blank Animation
        private double animationTime;          // Total time ellapsed in the animation
        private float animationDuration;       // The frame duration of the animation
        private int width;                     // Width of the entire animation
        private int height;                    // Height of the entire animation
        private int spritesInAnimation;        // Number of indvidual sprites/frames in animation
        private int currentFrame;              // Current frame in animation
        private double ellapsedTime;

        /// <summary>
        ///  Empty is just a Empty Animation
        ///  Primarily the Empty is a singleton for blank animation
        /// </summary>
        public static Animation Empty
        {
            get
            {
                // When the singleton has not been defined
                // define the singleton
                if (empty != null)
                {
                    return empty;
                }
                else
                {
                    // Defines the singleton as a blank animation
                    empty = new Animation();
                    return empty;
                }
            }
        }

        /// <summary>
        ///  The Animation duration per frame
        /// </summary>
        public float AnimationDuration
        {
            // Allow the animaton frame
            // duration to be adjusted
            get
            {
                return animationDuration;
            }
            set
            {
                animationDuration = value;
            }
        }

        public double EllapsedTime
        {
            get
            {
                return ellapsedTime;
            }
        }

        /// <summary>
        ///  The Animation sprite width
        /// </summary>
        public int Width
        {
            // Allow the animation sprite
            // width to be adjusted
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        /// <summary>
        ///  The Animation sprite height
        /// </summary>
        public int Height
        {
            // Allow the animation
            // sprite height to be adjusted
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        /// <summary>
        ///  ONLY for singleton
        /// </summary>
        private Animation()
        {
            this.animationTime = 0;
            this.animationDuration = 0;
            this.width = 0;
            this.height = 0;
            this.spritesInAnimation = 0;
            this.currentFrame = 1;
        }

        /// <summary>
        ///  Creates a Animation from height, width
        ///  and the spritesInAnimation
        /// </summary>
        /// <param name="width">The width of the entire animation sprite</param>
        /// <param name="height">The height of the entire animation sprite</param>
        /// <param name="spritesInAnimation">The number of sprite sin the animation</param>
        public Animation(int width, int height, int spritesInAnimation)
        {
            this.animationTime = 0;
            this.animationDuration = 0;
            this.ellapsedTime = 0;
            this.width = width;
            this.height = height;
            this.spritesInAnimation = spritesInAnimation;
            this.currentFrame = 1;
        }

        /// <summary>
        ///  Creates a Animation from height, width
        ///  spritesInAnimation, and the currentFrame
        /// </summary>
        /// <param name="width">The width of the entire animation sprite</param>
        /// <param name="height">The height of the entire animation sprite</param>
        /// <param name="spritesInAnimation">The number of sprite sin the animation</param>
        /// <param name="currentFrame">The current frame in the aniomation</param>
        public Animation(int width, int height, int spritesInAnimation, int currentFrame)
        {
            this.animationTime = 0;
            this.animationDuration = 0;
            this.ellapsedTime = 0;
            this.width = width;
            this.height = height;
            this.spritesInAnimation = spritesInAnimation;
            this.currentFrame = currentFrame;
        }

        /// <summary>
        ///  Plays the animation, requires gameTime to play
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <returns>
        ///  Transform of the frame from the animation sprite
        /// </returns>
        public Rectangle Play(GameTime gameTime)
        {
            // Increase the total anmation time
            animationTime += gameTime.ElapsedGameTime.TotalSeconds;

            // Check to see if the animation time has ellapsed past the animation duration
            if (animationTime >= animationDuration)
            {
                // If there are more fames in the animation
                // change the frame to the next frame
                // otherwise siwtch the frame to the first
                if (currentFrame + 1 > spritesInAnimation)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame++;
                }

                // Keep the ellpase offset but make sure
                // that we are a whole duration back
                animationTime -= animationDuration;
            }

            ellapsedTime = gameTime.ElapsedGameTime.TotalSeconds * 60;

            // Return the transform that is formed from the factor of current frame of the width of the sprite
            // The height is always 0, the x will be the frame offset
            return new Rectangle(
                (width / spritesInAnimation * currentFrame),
                0, +
                (int)Math.Round(width / (double)spritesInAnimation),
                height
            );
        }

        /// <summary>
        ///  Plays the animation, requires gameTime to play
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        /// <returns>
        ///  Transform of the frame from the animation sprite
        /// </returns>
        public Rectangle Play(GameTime gameTime, Vector2 velocity, Vector2 position, Rectangle transform, float scale)
        {
            // When offscreen set the transform to empty making the animation non-renderable.
            if (
                position.X - transform.Width * scale > GraphicsDeviceManager.DefaultBackBufferWidth ||
                position.X + transform.Width * scale < 0 ||
                position.Y - transform.Height * scale > GraphicsDeviceManager.DefaultBackBufferHeight ||
                position.Y + transform.Height * scale < 0
            )
            {
                return Rectangle.Empty;
            }

            // On the sub scale adjust spacetime to Lorentz's factor
            float rapidity = (float)Math.Atanh(    (Math.Abs(((position.X)) / (Player.PlayerInstance.Position.X - position.X))) * 0.1f   );
            //float rapidity = (float)Math.Atanh((Math.Abs((((position)) / (Player.PlayerInstance.Position - position)).Length())) * 0.1f);
            rapidity = (rapidity.ToString() == "NaN") ? 0 : rapidity;

            //Debug.WriteLine(  (Math.Abs(( (position.X)) / ( Player.PlayerInstance.Position.X - position.X))) * 0.1f );

            float dilationFactor1 = (float)(1 - Math.Pow(rapidity, 2));
            dilationFactor1 = dilationFactor1 > 0 ? dilationFactor1 : 1;
            float dilationFactor2 = (float)( 1 - (8 * Math.Pow(rapidity, 2) / dilationFactor1 ) );
            dilationFactor2 = dilationFactor2 > 0 ? dilationFactor2 : -dilationFactor2;
            float timeDialiation = (float)Math.Sqrt(dilationFactor2);

            //Debug.WriteLine(timeDialiation);

            if (timeDialiation < 0.7)
            {
                // There should never be a spacetime jump that is greater than what can be perceived
                timeDialiation = 0.7f;
            }

            if (timeDialiation > 1.0675f)
            {
                timeDialiation = 1.0675f;
            }

            ellapsedTime = ((gameTime.ElapsedGameTime.TotalSeconds * 60 * (1 / timeDialiation)) * 0.5f) + (gameTime.ElapsedGameTime.TotalSeconds * 0.5f);

            // Increase the total anmation time
            animationTime += (ellapsedTime / 60);

            ellapsedTime *= Math.Abs(velocity.Length());

            // Check to see if the animation time has ellapsed past the animation duration
            if (animationTime >= animationDuration)
            {
                // If there are more fames in the animation
                // change the frame to the next frame
                // otherwise siwtch the frame to the first
                if (currentFrame + 1 > spritesInAnimation)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame++;
                }

                // Keep the ellpase offset but make sure
                // that we are a whole duration back
                animationTime -= animationDuration;
            }

            // Return the transform that is formed from the factor of current frame of the width of the sprite
            // The height is always 0, the x will be the frame offset
            return new Rectangle(
                (width / spritesInAnimation * currentFrame),
                0, +
                (int)Math.Round(width / (double)spritesInAnimation),
                height
            );
        }


        /// <summary>
        ///  Use this to select a specific sprite in the sprite sheet
        /// </summary>
        /// <param name="currentFrame">Specific sprite in the sprite sheet</param>
        /// <returns>Transform to view this currentFrame</returns>
        public Rectangle GetFrame(int currentFrame)
        {
            return new Rectangle(
                    (width / spritesInAnimation * currentFrame),
                    0, +
                    (int)Math.Round(width / (double)spritesInAnimation),
                    height
                );
        }
    }
}
