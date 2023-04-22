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
        private List<Vector2> storeDirectionBullets;
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
            storeDirectionBullets = new List<Vector2>();
            storeCreatorBullets = new List<object>();

            bulletTotal = 0;

            content = new List<ushort>();

            // Gets size of screen
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
                                  Vector2 direction, 
                                  object creator, 
                                  bool sourceIsBullet)
        {
            AudioManager.Instance.CallSound("Fire");

            // Sets the sprite to use for the bullet for GameObject storage purposes
            ushort sprite;
            switch (ability)
            {
                case BulletType.PlayerNormal:
                    sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.EnemyNormal:
                    sprite = GameObject.Instance.Content.enemy_red_bullet_strip4;
                    break;

                case BulletType.BouncingSide:
                    sprite = GameObject.Instance.Content.enemy_orange_bullet_45_strip4;
                    break;

                case BulletType.BouncingCenter:
                    sprite = GameObject.Instance.Content.enemy_orange_bullet_90_strip4;
                    break;

                case BulletType.Splitter:
                    sprite = GameObject.Instance.Content.enemy_green_bullet_main_strip4;
                    break;

                case BulletType.SplitOff:
                    sprite = GameObject.Instance.Content.enemy_green_bullet_split_strip4;
                    break;

                case BulletType.Wave:
                    sprite = GameObject.Instance.Content.enemy_yellow_bullet_strip3;
                    break;

                case BulletType.Seeker:
                    sprite = GameObject.Instance.Content.enemy_violet_bullet_strip4;
                    break;

                case BulletType.Explosion:
                    sprite = GameObject.Instance.Content.bomb_explosion_strip5;
                    break;

                case BulletType.BigExplosion:
                    sprite = GameObject.Instance.Content.big_explode_strip5;
                    break;

                case BulletType.Heart:
                    sprite = GameObject.Instance.Content.heartbullet_strip4;
                    break;

                default:
                    sprite = GameObject.Instance.Content.smallbullet_strip4;
                    break;
            }

            // Add sprite linker to list
            if (Instance.content.Count == 0)
            {
                Instance.content.Add(sprite);
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

            for (int i = 0; i < Instance.activeBullets.Count; i++)
            {
                if (Instance.activeBullets[i] == null)
                {
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
                Instance.storeDirectionBullets.Add(direction);
                Instance.storeCreatorBullets.Add(creator);
            } 
            else
            {
                if (isReplacing == false)
                {
                    Instance.activeBullets.Add(new Bullet(ability,      // Ability of the bullet to shoot
                                                         position,      // Position to spawn the bullet
                                                         direction,     // Direction of the bullet
                                                         creator,       // Reference to creator of bullet
                                                         sprite,        // Ushort for Sprite (GameObject)
                                                         bulletTotal    // Total count of bullets
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
                                                         direction,     // Direction of the bullet
                                                         creator,       // Reference to creator of bullet
                                                         sprite,        // Ushort for Sprite (GameObject)
                                                         setNumber      // Total count of bullets
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
                    }
                }
            }

            // Slot in stored bullets created by other bullets into main list
            for (int i = 0; i < Instance.storeAbilityBullets.Count; i++)
            {
                BulletManager.Instance.CreateBullet(Instance.storeAbilityBullets[i], 
                                                    Instance.storePositionBullets[i], 
                                                    Instance.storeDirectionBullets[i],
                                                    Instance.storeCreatorBullets[i], 
                                                    false);
            }

            // Clear all storage lists
            Instance.storeAbilityBullets.Clear();
            Instance.storePositionBullets.Clear();
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
                    SpriteEffects flipping = SpriteEffects.None;

                    if (bullet.Direction.X == -1)
                    { // Flip Horizontally
                        flipping = SpriteEffects.FlipHorizontally;
                    }

                    if (bullet.Direction.Y == 1)
                    { // Flip Vertically
                        flipping = flipping | SpriteEffects.FlipVertically;
                    }

                    GameObject.Instance.SpriteBatch.Draw(
                        bullet.Sprite,                  // The sprite-sheet for the player
                        bullet.Position,                // The position for the player
                        bullet.Transform,               // The scale and bounding box for the animation
                        Color.White,                    // The color for the palyer
                        0,                              // rotation uses the velocity
                        Vector2.Zero,                   // Starting render position
                        bullet.Scale,                   // The scale of the sprite
                        flipping,                       // Which direction the sprite faces
                        0.0f                            // Layer depth of the player is 0.0
                    );
                }
            }
        }

        public void Reset()
        {
            instance = null;
        }

        #endregion
    }
}
