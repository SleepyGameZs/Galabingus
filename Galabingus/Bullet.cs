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
using Microsoft.Xna.Framework.Input;

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
        Circle,
        Large,
        Seeker
    }

    // Zane Smith

    internal class Bullet : GameObject
    {

        #region-------------------[ Fields ]-------------------

        // Is this bullet ready to be destroyed?
        private bool bol_destroy;

        // Position data
        private Vector2 vc2_currentPosition;
        private Vector2 vc2_oldPosition;

        // Movement data - uses degrees for storage purposes
        private int int_angle;
        private Vector2 vc2_velocity;
        private int int_direction;

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

        /// <summary>
        /// The color of the bullet
        /// </summary>
        public Color Color
        {
            get
            {
                return clr_bulletColor;
            }
        }

        /// <summary>
        /// The angle of rotation for the bullet
        /// </summary>
        public int Angle
        {
            get
            {
                return int_angle;
            }
        }

        /// <summary>
        /// The direction of the bullet (flips where 0 is for angle)
        /// </summary>
        public int Direction
        {
            get
            {
                return int_direction;
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
        /// <param name="int_angle">The initial angle of the bullet</param>
        /// <param name="int_direction">The initial direction of the bullet</param>
        /// <param name="ush_contentName">Name to use for GameObject storage</param>
        /// <param name="scale">scale of bullet object</param>
        /// <param name="bulletNumber">Number to give bullet in GameObject list</param>
        public Bullet (
            BulletType BT_ability,
            Vector2 vc2_position,
            int int_angle,
            int int_direction,
            ushort ush_contentName,
            ushort ush_bulletNumber
        ) : base(ush_contentName, ush_bulletNumber)
        {
            // Set Sprite from given
            this.ush_contentName = ush_contentName;
            this.ush_bulletNumber = ush_bulletNumber;

            switch (BT_ability)
            {
                case BulletType.Normal:
                    clr_bulletColor = Color.LightBlue;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Bouncing:
                    clr_bulletColor = Color.Orange;
                    GameObject.Instance.Content = GameObject.Instance.Content.tinybullet_strip4;
                    break;

                case BulletType.Splitter:
                    clr_bulletColor = Color.LimeGreen;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.SplitSmall:
                    clr_bulletColor = Color.LimeGreen;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Circle:
                    clr_bulletColor = Color.DarkMagenta;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Large:
                    clr_bulletColor = Color.Yellow;
                    GameObject.Instance.Content = GameObject.Instance.Content.bigbullet_strip4;
                    break;

                case BulletType.Seeker:
                    clr_bulletColor = Color.Violet;
                    GameObject.Instance.Content = GameObject.Instance.Content.circlebullet_strip4;
                    break;

                default:
                    clr_bulletColor = Color.White;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;
            }

            // Set bullet state & timer
            this.BT_ability = BT_ability;
            int_stateTimer = 0;

            // Set the animation duration
            this.Animation.AnimationDuration = 0.03f;

            // Set Position
            this.Scale = 3f;
            this.Position = new Vector2(vc2_position.X + Transform.Width * Scale / 2.0f, vc2_position.Y - Transform.Height * Scale / 2.0f);
            vc2_currentPosition = this.Position;
            vc2_oldPosition = this.Position;

            // Convert to radians
            this.int_direction = int_direction;
            this.int_angle = int_angle;
            if (int_direction < 0)
            {
                int_angle += 180;
            }

            // Set values for vector lengths
            float flt_horizontalVal = (float)Math.Cos(MathHelper.ToRadians(int_angle));
            float flt_verticalVal = (float)Math.Sin(MathHelper.ToRadians(int_angle));
            vc2_velocity = Vector2.Normalize(new Vector2(flt_horizontalVal, flt_verticalVal));

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
            // Get old position
            vc2_oldPosition = this.Position;

            // Ability specific setting
            switch (BT_ability)
            {
                case BulletType.Normal:
                    // Set Current Position
                    vc2_currentPosition = SetPosition(gameTime, 8);

                    break;

                case BulletType.Bouncing:
                    // Set Current Position
                    vc2_currentPosition = SetPosition(gameTime, 3);

                    // Check for wall collison
                    bool CeilingHit = this.Position.Y < Sprite.Height;
                    bool FloorHit = this.Position.Y > BulletManager.Instance.ScreenDimensions.Y - Sprite.Height * 3;

                    if (CeilingHit)
                    {
                        vc2_velocity.Y *= -1;

                        // Change angle (check direction)
                        if (int_direction < 0)
                        {
                            int_angle -= 90;
                        } 
                        else
                        {
                            int_angle += 90;
                        }
                        
                    }

                    if (FloorHit)
                    {
                        vc2_velocity.Y *= -1;

                        // Change angle (check direction)
                        if (int_direction < 0)
                        {
                            int_angle += 90;
                        }
                        else
                        {
                            int_angle -= 90;
                        }
                    }

                    break;

                case BulletType.Splitter:
                    // Set Current Position
                    vc2_currentPosition = SetPosition(gameTime, 6);

                    // Center position of the player
                    float flt_PlayerX = Player.PlayerInstance.Position.X +              // Base Position
                                        Player.PlayerInstance.Sprite.Bounds.Center.X +  // Centering
                                        Player.PlayerInstance.Velocity.X;               // Adds player velocity

                    // Split into 2 bullets
                    if (vc2_currentPosition.X < flt_PlayerX && vc2_currentPosition.X > flt_PlayerX - 20)
                    {
                        // Fix positions
                        Vector2 vc2_topBullet = new Vector2(vc2_currentPosition.X - 50, vc2_currentPosition.Y);
                        Vector2 vc2_bottomBullet = new Vector2(vc2_currentPosition.X - 30, vc2_currentPosition.Y);

                        // Create Bullets
                        BulletManager.Instance.CreateBullet(BulletType.SplitSmall, vc2_topBullet, 90, int_direction);
                        BulletManager.Instance.CreateBullet(BulletType.SplitSmall, vc2_bottomBullet, -90, int_direction);

                        // Tell Bullet Manager to delete this bullet
                        bol_destroy = true;
                    }

                    break;

                case BulletType.SplitSmall:
                    // Set Current Position
                    vc2_currentPosition = SetPosition(gameTime, 10);
                    break;

                case BulletType.Circle:
                    // Set Current Position
                    vc2_currentPosition = SetPosition(gameTime, 1);
                    break;

                case BulletType.Large:
                    // Set Current Position
                    vc2_currentPosition = SetPosition(gameTime, 7);
                    break;

                case BulletType.Seeker:
                    // Change angle
                    if (int_stateTimer < 200)
                    {
                        Player obj_player = Player.PlayerInstance;
                        Vector2 vc2_playerCenter = new Vector2(obj_player.Position.X +              // Base X position 
                                                               obj_player.Sprite.Bounds.Center.X +  // Center X
                                                               obj_player.Velocity.X,               // Velocity X
                                                               obj_player.Position.Y +              // Base Y
                                                               obj_player.Sprite.Bounds.Center.Y +  // Center Y
                                                               obj_player.Velocity.Y);              // Velocity X

                        Vector2 vc2_bulletCenter = new Vector2(vc2_oldPosition.X +
                                                               Sprite.Bounds.Center.X,
                                                               vc2_oldPosition.Y +
                                                               Sprite.Bounds.Center.Y);

                        // Find vector distance between player and bullet
                        Vector2 vc2_playerBulletDistance = vc2_playerCenter - vc2_bulletCenter;

                        // Find angle distance between player and bullet
                        double dbl_playerBulletAngle = Math.Atan2(vc2_playerBulletDistance.X, vc2_playerBulletDistance.Y);

                        vc2_velocity = Vector2.Normalize( // Normalize in case of trolling
                                                         new Vector2((float)(10 * Math.Sin(dbl_playerBulletAngle)), // X
                                                                     (float)(10 * Math.Cos(dbl_playerBulletAngle))  // Y
                                                                     ));
                    }

                    // Set Current Position
                    vc2_currentPosition = SetPosition(gameTime, 4);
                    break;

                default:
                    // Doesn't move lol
                    break;
            }

            // Increment State Timer
            int_stateTimer++;

            // Manage Animation
            //this.Animation.AnimationDuration = 0.03f; // Matt: Don't do this here set this in the constructor
            // Matt: special relativity requires animation with velocity account at what position and size
            this.Transform = this.Animation.Play(gameTime, vc2_velocity, Position, Transform, Scale); 

            // Check if off screen
            bool bol_bulletOffScreen = this.Position.X < 0 &&
                                       this.Position.X > BulletManager.Instance.ScreenDimensions.X;
            if (bol_bulletOffScreen)
            {
                bol_destroy = true;
            }

        }

        /// <summary>
        /// Handles changing the player's position normally, then returns the player's current position
        /// </summary>
        /// <param name="gameTime">Game Time Data</param>
        /// <returns></returns>
        private Vector2 SetPosition(GameTime gameTime, int int_abilitySpeed)
        {
            int int_speedmulti = 1;
            // Matt's framerate fixing data
            float deltaTime = (float)Animation.EllapsedTime;

            // Sets position
            this.Position += vc2_velocity * int_speedmulti * int_abilitySpeed * deltaTime;

            // Returns position
            return this.Position;
        }

        #endregion

    }
}
