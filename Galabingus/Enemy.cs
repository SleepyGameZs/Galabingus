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
        private bool bol_destroy;

        // Movement data
        private Vector2 vc2_velocity;

        // State data
        private EnemyType ET_ability;
        private int int_stateTimer;

        // Name used to find values from GameObject dynamic
        private ushort ush_contentName;

        // Number into game object index to look for items
        private ushort ush_enemyNumber;

        // Time between each shot
        private int int_shotTimer;

        // Randomizer for time between shots
        private Random rng;
        private int int_shotWaitTime;
        private int int_shotWaitVariance;

        #endregion

        #region-------------------[ Properties ]-------------------

        /// <summary>
        /// Used to see if this bullet should be destroyed now
        /// </summary>
        public bool Destroy
        {
            get { return bol_destroy; }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort ush_contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored position value
        /// </summary>
        public Vector2 Position
        {
            get
            {
                GameObject.Instance.Content = ush_contentName;
                return GetPosition(ush_enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetPosition(ush_enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort ush_contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored sprite for this bullet
        /// </summary>
        public Texture2D Sprite
        {
            get
            {
                GameObject.Instance.Content = ush_contentName;
                return GetSprite(ush_enemyNumber);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort ush_contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's transform off total spritesheet (most likely
        /// relates to which frame of the animation is to be shown)
        /// </summary>
        public Rectangle Transform
        {
            get
            {
                GameObject.Instance.Content = ush_contentName;
                return GetTransform(ush_enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetTransform(ush_enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort ush_contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's sprite scale, so that it can be easily resized
        /// </summary>
        public float Scale
        {
            get
            {
                GameObject.Instance.Content = ush_contentName;
                return GetScale(ush_enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetScale(ush_enemyNumber, value);
            }
        }

        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = ush_contentName;
                return GetAnimation(ush_enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetAnimation(ush_enemyNumber, value);
            }
        }

        /// <summary>
        /// Accesses dynamic singleton GameObject class, using ushort ush_contentName to find
        /// specific type of thing to access, an bulletNumber as the index inside that list of bullets
        /// This allows one to access this bullet's stored collider for collision checking
        /// </summary>
        public Collider Collider
        {
            get
            {
                GameObject.Instance.Content = ush_contentName;
                return GetCollider(ush_enemyNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetCollider(ush_enemyNumber, value);
            }
        }

        #endregion

        #region-------------------[ Constructor ]-------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BT_ability">Sets which type of bullet is to be shot for
        ///                          bullet finite state machine.</param>
        /// <param name="vc2_position">The starting position of the bullet</param>
        /// <param name="int_direction">The initial direction of the bullet</param>
        /// <param name="ush_contentName">Name to use for GameObject storage</param>
        /// <param name="scale">scale of bullet object</param>
        /// <param name="bulletNumber">Number to give bullet in GameObject list</param>
        public Enemy (
            EnemyType ET_ability,
            Vector2 vc2_position,
            ushort ush_contentName,
            ushort ush_enemyNumber
        ) : base(ush_contentName, ush_enemyNumber)
        {
            // Set Sprite from given
            this.ush_contentName = ush_contentName;
            this.ush_enemyNumber = ush_enemyNumber;
            Animation.AnimationDuration = 0.01f;

            //GameObject.Instance.Content = GameObject.Instance.Content.tile_strip26;

            // Set Scale
            this.Scale = 3f;

            // Set bullet state & timer
            this.ET_ability = ET_ability;
            int_stateTimer = 0;

            // Set Position
            this.Position = new Vector2(vc2_position.X, vc2_position.Y);

            // Set velocity to zero at start
            vc2_velocity = Vector2.Zero;

            // set randomizer + extra time between next shot
            rng = new Random();
            int_shotWaitVariance = 8;
            int_shotWaitTime = rng.Next(int_shotWaitVariance) - int_shotWaitVariance / 2;

            // Set shot timer with some randomization
            int_shotTimer = rng.Next(50);
        }

        #endregion

        #region-------------------[ Methods ]-------------------

        public void Update (GameTime gameTime)
        {
            // Check if off screen
            bool bol_bulletOnScreen = !(this.Position.X < 0 &&
                                       this.Position.X > BulletManager.Instance.ScreenDimensions.X);
            //Debug.WriteLine("e");
            if (bol_bulletOnScreen)
            {

                // Check which direction the enemy is facing
                int int_spriteDirection;
                if (Camera.Instance.CameraScroll < 0)
                {
                    int_spriteDirection = 1;
                }
                else
                {
                    int_spriteDirection = -1;
                }

                // Get shooting position
                float flt_enemyShootX = (Transform.Width * this.Scale) / 2;
                float flt_enemyShootY = (Transform.Height * this.Scale) / 2;

                Vector2 vc2_shootPos = new Vector2(Position.X               // Base player X position
                                                   + flt_enemyShootX,
                                                       Position.Y               // Base player Y position
                                                       + flt_enemyShootY        // Center vertically
                                                       );

                // Will only perform actions if currently on the screen
                switch (this.ET_ability)
                {
                    case EnemyType.Bouncing:
                        // Shooting Delay
                        if (int_shotTimer > (int)(140 * (1 + (0.1 * int_shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (int_spriteDirection < 0)
                            {
                                vc2_shootPos = new Vector2(Position.X,
                                                           Position.Y + flt_enemyShootY * 2 - 20);
                            }

                            // Shoot the 3 bullets
                            BulletManager.Instance.CreateBullet(BulletType.Bouncing, vc2_shootPos, 0, int_spriteDirection);
                            BulletManager.Instance.CreateBullet(BulletType.Bouncing, vc2_shootPos, 30, int_spriteDirection);
                            BulletManager.Instance.CreateBullet(BulletType.Bouncing, vc2_shootPos, -30, int_spriteDirection);

                            // Reset Shooting time
                            int_shotWaitTime = rng.Next(int_shotWaitVariance) - int_shotWaitVariance / 2;
                            int_shotTimer = 0;
                        }
                        break;

                    case EnemyType.Splitter:
                        // Shooting Delay
                        if (int_shotTimer > (int)(140 * (1 + (0.1 * int_shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (int_spriteDirection < 0)
                            {
                                vc2_shootPos = new Vector2(Position.X,
                                                           Position.Y + flt_enemyShootY * 2 - 10);
                            }

                            // Shoot the splitter bullet
                            BulletManager.Instance.CreateBullet(BulletType.Splitter, vc2_shootPos, 0, int_spriteDirection);

                            // Reset Shooting time
                            int_shotWaitTime = rng.Next(int_shotWaitVariance) - int_shotWaitVariance / 2;
                            int_shotTimer = 0;
                        }
                        break;

                    case EnemyType.Large:
                        // Shooting Delay
                        if (int_shotTimer > (int)(120 * (1 + (0.1 * int_shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (int_spriteDirection < 0)
                            {
                                vc2_shootPos = new Vector2(Position.X,
                                                           Position.Y + flt_enemyShootY * 2);
                            }

                            // Shoot the BIG BULLET
                            BulletManager.Instance.CreateBullet(BulletType.Large, vc2_shootPos, 0, int_spriteDirection);

                            // Reset Shooting time
                            int_shotWaitTime = rng.Next(int_shotWaitVariance);
                            int_shotTimer = 0;
                        }
                        break;

                    case EnemyType.Seeker:
                        // Shooting Delay
                        if (int_shotTimer > (int)(190 * (1 + (0.1 * int_shotWaitTime))))
                        {
                            // Fix rotation errors when flipped
                            if (int_spriteDirection < 0)
                            {
                                vc2_shootPos = new Vector2(Position.X,
                                                           Position.Y + flt_enemyShootY * 2 - 10);
                            }

                            // Shoot the seeker bullet
                            BulletManager.Instance.CreateBullet(BulletType.Seeker, vc2_shootPos, 0, int_spriteDirection);

                            // Reset Shooting time
                            int_shotWaitTime = rng.Next(int_shotWaitVariance) - int_shotWaitVariance / 2;
                            int_shotTimer = 0;
                        }
                        break;
                }
                int_shotTimer++;


            }

            // Manage Animation
            this.Animation.AnimationDuration = 0.03f;
            this.Transform = this.Animation.Play(gameTime);
        }

        #endregion
    }
}
