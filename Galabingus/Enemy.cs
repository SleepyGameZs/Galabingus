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
    /// <summary>
    /// Enumeration for all available normal enemy types
    /// </summary>
    public enum EnemyType
    {
        Normal,
        Bouncing,
        Splitter,
        Circle,
        Large,
        Seeker
    }

    internal class Enemy : GameObject
    {
        #region-------------------[ Fields ]-------------------
        // Is this enemy ready to be destroyed?
        private bool destroy;

        // Movement data
        private Vector2 velocity;

        // State data
        private EnemyType ability;
        private int stateTimer;

        // Name used to find values from GameObject dynamic
        private ushort contentName;

        // Number into game object index to look for items
        private ushort enemyNumber;

        // Time between each shot
        private int shotTimer;

        // Randomizer for time between shots
        private Random rng;
        private int shotWaitTime;
        private int shotWaitVariance;

        // Health value of this enemy
        private int currentHealth;
        private int totalHealth;

        // Reference to what thing created this enemy (can be null)
        private object creatorReference;

        #endregion

        #region-------------------[ Properties ]-------------------

        /// <summary>
        /// Used to see if this bullet should be destroyed now
        /// </summary>
        public bool Destroy
        {
            get { return destroy; }
            set { destroy = value; }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored position value
        /// </summary>
        public Vector2 Position
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetPosition(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetPosition(enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored sprite for this bullet
        /// </summary>
        public Texture2D Sprite
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetSprite(enemyNumber);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's transform off total spritesheet (most likely
        /// relates to which frame of the animation is to be shown)
        /// </summary>
        public Rectangle Transform
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetTransform(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetTransform(enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's sprite scale, so that it can be easily resized
        /// </summary>
        public float Scale
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetScale(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetScale(enemyNumber, value);
            }
        }

        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetAnimation(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetAnimation(enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored collider for collision checking
        /// </summary>
        public Collider Collider
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetCollider(enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetCollider(enemyNumber, value);
            }
        }

        /// <summary>
        /// Allows setting and returning of this enemy's health
        /// </summary>
        public int Health
        {
            get
            {
                return currentHealth;
            }
            set { 
                currentHealth = value; 
            }
        }

        /// <summary>
        /// The object which created this bullet.
        /// </summary>
        public object Creator
        {
            get
            {
                return creatorReference;
            }
        }

        #endregion

        #region-------------------[ Constructor ]-------------------

        /// <summary>
        /// Spawns an enemy with given stats
        /// </summary>
        /// <param name="ability">The ability of the enemy</param>
        /// <param name="position">The position of the enemy</param>
        /// <param name="position">The thing that created this enemy (may be null)</param>
        /// <param name="contentName">Name to use for GameObject storage</param>
        /// <param name="enemyNumber">Number to give bullet in GameObject list</param>
        public Enemy (
            EnemyType ability,
            Vector2 position,
            object creator,
            ushort contentName,
            ushort enemyNumber
        ) : base(contentName, enemyNumber, CollisionGroup.Enemy)
        {
            this.thisGameObject = this;
            // Set Sprite from given
            this.contentName = contentName;
            this.enemyNumber = enemyNumber;
            Animation.AnimationDuration = 0.01f;

            // Set creator
            creatorReference = creator;

            // Set Scale
            this.Scale = Player.PlayerInstance.Scale;

            // Set bullet state & timer
            this.ability = ability;
            stateTimer = 0;

            // Set Position
            this.Position = new Vector2(position.X, position.Y);

            // Set velocity to zero at start
            velocity = Vector2.Zero;

            // set randomizer + extra time between next shot
            rng = new Random();
            shotWaitVariance = 8;
            shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;

            // Set shot timer with some randomization
            shotTimer = rng.Next(50);

            // Set Health
            totalHealth = 3;
            currentHealth = totalHealth;
        }

        #endregion

        #region-------------------[ Methods ]-------------------

        public void Update (GameTime gameTime)
        {
            // Check if off screen
            bool bulletOnScreen = !(this.Position.X < 0 &&
                                       this.Position.X > BulletManager.Instance.ScreenDimensions.X);
            //Debug.WriteLine("e");
            if (bulletOnScreen)
            {

                // Check which direction the enemy is facing
                int spriteDirection;
                if (Camera.Instance.CameraScroll < 0)
                {
                    spriteDirection = 1;
                }
                else
                {
                    spriteDirection = -1;
                }

                // Get shooting position
                float enemyShootX = (Transform.Width * this.Scale) / 2;
                float enemyShootY = (Transform.Height * this.Scale) / 2;

                Vector2 shootPos = new Vector2(Position.X + // Base player X position
                                               enemyShootX, // Center horizontally
                                               Position.Y + // Base player Y position
                                               enemyShootY  // Center vertically
                                               );

                // Will only perform actions if currently on the screen
                switch (this.ability)
                {
                    case EnemyType.Bouncing:
                        // Shooting Delay
                        if (shotTimer > (int)(140 * (1 + (0.1 * shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (spriteDirection < 0)
                            {
                                shootPos = new Vector2(Position.X,
                                                       Position.Y + enemyShootY * 2 - 20);
                            }

                            // Shoot the 3 bullets
                            BulletManager.Instance.CreateBullet(BulletType.Bouncing, shootPos, 0, spriteDirection, this, false);
                            BulletManager.Instance.CreateBullet(BulletType.Bouncing, shootPos, 30, spriteDirection, this, false);
                            BulletManager.Instance.CreateBullet(BulletType.Bouncing, shootPos, -30, spriteDirection, this, false);

                            // Reset Shooting time
                            shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;
                            shotTimer = 0;
                        }
                        break;

                    case EnemyType.Splitter:
                        // Shooting Delay
                        if (shotTimer > (int)(140 * (1 + (0.1 * shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (spriteDirection < 0)
                            {
                                shootPos = new Vector2(Position.X,
                                                       Position.Y + enemyShootY * 2 - 15);
                            }

                            // Shoot the splitter bullet
                            BulletManager.Instance.CreateBullet(BulletType.Splitter, shootPos, 0, spriteDirection, this, false);

                            // Reset Shooting time
                            shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;
                            shotTimer = 0;
                        }
                        break;

                    case EnemyType.Large:
                        // Shooting Delay
                        if (shotTimer > (int)(120 * (1 + (0.1 * shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (spriteDirection < 0)
                            {
                                shootPos = new Vector2(Position.X,
                                                       Position.Y + enemyShootY * 2 - 15);
                            }

                            // Shoot the BIG BULLET
                            BulletManager.Instance.CreateBullet(BulletType.Large, shootPos, 0, spriteDirection, this, false);

                            // Reset Shooting time
                            shotWaitTime = rng.Next(shotWaitVariance);
                            shotTimer = 0;
                        }
                        break;

                    case EnemyType.Seeker:
                        // Shooting Delay
                        if (shotTimer > (int)(190 * (1 + (0.1 * shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (spriteDirection < 0)
                            {
                                shootPos = new Vector2(Position.X,
                                                       Position.Y + enemyShootY * 2 - 10);
                            }

                            // Shoot the seeker bullet
                            BulletManager.Instance.CreateBullet(BulletType.Seeker, shootPos, 0, spriteDirection, this, false);

                            // Reset Shooting time
                            shotWaitTime = rng.Next(shotWaitVariance) - shotWaitVariance / 2;
                            shotTimer = 0;
                        }
                        break;
                }
                shotTimer++;

                // Creates currect collider for Enemy
                this.Transform = this.Animation.Play(gameTime);

                List<Collision> intercepts = this.Collider.UpdateTransform(
                    (ushort)CollisionGroup.Enemy,        // Content on same collision layer won't coll
                    enemyNumber
                );

                this.Collider.Resolved = true;

            }

            // Set position of enemy
            Position -= Camera.Instance.OffSet;

            // Manage Animation
            this.Animation.AnimationDuration = 0.03f;
            this.Transform = this.Animation.Play(gameTime);
        }

        #endregion
    }
}
