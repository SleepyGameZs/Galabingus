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
    /// <summary>
    /// Enumeration for all available normal enemy types
    /// </summary>
    public enum EnemyType
    {
        Normal,
        Bouncing,
        Splitter,
        Circle
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

            GameObject.Instance.Content = GameObject.Instance.Content.tile_strip26;

            // Set Animation
            //this.Sprite = 

            // Set Scale
            this.Scale = 2.5f;

            // Set bullet state & timer
            this.ET_ability = ET_ability;
            int_stateTimer = 0;

            // Set Position
            this.Position = new Vector2(vc2_position.X, vc2_position.Y);

            // Set velocity to zero at start
            vc2_velocity = Vector2.Zero;
        }

        #endregion

        #region-------------------[ Methods ]-------------------

        public void Update (GameTime gameTime)
        {
            // Check if off screen
            bool bol_bulletOffScreen = this.Position.X < 0 &&
                                       this.Position.X > BulletManager.Instance.ScreenDimensions.X;
            if (bol_bulletOffScreen)
            {
                // Will only perform actions if currently on the screen

            }

            // Manage Animation
            this.Animation.AnimationDuration = 0.03f;
            this.Transform = this.Animation.Play(gameTime);
        }

        #endregion
    }
}
