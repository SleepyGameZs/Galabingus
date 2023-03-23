using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

// Zane Smith

namespace Galabingus
{
    public sealed class BulletManager
    {
        #region------------------[ Fields ]------------------

        // actual variable attached to singleton calling property
        private static BulletManager instance = null;

        // List of bullets
        private List<Bullet> activeBullets;
        private List<ushort> content;

        // Bullet Total
        private ushort totalBullets;

        // Screen data
        private Vector2 screenSize;

        #endregion

        #region------------------[ Parameters ]------------------

        /// <summary>
        /// Reference to the Bullet Manager (use BMConstructor method to make a new bullet manager)
        /// </summary>
        public static BulletManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BulletManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Used to get screen dimensions for bullets
        /// </summary>
        public Vector2 ScreenDimensions
        {
            get { 
                return screenSize;
            }
        }

        #endregion

        #region------------------[ Constructor ]------------------

        private BulletManager()
        {
            activeBullets = new List<Bullet>();
            content = new List<ushort>();

            totalBullets = 0;

            screenSize = new Vector2(
                GraphicsDeviceManager.DefaultBackBufferWidth, // Width of screen
                GraphicsDeviceManager.DefaultBackBufferHeight // Height of screen
                );

        }

        #endregion

        #region------------------[ Methods ]------------------

        public void CreateBullet (BulletType ability, Vector2 position, int angle, int direction)
        {
            // Normalize and set speed based on bullet type
            ushort sprite = GameObject.Instance.Content.smallbullet_strip4;
            switch (ability)
            {
                case BulletType.Normal:
                    sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Bouncing:
                    sprite = GameObject.Instance.Content.tinybullet_strip4;
                    break;

                case BulletType.Splitter:
                    sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Circle:
                    sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Large:
                    sprite = GameObject.Instance.Content.bigbullet_strip4;
                    break;

                case BulletType.Seeker:
                    sprite = GameObject.Instance.Content.circlebullet_strip4;
                    break;
            }

            // Add sprite linker to list
            if (content.Count == 0)
            {
                content.Add(sprite);
            }
            else
            {
                bool foundSprite = false;
                foreach (ushort asset in content)
                {
                    if (asset == sprite)
                    {
                        foundSprite = true;
                    }
                }
                if (!foundSprite)
                {
                    content.Add(sprite);
                }
            }

            // Add bullet itself to list
            activeBullets.Add(new Bullet(ability,        // Ability of the bullet to shoot
                                         position,      // Position to spawn the bullet
                                         angle,         // Angle to move the bullet
                                         direction,     // Direction of the bullet
                                         sprite,        // Sprite of the bullet
                                         totalBullets   // total count of bullets
                                         )
                              );
            
            // Increment count
            totalBullets++;

        }

        public void Update(GameTime gameTime)
        {
            //Debug.WriteLine(sprite);
            for (int i = 0; i < activeBullets.Count; i++)
            {
                // Runs the bullet's update.
                activeBullets[i].Update(gameTime);

                // Checks if bullet is set to be destroyed.
                if (activeBullets[i].Destroy)
                {
                    activeBullets.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        public void Draw()
        {
            foreach (Bullet bullet in activeBullets)
            {
                // Convert angle
                float direction = (float)MathHelper.ToRadians(bullet.Angle);

                // Get direction
                SpriteEffects directionSpriteEffects;
                if (bullet.Direction < 1)
                {
                    directionSpriteEffects = SpriteEffects.None;
                    direction += (float)Math.PI;
                } else
                {
                    directionSpriteEffects = SpriteEffects.FlipHorizontally;
                }

                GameObject.Instance.SpriteBatch.Draw(
                    bullet.Sprite,                  // The sprite-sheet for the player
                    bullet.Position,                // The position for the player
                    bullet.Transform,               // The scale and bounding box for the animation
                    bullet.Color,                   // The color for the palyer
                    direction,                      // rotation uses the velocity
                    Vector2.Zero,                       // Starting render position
                    bullet.Scale,                   // The scale of the sprite
                    SpriteEffects.None,                 // Which direction the sprite faces
                    0.0f                                // Layer depth of the player is 0.0
                );
            }
        }

        #endregion
    }
}
