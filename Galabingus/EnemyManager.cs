﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
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

        // List existing of enemies
        private List<Enemy> activeEnemies;
        private List<ushort> content;

        // Enemy Created Enemy Storage
        private List<EnemyType> storeAbilityEnemies;
        private List<Vector2> storePositionEnemies;
        private List<object> storeCreatorEnemies;

        // Enemy Total
        private ushort enemyTotal;

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

        /// <summary>
        /// Returns the direction enemies should be facing
        /// </summary>
        public SpriteEffects EnemyDirection
        {
            get { 
                return enemyDirection; 
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

            enemyTotal = 0;

            storeAbilityEnemies = new List<EnemyType>();
            storePositionEnemies = new List<Vector2>();
            storeCreatorEnemies = new List<object>();

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
            EnemyType tempAbility = EnemyType.Normal;

            for (int i = 0; i < enemyData.Count; i++)
            {

                //check if slot contains an enemy
                if (enemyData[i][0] == 1)
                { // Found enemy, set up it stats

                    // Set its ability
                    tempAbility = (EnemyType)enemyData[i][1];

                    // Set its position
                    Vector2 enemyPos = new Vector2(enemyData[i][2], enemyData[i][3]);

                    // Create actual enemy
                    CreateEnemy(tempAbility,// Ability
                                enemyPos,   // Position
                                null,       // Creator
                                false       // Was this Enemy spawned by another enemy?
                                );
                }
            }

            return instance;
        }

        public void CreateEnemy (EnemyType ability, Vector2 position, object creator, bool isSourceEnemy) 
        {
            // account for if creator is null

            ushort sprite = GameObject.Instance.Content.tile_strip26;

            switch (ability)
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

            bool isReplacing = false;
            ushort setNumber = (ushort)Math.Max(0, (Instance.activeEnemies.Count - 1));

            Debug.WriteLine($"COUNT: {activeEnemies.Count}");

            for (int i = 0; i < Instance.activeEnemies.Count; i++)
            {
                if (Instance.activeEnemies[i] == null)
                {
                    Debug.WriteLine($"Found Reusable Val at: {i}");
                    setNumber = (ushort)(i);
                    isReplacing = true;
                    break;
                }
            }

            // Add bullet itself to list
            if (isSourceEnemy)
            { // Was created by an enemy, store the data
                Instance.storeAbilityEnemies.Add(ability);
                Instance.storePositionEnemies.Add(position);
                Instance.storeCreatorEnemies.Add(creator);
            }
            else
            { // Add enemy itself to list
                if (isReplacing == false)
                {
                    Instance.activeEnemies.Add(new Enemy(ability,    // Ability of the Enemy spawned
                                                         position,   // Position of Enemy
                                                         creator,    // What created this enemy
                                                         sprite,     // Sprite for Enemy
                                                         enemyTotal  // Total enemies
                                                         )
                                               );

                    // Increment total
                    enemyTotal++;
                }
                else
                {
                    Instance.activeEnemies[setNumber] = new Enemy(ability,    // Ability of the Enemy spawned
                                                                  position,   // Position of Enemy
                                                                  creator,    // What created this enemy
                                                                  sprite,     // Sprite for Enemy
                                                                  enemyTotal  // Total enemies
                                                                  );
                }
            }
        }

        /// <summary>
        /// Runs the update methods for all active enemies, and manages deletion
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update (GameTime gameTime)
        {
            // Get camera's movement direction
            float cameraScroll = Camera.Instance.CameraScroll;
            if (cameraScroll < 0)
            {
                Instance.enemyDirection = SpriteEffects.None;
            } 
            else
            {
                Instance.enemyDirection = SpriteEffects.FlipHorizontally;
            }

            // Run enemy updates
            for (int i = 0; i < Instance.activeEnemies.Count; i++)
            {
                if (Instance.activeEnemies[i] != null)
                { // Checks if the enemy slot is null
                    // Runs the bullet's update.
                    Instance.activeEnemies[i].Update(gameTime);

                    // Checks if enemy is set to be destroyed.
                    if (Instance.activeEnemies[i].Destroy)
                    {
                        Instance.activeEnemies[i].Delete((ushort)i);
                        Instance.activeEnemies[i] = null;

                        // NOTE: If an enmy should have an on death action put it here
                    }
                }
            }

            // Slot in stored bullets created by other bullets into main list
            for (int i = 0; i < Instance.storeAbilityEnemies.Count; i++)
            {// Create all the stored enemies
                Instance.CreateEnemy(Instance.storeAbilityEnemies[i],    // Ability of the Enemy spawned
                                     Instance.storePositionEnemies[i],   // Position of Enemy
                                     Instance.storeCreatorEnemies[i],    // Enemy's creator
                                     false
                                     );
            }

            // Clear all storage lists
            Instance.storeAbilityEnemies.Clear();
            Instance.storePositionEnemies.Clear();
            Instance.storeCreatorEnemies.Clear();
        }

        /// <summary>
        /// Draws all active enemies
        /// </summary>
        public void Draw ()
        {
            foreach (Enemy enemy in Instance.activeEnemies)
            {
                if (enemy != null)
                {
                    GameObject.Instance.SpriteBatch.Draw(
                        enemy.Sprite,                   // The sprite-sheet for the player
                        enemy.Position,                 // The position for the player
                        enemy.Transform,                // The scale and bounding box for the animation
                        Color.White,                    // The color for the palyer (RED IS TEMP UNTIL WE GET ENEMY SPRITES IN)
                        0.0f,                           // There cannot be any rotation of the player
                        Vector2.Zero,                   // Starting render position
                        enemy.Scale,                    // The scale of the sprite
                        Instance.enemyDirection,        // Which direction the sprite faces
                        0.0f                            // Layer depth of the player is 0.0
                    );
                }
                    
            }
        }

        #endregion

    }
}
