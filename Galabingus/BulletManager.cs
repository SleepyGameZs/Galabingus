using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus
{
    public sealed class BulletManager
    {
        #region------------------[ Fields ]------------------

        // actual variable attached to singleton calling property
        private static BulletManager s_instance = null;

        // List of bullets
        private List<Bullet> l_obj_activeBullets;
        private List<ushort> l_ush_content;

        // Screen data
        private Vector2 vc2_screenSize;

        #endregion

        #region------------------[ Parameters ]------------------

        /// <summary>
        /// Reference to the Bullet Manager (use BMConstructor method to make a new bullet manager)
        /// </summary>
        public static BulletManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new BulletManager();
                }
                return s_instance;
            }
        }

        /// <summary>
        /// Used to get screen dimensions for bullets
        /// </summary>
        public Vector2 ScreenDimensions
        {
            get { 
                return vc2_screenSize;
            }
        }

        #endregion

        #region------------------[ Constructor ]------------------

        private BulletManager()
        {
            l_obj_activeBullets = new List<Bullet>();
            l_ush_content = new List<ushort>();

            vc2_screenSize = new Vector2(
                GraphicsDeviceManager.DefaultBackBufferWidth, // Width of screen
                GraphicsDeviceManager.DefaultBackBufferHeight // Height of screen
                );

        }

        #endregion

        #region------------------[ Methods ]------------------

        public void CreateBullet (BulletType BT_ability, Vector2 vc2_position, int int_direction)
        {
            // Normalize and set speed based on bullet type
            ushort ush_sprite = GameObject.Instance.Content.smallbullet_strip4;
            switch (BT_ability)
            {
                case BulletType.Normal:
                    ush_sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Bouncing:
                    ush_sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Splitter:
                    ush_sprite = GameObject.Instance.Content.bigbullet_strip4;
                    break;

                case BulletType.Circle:
                    ush_sprite = GameObject.Instance.Content.bigbullet_strip4;
                    break;
            }

            if (l_ush_content.Count == 0)
            {
                l_ush_content.Add(ush_sprite);
            }
            else
            {
                bool foundSprite = false;
                foreach (ushort asset in l_ush_content)
                {
                    if (asset == ush_sprite)
                    {
                        foundSprite = true;
                    }
                }
                if (!foundSprite)
                {
                    l_ush_content.Add(ush_sprite);
                }
            }

            // Add sprite linker to list
            l_ush_content.Add(ush_sprite);

            l_obj_activeBullets.Add(new Bullet(BT_ability, vc2_position, int_direction, ush_sprite));

        }

        public void Update(GameTime gameTime)
        {
            Debug.WriteLine(l_obj_activeBullets.Count);
            for (int i = 0; i < l_obj_activeBullets.Count; i++)
            {
                // Runs the bullet's update.
                GameObject.Instance.Content = l_ush_content[i];
                l_obj_activeBullets[i].Update(gameTime);

                // Checks if the bullet is on the screen, or set to be destroyed.
                bool bol_bulletOffScreen = l_obj_activeBullets[i].Position.X < 0 &&
                                            l_obj_activeBullets[i].Position.X > vc2_screenSize.X;
                if (bol_bulletOffScreen || l_obj_activeBullets[i].Destroy)
                {
                    l_obj_activeBullets.RemoveAt(i);
                    i -= 1;
                    //l_obj_activeBullets[i].Position -= new Vector2(100, 0); -- use for testing
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < l_obj_activeBullets.Count; i++)
            {
                GameObject.Instance.SpriteBatch.Draw(
                    l_obj_activeBullets[i].Sprite,                          // The sprite-sheet for the player
                    l_obj_activeBullets[i].Position,                        // The position for the player
                    l_obj_activeBullets[i].Transform,                       // The scale and bounding box for the animation
                    l_obj_activeBullets[i].Color,                     // The color for the palyer
                    0.0f,                            // There cannot be any rotation of the player
                    Vector2.Zero,                    // Starting render position
                    l_obj_activeBullets[i].Scale,                      // The scale of the sprite
                    SpriteEffects.None,              // Which direction the sprite faces
                    0.0f                             // Layer depth of the player is 0.0
                );
            }
                
        }

        #endregion


    }
}
