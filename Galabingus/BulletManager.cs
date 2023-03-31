using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

// Creator: Zane Smith
// Purpose: A manager system for all bullets in the game, whether owned by the player or enemies.

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

        // Bullet Created Bullet Storage
        private List<BulletType> storeAbilityBullets;
        private List<Vector2> storePositionBullets;
        private List<int> storeAngleBullets;
        private List<int> storeDirectionBullets;
        private List<object> storeCreatorBullets;

        // Bullet Total
        private ushort bulletTotal;

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
                return Instance.screenSize;
            }
        }

        #endregion

        #region------------------[ Constructor ]------------------

        /// <summary>
        /// Constructor used to generate the bullet manager 
        /// singleton (accessed via property)
        /// </summary>
        private BulletManager()
        {
            activeBullets = new List<Bullet>();

            // Storage data for when a bullet spawns a bullet
            storeAbilityBullets = new List<BulletType>(); 
            storePositionBullets = new List<Vector2>();
            storeAngleBullets = new List<int>();
            storeDirectionBullets = new List<int>();
            storeCreatorBullets = new List<object>();

            bulletTotal = 0;

            content = new List<ushort>();

            screenSize = new Vector2(
                GameObject.Instance.GraphicsDevice.Viewport.Width, // Width of screen
                GameObject.Instance.GraphicsDevice.Viewport.Height // Height of screen
                );

        }

        #endregion

        #region------------------[ Methods ]------------------

        /// <summary>
        /// Method to create a bullet, and have it hooked up to the bullet manager.
        /// </summary>
        /// <param name="ability">The ability the bullet should have</param>
        /// <param name="position">The position to create the bullet at</param>
        /// <param name="angle">The angle the bullet should have</param>
        /// <param name="direction">The direction (left or right) for the bullet</param>
        /// <param name="creator">Reference to the creator of the bullet</param>
        /// <param name="sourceIsBulet">If the thing spawning the bullet is itself a bullet</param>
        public void CreateBullet (BulletType ability, 
                                  Vector2 position, 
                                  int angle, 
                                  int direction, 
                                  object creator, 
                                  bool sourceIsBullet)
        {
            // Sets the sprite to use for the bullet for GameObject storage purposes
            ushort sprite;
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

                default:
                    sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;
            }

            // Add sprite linker to list
            if (Instance.content.Count == 0)
            {
                Instance.content.Add(sprite);
                Debug.WriteLine($"bruh");
            }
            else
            {
                bool foundSprite = false;
                foreach (ushort asset in Instance.content)
                {
                    if (asset == sprite)
                    {
                        foundSprite = true;
                        
                    }
                }
                if (!foundSprite)
                {
                    Instance.content.Add(sprite);
                }
            }

            bool isReplacing = false;
            ushort setNumber = (ushort)Math.Max(0, (Instance.activeBullets.Count - 1));

            //Debug.WriteLine($"COUNT: {activeBullets.Count}");

            for (int i = 0; i < Instance.activeBullets.Count; i++)
            {
                if (Instance.activeBullets[i] == null)
                {
                    //Debug.WriteLine($"Found Reusable Val at: {i}");
                    setNumber = (ushort)(i);
                    isReplacing = true;
                    break;
                }
            }

            // Add bullet itself to list
            if (sourceIsBullet)
            {
                Instance.storeAbilityBullets.Add(ability);
                Instance.storePositionBullets.Add(position);
                Instance.storeAngleBullets.Add(angle);
                Instance.storeDirectionBullets.Add(direction);
                Instance.storeCreatorBullets.Add(creator);

            } 
            else
            {
                if (isReplacing == false)
                {
                    Instance.activeBullets.Add(new Bullet(ability,       // Ability of the bullet to shoot
                                                         position,      // Position to spawn the bullet
                                                         angle,         // Angle to move the bullet
                                                         direction,     // Direction of the bullet
                                                         creator,       // Reference to creator of bullet
                                                         sprite,        // Sprite of the bullet
                                                         bulletTotal    // total count of bullets
                                                         )
                                               );

                    // Increment total
                    bulletTotal++;
                } 
                else
                {
                    Instance.activeBullets[setNumber] =
                                              new Bullet(ability,       // Ability of the bullet to shoot
                                                         position,      // Position to spawn the bullet
                                                         angle,         // Angle to move the bullet
                                                         direction,     // Direction of the bullet
                                                         creator,       // Reference to creator of bullet
                                                         sprite,        // Sprite of the bullet
                                                         setNumber      // total count of bullets
                                                         );
                }
            }
        }

        /// <summary>
        /// Runs the updates of all individual bullets, and checks to see 
        /// if they are set to be despawned
        /// </summary>
        /// <param name="gameTime">The total game time variable</param>
        public void Update(GameTime gameTime)
        {
            //Debug.WriteLine($"COUNT: {Instance.activeBullets.Count}");

            //Debug.WriteLine(sprite);
            for (int i = 0; i < Instance.activeBullets.Count; i++)
            {

                if (Instance.activeBullets[i] != null)
                {
                    // Runs the bullet's update.
                    if (Instance.activeBullets[i] != null) {
                        Instance.activeBullets[i].Update(gameTime);
                    }

                    // Checks if bullet is set to be destroyed.
                    if (Instance.activeBullets[i].Destroy)
                    {
                        Instance.activeBullets[i].Collider.Unload();
                        Instance.activeBullets[i].Delete((ushort)i);
                        Instance.activeBullets[i] = null;
                        //Debug.WriteLine($"bogos");
                    }
                }
            }

            // Slot in stored bullets created by other bullets into main list
            for (int i = 0; i < Instance.storeAbilityBullets.Count; i++)
            {
                BulletManager.Instance.CreateBullet(Instance.storeAbilityBullets[i], 
                                                    Instance.storePositionBullets[i], 
                                                    Instance.storeAngleBullets[i],
                                                    Instance.storeDirectionBullets[i],
                                                    Instance.storeCreatorBullets[i], 
                                                    false);
            }

            // Clear all storage lists
            Instance.storeAbilityBullets.Clear();
            Instance.storePositionBullets.Clear();
            Instance.storeAngleBullets.Clear();
            Instance.storeDirectionBullets.Clear();
            Instance.storeCreatorBullets.Clear();
        }

        /// <summary>
        /// Draws all existing bullets stored by the manger, and makes checks for
        /// the angle and direction to do rotation.
        /// </summary>
        public void Draw()
        {
            foreach (Bullet bullet in Instance.activeBullets)
            {
                if (bullet != null)
                {
                    // Convert angle
                    float direction = (float)MathHelper.ToRadians(bullet.Angle);

                    // Get direction
                    if (bullet.Direction < 1)
                    {
                        direction += (float)Math.PI;
                    }

                    GameObject.Instance.SpriteBatch.Draw(
                        bullet.Sprite,                  // The sprite-sheet for the player
                        bullet.Position,                // The position for the player
                        bullet.Transform,               // The scale and bounding box for the animation
                        bullet.Color,                   // The color for the palyer
                        direction,                      // rotation uses the velocity
                        new Vector2(bullet.Transform.Width * 0.5f, bullet.Transform.Height * 0.5f), // Starting render position
                        bullet.Scale,                   // The scale of the sprite
                        SpriteEffects.None,                 // Which direction the sprite faces
                        0.0f                                // Layer depth of the player is 0.0
                    );
                }
            }
        }

        #endregion
    }
}
