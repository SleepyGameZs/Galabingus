using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Galabingus
{
    public sealed class EnemyManager
    {
        #region-------------------[ Fields ]-------------------

        // actual variable attached to singleton calling property
        private static EnemyManager s_instance = null;

        // Fileplaced enemy positions (used as base)
        private List<int[]> l_obj_enemyData;

        // List of enemies
        private List<Enemy> l_obj_activeEnemies;
        private List<ushort> l_ush_content;

        // Enemy Total
        private ushort ush_totalEnemies;

        #endregion

        #region-------------------[ Properties ]-------------------

        /// <summary>
        /// Reference to the Bullet Manager (use BMConstructor method to make a new bullet manager)
        /// </summary>
        public static EnemyManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new EnemyManager();
                }
                return s_instance;
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

            l_obj_activeEnemies = new List<Enemy>();
            l_ush_content = new List<ushort>();
        }

        #endregion

        #region-------------------[ Methods ]-------------------

        /// <summary>
        /// Loads in data for where to place enemies, and parses it
        /// </summary>
        /// <param name="l_obj_enemyData">A list containing enemy data in the form (FORMAT IS PLACEHOLDER SHAWN)
        ///                               [0] -> Is this an enemy? (1 or 0)
        ///                               [1] -> What kind of enemy (checks within bounds
        ///                                      of EnemyType Enum
        ///                               [2] -> X Position
        ///                               [3] -> Y Position
        ///                               </param>
        public EnemyManager Initialize(List<int[]> l_obj_enemyData)
        {

            for (int i = 0; i < l_obj_enemyData.Count; i++)
            {
                // Setup temp enemy values
                EnemyType ET_tempAbility = EnemyType.Normal;

                //check if slot contains an enemy
                //SHAWN: Delete this if, I did that check for you
                if (l_obj_enemyData[i][0] == 1)
                {
                    // Found enemy, setup stats
                    ET_tempAbility = (EnemyType)l_obj_enemyData[i][1];

                    Vector2 vc2_enemyPos = new Vector2(l_obj_enemyData[i][2], l_obj_enemyData[i][3]);
                    ushort ush_sprite = GameObject.Instance.Content.tile_strip26;

                    switch (ET_tempAbility)
                    {
                        case EnemyType.Normal:
                            ush_sprite = GameObject.Instance.Content.tile_strip26;
                            break;

                        case EnemyType.Bouncing:
                            ush_sprite = GameObject.Instance.Content.tile_strip26;
                            break;

                        case EnemyType.Splitter:
                            ush_sprite = GameObject.Instance.Content.tile_strip26;
                            break;

                        case EnemyType.Circle:
                            ush_sprite = GameObject.Instance.Content.tile_strip26;
                            break;

                        default:
                            ush_sprite = GameObject.Instance.Content.tile_strip26;
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

                    // Add enemy itself to list
                    l_obj_activeEnemies.Add(new Enemy( ET_tempAbility, vc2_enemyPos, ush_sprite, ush_totalEnemies));

                    // Increment count
                    ush_totalEnemies++;
                }
            }

            return s_instance;
        }

        public void Update (GameTime gameTime)
        {
            for (int i = 0; i < l_obj_activeEnemies.Count; i++)
            {
                // Runs the bullet's update.
                l_obj_activeEnemies[i].Update(gameTime);

                // Checks if enemy is set to be destroyed.
                if (l_obj_activeEnemies[i].Destroy)
                {
                    l_obj_activeEnemies.RemoveAt(i);
                    i -= 1;
                }
            }
        }

        public void Draw ()
        {
            foreach (Enemy obj_enemy in l_obj_activeEnemies)
            {
                GameObject.Instance.SpriteBatch.Draw(
                    obj_enemy.Sprite,                   // The sprite-sheet for the player
                    obj_enemy.Position,                 // The position for the player
                    obj_enemy.Transform,                // The scale and bounding box for the animation
                    Color.Red,                          // The color for the palyer (RED IS TEMP UNTIL WE GET ENEMY SPRITES IN)
                    0.0f,                               // There cannot be any rotation of the player
                    Vector2.Zero,                       // Starting render position
                    obj_enemy.Scale,                    // The scale of the sprite
                    SpriteEffects.None,                 // Which direction the sprite faces
                    0.0f                                // Layer depth of the player is 0.0
                );
            }
        }

        #endregion

    }
}
