using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

// BULLET MANAGER - By Zane Smith
/* Bullets are used by the player and enemies, and are the core of how 
 * the game actually functions. Their data is set here, with a special note
 * that if they would be created during the bullet's update run their data will
 * instead be stored, allowing their creation to be done once the entire update
 * is finished. This allows all new bullets to have their first update run at the
 * same time.*/

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

        #endregion

        #region------------------[ Constructor ]------------------

        /// <summary>
        /// The primary constructor,it loads in the base data for the lists and dictionaries, then
        /// everything from the files is loaded in the Initialize method
        /// </summary>
        private BulletManager ()
        {
            // True bullet storage data
            activeBullets = new List<Bullet>();
            content = new List<ushort>();
            bulletTotal = 0;

            // Storage data for when a bullet spawns a bullet
            storeAbilityBullets = new List<BulletType>(); 
            storePositionBullets = new List<Vector2>();
            storeDirectionBullets = new List<Vector2>();
            storeCreatorBullets = new List<object>();
        }

        #endregion

        #region------------------[ Methods ]------------------

        /// <summary>
        /// Method for creating a bullet, it takes in data and organizes it into a completed bullet which
        /// is placed in the level. If the bullet would be created during the bulet manager's update loop,
        /// it is instead stored separately until the loop ends.
        /// </summary>
        /// <param name="ability">The ability the bullet should have</param>
        /// <param name="position">The position to create the bullet at</param>
        /// <param name="direction">The direction (horizontal and vertical) for the bullet</param>
        /// <param name="creator">Contains a reference to the object that created this
        ///                       bullet. This will determine what types of things the
        ///                       bullet can hit (can be null).</param>
        /// <param name="sourceIsBulet">Was this bullet created during the bullet manager's update /
        ///                             by another bullet. If true the bullet to be created will have
        ///                             its data stored separately so that it can be properly
        ///                             introduced into the bullet list afterwards</param>
        public void CreateBullet (BulletType ability, 
                                  Vector2 position,
                                  Vector2 direction, 
                                  object creator, 
                                  bool sourceIsBullet)
        {

            #region STEP 1: Link Data to GameObject

            // Sets the sprite to use for the bullet for GameObject storage purposes
            ushort sprite;
            switch (ability)
            {
                case BulletType.PlayerNormal:
                    sprite = GameObject.Instance.Content.smallbullet_strip4;
                    AudioManager.Instance.CallSound("Fire");
                    break;

                case BulletType.BigShot:
                    sprite = GameObject.Instance.Content.player_bigshot_strip4;
                    AudioManager.Instance.CallSound("Big Shot");
                    break;

                case BulletType.EnemyNormal:
                    sprite = GameObject.Instance.Content.enemy_red_bullet_strip4;
                    AudioManager.Instance.CallSound("Enemy Fire");
                    break;

                case BulletType.BouncingSide:
                    sprite = GameObject.Instance.Content.enemy_orange_bullet_45_strip4;
                    break;

                case BulletType.BouncingCenter:
                    sprite = GameObject.Instance.Content.enemy_orange_bullet_90_strip4;
                    AudioManager.Instance.CallSound("Scatter");
                    break;

                case BulletType.Splitter:
                    sprite = GameObject.Instance.Content.enemy_green_bullet_main_strip4;
                    AudioManager.Instance.CallSound("Split");
                    break;

                case BulletType.SplitOff:
                    sprite = GameObject.Instance.Content.enemy_green_bullet_split_strip4;
                    AudioManager.Instance.CallSound("Break");
                    break;

                case BulletType.Wave:
                    sprite = GameObject.Instance.Content.enemy_yellow_bullet_strip3;
                    AudioManager.Instance.CallSound("Wave");
                    break;

                case BulletType.Seeker:
                    sprite = GameObject.Instance.Content.enemy_purple_bullet_strip4;
                    AudioManager.Instance.CallSound("Homing");
                    break;

                case BulletType.Shatter:
                    sprite = GameObject.Instance.Content.bullet_purple_core_strip4;
                    AudioManager.Instance.CallSound("Purple Scatter");
                    break;

                case BulletType.ShatterUp:
                    sprite = GameObject.Instance.Content.bullet_purple_60_strip4;
                    AudioManager.Instance.CallSound("Purple Break");
                    break;

                case BulletType.ShatterDown:
                    sprite = GameObject.Instance.Content.bullet_purple_60_strip4;
                    break;

                case BulletType.ShatterSide:
                    sprite = GameObject.Instance.Content.bullet_purple_0_strip4;
                    break;

                case BulletType.Explosion:
                    sprite = GameObject.Instance.Content.bomb_explosion_strip5;
                    break;

                case BulletType.BigExplosion:
                    sprite = GameObject.Instance.Content.big_explode_strip5;
                    break;

                case BulletType.Heart:
                    sprite = GameObject.Instance.Content.bullet_heart_strip4;
                    break;

                case BulletType.BossBouncingSide:
                    sprite = GameObject.Instance.Content.bullet_orange_boss_45_strip4;
                    break;

                case BulletType.BossBouncingCenter:
                    sprite = GameObject.Instance.Content.bullet_orange_boss_90_strip4;
                    AudioManager.Instance.CallSound("Scatter");
                    break;

                case BulletType.BossShatter:
                    sprite = GameObject.Instance.Content.bullet_purple_boss_core_strip4;
                    AudioManager.Instance.CallSound("Purple Scatter");
                    break;

                case BulletType.BossShatterUp:
                    sprite = GameObject.Instance.Content.bullet_purple_boss_60_strip4;
                    AudioManager.Instance.CallSound("Purple Break");
                    break;

                case BulletType.BossShatterDown:
                    sprite = GameObject.Instance.Content.bullet_purple_boss_60_strip4;
                    break;

                case BulletType.BossShatterSide:
                    sprite = GameObject.Instance.Content.bullet_purple_boss_0_strip4;
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

            #endregion

            #region STEP 2: Bullet Storage Handling

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

            #endregion

            #region STEP 3: Generate Bullet Proper

            // Stages of checking:
            // 1: Checks to see if this bullet was created during the bullet update loop.
            //    If it was, its data is stored, rather than having them be placed.
            // 2: Bullet should be created here
            //   A: The bullet is added onto the end of the main list
            //   B: The bullet fills a previously used slot in the main list

            // Add bullet itself to list
            if (sourceIsBullet)
            { // Was created by an bullet, store the data
                Instance.storeAbilityBullets.Add(ability);
                Instance.storePositionBullets.Add(position);
                Instance.storeDirectionBullets.Add(direction);
                Instance.storeCreatorBullets.Add(creator);
            } 
            else
            { // Add bullet itself to list
                if (isReplacing == false)
                { // The bullet is added onto the end of the main list
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
                { // The bullet fills a previously used slot in the main list
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

            #endregion
        }

        #region Normal Monogame methods

        /// <summary>
        /// Runs the updates of all individual bullets, and checks to see 
        /// if they are set to be despawned
        /// </summary>
        /// <param name="gameTime">Used to get the correct pace</param>
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
        /// the directions to do rotation.
        /// </summary>
        public void Draw()
        {
            foreach (Bullet bullet in Instance.activeBullets)
            {
                if (bullet != null)
                {
                    // Flip the visual sprite if needed
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

        #endregion

        /// <summary>
        /// Resets the bullet manager
        /// </summary>
        public void Reset()
        {
            instance = null;
        }

        #endregion
    }
}
