using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// Zane Smith

namespace Galabingus
{
    public sealed class EnemyManager
    {
        #region-------------------[ Fields ]-------------------

        // actual variable attached to singleton calling property
        private static EnemyManager instance = null;

        // Fileplaced enemy positions (used as base)
        private List<int[]> enemyData;

        // List of enemies
        private List<Enemy> activeEnemies;
        private List<ushort> content;

        // Enemy Total
        private ushort totalEnemies;

        // Draw Direction
        private SpriteEffects enemyDirection;

        #endregion

        #region-------------------[ Properties ]-------------------

        /// <summary>
        /// Reference to the Bullet Manager (use BMConstructor method to make a new bullet manager)
        /// </summary>
        public static EnemyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnemyManager();
                }
                return instance;
            }
        }

        #endregion

        #region-------------------[ Constructor ]-------------------

        /// <summary>
        /// 'fake' constructor, since data is loaded into the singleton from Initialize
        /// </summary>
        private EnemyManager()
        {
            // Fake constructor, real data stuff done below in Initialize

            activeEnemies = new List<Enemy>();
            content = new List<ushort>();

            // Get base camera direction data
            enemyDirection = SpriteEffects.None;
        }

        #endregion

        #region-------------------[ Methods ]-------------------

        /// <summary>
        /// Loads in data for where to place enemies, and parses it
        /// </summary>
        /// <param name="enemyData">A list containing enemy data in the form (FORMAT IS PLACEHOLDER SHAWN)
        ///                         [0] -> Is this an enemy? (1 or 0)
        ///                         [1] -> What kind of enemy (checks within bounds
        ///                                of EnemyType Enum
        ///                         [2] -> X Position
        ///                         [3] -> Y Position
        ///                         </param>
        public EnemyManager Initialize(List<int[]> enemyData)
        {

            for (int i = 0; i < enemyData.Count; i++)
            {
                // Setup temp enemy values
                EnemyType tempAbility = EnemyType.Normal;

                //check if slot contains an enemy
                //SHAWN: Delete this if, I did that check for you
                if (enemyData[i][0] == 1)
                {
                    // Found enemy, setup stats
                    tempAbility = (EnemyType)enemyData[i][1];

                    Vector2 enemyPos = new Vector2(enemyData[i][2], enemyData[i][3]);
                    ushort sprite = GameObject.Instance.Content.tile_strip26;

                    switch (tempAbility)
                    {
                        case EnemyType.Normal:
                            sprite = GameObject.Instance.Content.enemy_dblue_strip4;
                            break;

                        case EnemyType.Bouncing:
                            sprite = GameObject.Instance.Content.enemy_orange_strip4;
                            break;

                        case EnemyType.Splitter:
                            sprite = GameObject.Instance.Content.enemy_green_strip4;
                            break;

                        case EnemyType.Circle:
                            sprite = GameObject.Instance.Content.enemy_purple_strip4;
                            break;

                        case EnemyType.Large:
                            sprite = GameObject.Instance.Content.enemy_yellow_strip4;
                            break;

                        case EnemyType.Seeker:
                            sprite = GameObject.Instance.Content.enemy_violet_strip4;
                            break;

                        default:
                            sprite = GameObject.Instance.Content.enemy_lblue_strip4;
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

                    // Add enemy itself to list
                    activeEnemies.Add(new Enemy( tempAbility,   // Ability of the Enemy spawned
                                                 enemyPos,      // Position of Enemy
                                                 sprite,        // Sprite for Enemy
                                                 totalEnemies   // Total enemies
                                                )
                                      );

                    // Increment count
                    totalEnemies++;
                }
            }

            return instance;
        }

        public void Update (GameTime gameTime)
        {
            // Get camera's movement direction
            float cameraScroll = Camera.Instance.CameraScroll;
            if (cameraScroll < 0)
            {
                enemyDirection = SpriteEffects.None;
            } 
            else
            {
                enemyDirection = SpriteEffects.FlipHorizontally;
            }

            // Run enemy updates
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                // Runs the bullet's update.
                activeEnemies[i].Update(gameTime);

                // Checks if enemy is set to be destroyed.
                if (activeEnemies[i].Destroy)
                {
                    activeEnemies.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        public void Draw ()
        {
            foreach (Enemy enemy in activeEnemies)
            {
                GameObject.Instance.SpriteBatch.Draw(
                    enemy.Sprite,                   // The sprite-sheet for the player
                    enemy.Position,                 // The position for the player
                    enemy.Transform,                // The scale and bounding box for the animation
                    Color.White,                        // The color for the palyer (RED IS TEMP UNTIL WE GET ENEMY SPRITES IN)
                    0.0f,                               // There cannot be any rotation of the player
                    Vector2.Zero,                       // Starting render position
                    enemy.Scale,                    // The scale of the sprite
                    enemyDirection,                 // Which direction the sprite faces
                    0.0f                                // Layer depth of the player is 0.0
                );
            }
        }

        #endregion

    }
}
