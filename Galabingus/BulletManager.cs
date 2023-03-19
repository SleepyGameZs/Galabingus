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
        private static BulletManager s_instance = null;

        // List of bullets
        private List<Bullet> l_obj_activeBullets;
        private List<ushort> l_ush_content;

        // Bullet Total
        private ushort ush_totalBullets;

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

            ush_totalBullets = 0;

            vc2_screenSize = new Vector2(
                GraphicsDeviceManager.DefaultBackBufferWidth, // Width of screen
                GraphicsDeviceManager.DefaultBackBufferHeight // Height of screen
                );

        }

        #endregion

        #region------------------[ Methods ]------------------

        public void CreateBullet (BulletType BT_ability, Vector2 vc2_position, int int_angle, int int_direction)
        {
            // Normalize and set speed based on bullet type
            ushort ush_sprite = GameObject.Instance.Content.smallbullet_strip4;
            switch (BT_ability)
            {
                case BulletType.Normal:
                    ush_sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Bouncing:
                    ush_sprite = GameObject.Instance.Content.tinybullet_strip4;
                    break;

                case BulletType.Splitter:
                    ush_sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Circle:
                    ush_sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Large:
                    ush_sprite = GameObject.Instance.Content.bigbullet_strip4;
                    break;

                case BulletType.Seeker:
                    ush_sprite = GameObject.Instance.Content.circlebullet_strip4;
                    break;
            }

            // Add sprite linker to list
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

            // Add bullet itself to list
            l_obj_activeBullets.Add(new Bullet(BT_ability, vc2_position, int_angle, int_direction, ush_sprite, ush_totalBullets));
            
            // Increment count
            ush_totalBullets++;

        }

        public void Update(GameTime gameTime)
        {
            //Debug.WriteLine(sprite);
            for (int i = 0; i < l_obj_activeBullets.Count; i++)
            {
                // Runs the bullet's update.
                l_obj_activeBullets[i].Update(gameTime);

                // Checks if bullet is set to be destroyed.
                if (l_obj_activeBullets[i].Destroy)
                {
                    l_obj_activeBullets.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        public void Draw()
        {
            foreach (Bullet obj_bullet in l_obj_activeBullets)
            {
                // Convert angle
                float flt_direction = (float)MathHelper.ToRadians(obj_bullet.Angle);

                // Get direction
                SpriteEffects spx_direction;
                if (obj_bullet.Direction < 1)
                {
                    spx_direction = SpriteEffects.None;
                    flt_direction += (float)Math.PI;
                } else
                {
                    spx_direction = SpriteEffects.FlipHorizontally;
                }

                GameObject.Instance.SpriteBatch.Draw(
                    obj_bullet.Sprite,                  // The sprite-sheet for the player
                    obj_bullet.Position,                // The position for the player
                    obj_bullet.Transform,               // The scale and bounding box for the animation
                    obj_bullet.Color,                   // The color for the palyer
                    flt_direction,                      // rotation uses the velocity
                    Vector2.Zero,                       // Starting render position
                    obj_bullet.Scale,                   // The scale of the sprite
                    SpriteEffects.None,                 // Which direction the sprite faces
                    0.0f                                // Layer depth of the player is 0.0
                );
            }
        }

        #endregion


    }
}
