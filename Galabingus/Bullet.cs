using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Galabingus
{
    /// <summary>
    /// Enumeration for all available bullet types to shoot
    /// </summary>
    public enum BulletType
    {
        Normal,
        Bouncing,
        Splitter,
        SplitSmall,
        Circle
    }

    internal class Bullet : GameObject
    {

        #region-------------------[ Fields ]-------------------

        // Is this bullet ready to be destroyed?
        private bool bol_destroy;

        // Movement data
        private double dbl_direction;
        private Vector2 vc2_velocity;

        // State data
        private BulletType BT_ability;
        private int int_stateTimer;

        // Animation Data
        private Color clr_bulletColor;

        // Name used to find values from GameObject dynamic
        private ushort ush_contentName;

        // Number into game object index to look for items
        private ushort ush_bulletNumber;

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
                return GetPosition(ush_bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetPosition(ush_bulletNumber, value);
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
                return GetSprite(ush_bulletNumber);
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
                return GetTransform(ush_bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetTransform(ush_bulletNumber, value);
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
                return GetScale(ush_bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetScale(ush_bulletNumber, value);
            }
        }

        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = ush_contentName;
                return GetAnimation(ush_bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetAnimation(ush_bulletNumber, value);
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
                return GetCollider(ush_bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = ush_contentName;
                SetCollider(ush_bulletNumber, value);
            }
        }

        public Color Color
        {
            get
            {
                return clr_bulletColor;
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
        public Bullet (
            BulletType BT_ability,
            Vector2 vc2_position,
            int int_direction,
            ushort ush_contentName,
            ushort ush_bulletNumber
        ) : base(ush_contentName, ush_bulletNumber)
        {
            // Set Sprite from given
            this.ush_contentName = ush_contentName;
            this.ush_bulletNumber = ush_bulletNumber;

            GameObject.Instance.Content = GameObject.Instance.Content.bigbullet_strip4;

            // Set bullet state & timer
            this.BT_ability = BT_ability;
            int_stateTimer = 0;

            // Set Position
            this.Scale = 3f;
            this.Position = new Vector2(vc2_position.X + Transform.Width * Scale / 2.0f, vc2_position.Y - Transform.Height * Scale / 2.0f);

            // Convert to radians
            dbl_direction = int_direction * (Math.PI / 180);

            // Set values for vector lengths
            float flt_horizontalVal = (float)Math.Cos(dbl_direction);
            float flt_verticalVal = (float)Math.Sin(dbl_direction);
            vc2_velocity = Vector2.Normalize(new Vector2(flt_horizontalVal, flt_verticalVal));

            switch (BT_ability)
            {
                case BulletType.Normal:
                    clr_bulletColor = Color.CornflowerBlue;
                    break;

                case BulletType.Bouncing:
                    clr_bulletColor = Color.Orange;
                    break;

                case BulletType.Splitter:
                    clr_bulletColor = Color.Green;
                    break;

                case BulletType.SplitSmall:
                    clr_bulletColor = Color.Green;
                    break;

                case BulletType.Circle:
                    clr_bulletColor = Color.Purple;
                    break;

                default:
                    clr_bulletColor = Color.White;
                    break;
            }

            // Set sprite manually at position
            //GameObject.Instance.Content = ::file name::
            //GameObject.Instance.Sprite;

            // Set constructor easier access
            // ush_contentName = ::file name::;

            // value to use if established in constructor
            // this.Sprite <- property


            // how to do collisions
            // Update.
        }

        #endregion

        #region-------------------[ Methods ]-------------------

        public void Update(GameTime gameTime)
        {
            // Apply position change
            int int_speedmulti = 9;
            switch (BT_ability)
            {
                case BulletType.Normal:
                    this.Position += vc2_velocity * int_speedmulti * 2;
                    break;

                case BulletType.Bouncing:
                    this.Position += vc2_velocity * int_speedmulti;
                    break;

                case BulletType.Splitter:
                    this.Position += vc2_velocity * int_speedmulti * 3;
                    break;

                case BulletType.SplitSmall:
                    this.Position += vc2_velocity * int_speedmulti * 2;
                    break;

                case BulletType.Circle:
                    this.Position += vc2_velocity * int_speedmulti;
                    break;

                default:
                    // Doesn't move lol
                    break;
            }

            // Manage Animation
            this.Animation.AnimationDuration = 0.03f;
            this.Transform = this.Animation.Play(gameTime);

            // Check if off screen
            bool bol_bulletOffScreen = this.Position.X < 0 &&
                                       this.Position.X > BulletManager.Instance.ScreenDimensions.X;
            if (bol_bulletOffScreen)
            {
                bol_destroy = true;
            }

        }

        #endregion

    }
}
