﻿using Microsoft.Xna.Framework;
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
using System.Security.Cryptography;

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
        private bool destroy;

        // Position data
        private Vector2 currentPosition;
        private Vector2 oldPosition;

        // Movement data - uses degrees for storage purposes
        private int angle;
        private Vector2 velocity;
        private int direction;

        // State data
        private BulletType ability;
        private int stateTimer;

        // Animation Data
        private Color bulletColor;

        // Name used to find values from GameObject dynamic
        private ushort contentName;

        // Number into game object index to look for items
        private ushort bulletNumber;

        // Reference to the object that created the bullet
        private object creatorReference;

        #endregion 

        #region-------------------[ Properties ]-------------------

        /// <summary>
        /// Used to see if this bullet should be destroyed now
        /// </summary>
        public bool Destroy
        {
            get { 
                return destroy; 
            }
            set
            {
                destroy = value;
            }
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
                return GetPosition(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetPosition(bulletNumber, value);
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
                return GetSprite(bulletNumber);
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
                return GetTransform(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetTransform(bulletNumber, value);
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
                return GetScale(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetScale(bulletNumber, value);
            }
        }

        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetAnimation(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetAnimation(bulletNumber, value);
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
                return GetCollider(bulletNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetCollider(bulletNumber, value);
            }
        }

        /// <summary>
        /// The color of the bullet
        /// </summary>
        public Color Color
        {
            get
            {
                return bulletColor;
            }
        }

        /// <summary>
        /// The angle of rotation for the bullet
        /// </summary>
        public int Angle
        {
            get
            {
                return angle;
            }
        }

        /// <summary>
        /// The direction of the bullet (flips where 0 is for angle)
        /// </summary>
        public int Direction
        {
            get
            {
                return direction;
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

        /// <summary>
        /// Returns the bullet number of this bullet
        /// </summary>
        public ushort BulletNumber
        {
            get
            {
                return bulletNumber;
            }
        }

        /// <summary>
        /// Returns the bullet's ability
        /// </summary>
        public BulletType Ability
        {
            get
            {
                return ability;
            }
        }

        #endregion

        #region-------------------[ Constructor ]-------------------

        /// <summary>
        /// Spawns a bullet with given stats
        /// </summary>
        /// <param name="ability">The ability to give the bullet</param>
        /// <param name="position">The position to spawn the bullet at</param>
        /// <param name="angle">The angle the bullet should move at</param>
        /// <param name="direction">The direction of the bullet, mainly for visuals</param>
        /// <param name="creator">Reference to the object who created the bullet</param>
        /// <param name="contentName">Name to use for GameObject storage</param>
        /// <param name="bulletNumber">Number to give bullet in GameObject list</param>
        public Bullet (
            BulletType ability,
            Vector2 position,
            int angle,
            int direction,
            object creator,
            ushort contentName,
            ushort bulletNumber
        ) : base(contentName, bulletNumber, CollisionGroup.Bullet)
        {
            //this.thisGameObject = this;
            // Set Sprite from given
            this.contentName = contentName;
            this.bulletNumber = bulletNumber;

            // Establish bullet color and link to game object correct image
            switch (ability)
            {
                case BulletType.Normal:
                    bulletColor = Color.LightBlue;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Bouncing:
                    bulletColor = Color.Orange;
                    GameObject.Instance.Content = GameObject.Instance.Content.tinybullet_strip4;
                    break;

                case BulletType.Splitter:
                    bulletColor = Color.LimeGreen;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.SplitSmall:
                    bulletColor = Color.LimeGreen;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Circle:
                    bulletColor = Color.DarkMagenta;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;

                case BulletType.Large:
                    bulletColor = Color.Yellow;
                    GameObject.Instance.Content = GameObject.Instance.Content.bigbullet_strip4;
                    break;

                case BulletType.Seeker:
                    bulletColor = Color.Violet;
                    GameObject.Instance.Content = GameObject.Instance.Content.circlebullet_strip4;
                    break;

                default:
                    bulletColor = Color.White;
                    GameObject.Instance.Content = GameObject.Instance.Content.smallbullet_strip4;
                    break;
            }
            
            // Set the owner reference
            creatorReference = creator;

            // Set bullet state & timer
            this.ability = ability;
            stateTimer = 0;

            // Set the animation duration
            this.Animation.AnimationDuration = 0.03f;

            // Set Position
            this.Scale = Player.PlayerInstance.Scale;
            this.Position = new Vector2(position.X + Transform.Width * Scale / 2.0f, position.Y - Transform.Height * Scale / 2.0f);
            currentPosition = this.Position;
            oldPosition = this.Position;

            // Convert to radians
            this.direction = direction;
            this.angle = angle;
            if (direction < 0)
            {
                angle += 180;
            }

            // Set values for vector lengths
            float horizontalVal = (float)Math.Cos(MathHelper.ToRadians(angle));
            float verticalVal = (float)Math.Sin(MathHelper.ToRadians(angle));
            velocity = Vector2.Normalize(new Vector2(horizontalVal, verticalVal));

            // Set sprite manually at position
            //GameObject.Instance.Content = ::file name::
            //GameObject.Instance.Sprite;

            // Set constructor easier access
            // contentName = ::file name::;

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
            oldPosition = this.Position;

            // Ability specific setting
            switch (ability)
            {
                case BulletType.Normal:
                    // Set Current Position
                    if (Creator == Player.PlayerInstance)
                    {
                        currentPosition = SetPosition(gameTime, 16, true);
                    } 
                    else
                    {
                        currentPosition = SetPosition(gameTime, 16, false);
                    }
                    
                    break;

                case BulletType.Bouncing:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 3, false);

                    // Check for wall collison
                    bool CeilingHit = this.Position.Y < Sprite.Height;
                    bool FloorHit = this.Position.Y > BulletManager.Instance.ScreenDimensions.Y - Sprite.Height * 3;

                    if (CeilingHit)
                    {
                        velocity.Y *= -1;

                        // Change angle (check direction)
                        if (direction < 0)
                        {
                            angle -= 90;
                        }
                        else
                        {
                            angle += 90;
                        }

                    }

                    if (FloorHit)
                    {
                        velocity.Y *= -1;

                        // Change angle (check direction)
                        if (direction < 0)
                        {
                            angle += 90;
                        }
                        else
                        {
                            angle -= 90;
                        }
                    }

                    break;

                case BulletType.Splitter:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 6, false);

                    // X Position of the player
                    float PlayerX = Player.PlayerInstance.Position.X    // Base Position
                                    + Player.PlayerInstance.Velocity.X; // Player Velocity 

                    float rightBound = PlayerX;
                    float leftBound = PlayerX;

                    // Bullet will split when it reaches the visual center of the player
                    // (or anywhere up to the end of the player's sprite afterward)

                    if (direction < 0)
                    { // Facing Right
                        rightBound = (PlayerX + Player.PlayerInstance.Transform.Width * Player.PlayerInstance.Scale * 0.35f) + 1;
                        leftBound = (PlayerX) - 1;
                    } 
                    else
                    { // Facing Left
                        rightBound = (PlayerX + Player.PlayerInstance.Transform.Width * Player.PlayerInstance.Scale) + 1;
                        leftBound = (PlayerX + Player.PlayerInstance.Transform.Width * Player.PlayerInstance.Scale * 0.65f) - 1;
                    }

                    // Split into 2 bullets
                    if (currentPosition.X < rightBound && currentPosition.X > leftBound)
                    {
                        // Create Bullets
                        //BulletManager.Instance.CreateBullet(BulletType.SplitSmall, currentPosition, 90, direction, creatorReference, true);
                        //BulletManager.Instance.CreateBullet(BulletType.SplitSmall, currentPosition, -90, direction, creatorReference, true);

                        // Tell Bullet Manager to delete this bullet
                        destroy = true;
                    }
                    break;

                case BulletType.SplitSmall:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 10, true);
                    break;

                case BulletType.Circle:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 1, false);
                    break;

                case BulletType.Large:
                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 7, false);
                    break;

                case BulletType.Seeker:
                    // Change angle
                    if (stateTimer < 200)
                    {
                        Player player = Player.PlayerInstance;

                        // Get Player's Center relative to bullet
                        Vector2 playerCenter = new Vector2(player.Position.X + (player.Transform.Width * player.Scale) / 2,
                                                           player.Position.Y + (player.Transform.Height * player.Scale) / 2);

                        playerCenter = playerCenter + new Vector2((Transform.Width * Scale) / 2,
                                                                  (Transform.Height * Scale) / 2);

                        // Get Bullet's  Center
                        Vector2 bulletCenter = new Vector2(oldPosition.X, oldPosition.Y);

                        // Find vector distance between player and bullet
                        Vector2 playerBulletDistance = playerCenter - bulletCenter;

                        // Find angle distance between player and bullet
                        double playerBulletAngle = Math.Atan2(playerBulletDistance.X, playerBulletDistance.Y);

                        // Normalize in case of trolling + perform calculations
                        velocity = Vector2.Normalize(new Vector2((float)(10 * Math.Sin(playerBulletAngle)), // X
                                                                 (float)(10 * Math.Cos(playerBulletAngle))  // Y
                                                     ));
                    }

                    // Set Current Position
                    currentPosition = SetPosition(gameTime, 3, true);
                    break;

                default:
                    // Doesn't move lol
                    break;
            }

            // Increment State Timer
            stateTimer++;
            
            // Creates currect collider for Enemy
            /*this.Transform = this.Animation.Play(gameTime);*/
            List<Collision> intercepts = this.Collider.UpdateTransform(
                this.Sprite,                         // Bullet Sprite
                this.Position,                       // Bullet position
                this.Transform,                      // Bullet transform for sprite selection
                GameObject.Instance.GraphicsDevice,
                GameObject.Instance.SpriteBatch,
                this.direction,
                new Vector2(this.Scale,this.Scale),                          // Bullet scale
                SpriteEffects.None,
                (ushort)CollisionGroup.Bullet,                           // Collision Layer
                bulletNumber
            );

            // Check if off screen
            bool bol_bulletOffScreen = this.Position.X < 0 ||
                                       this.Position.X > BulletManager.Instance.ScreenDimensions.X;
            if (bol_bulletOffScreen)
            {
                destroy = true;
            }

            this.Collider.Resolved = true;

            /*foreach (Collision collision in intercepts)
            {
                if (collision.other != null)
                {
                    if (((collision.other as Player) is Player) && !destroy && !((collision.self as Bullet).Creator is Player))
                    {
                        // TODO: Write Player damage stuff here
                        if ((Player.PlayerInstance.Health - 0.5) >= 0)
                        {
                            Player.PlayerInstance.Health = Player.PlayerInstance.Health - 0.5f;
                        }
                        destroy = true;
                        velocity = Vector2.Zero;
                    }
                    else if (((collision.other as Enemy) is Enemy) && !destroy && !((collision.self as Bullet).Creator is Enemy))
                    {
                        // TODO: Write Enemy damage stuff here
                        ((Enemy)collision.other).Health -= 1;
                        destroy = true;
                        velocity = Vector2.Zero;

                        if (((Enemy)collision.other).Health <= 0)
                        {
                            ((Enemy)collision.other).Destroy = true;
                        }

                    }
                }
            }*/
        }

        /// <summary>
        /// Handles changing the player's position normally, then returns the player's current position
        /// </summary>
        /// <param name="gameTime">Game Time Data</param>
        /// <returns></returns>
        private Vector2 SetPosition(GameTime gameTime, int abilitySpeed, bool ignoreCamera)
        {
            int speedmulti = 1;

            // Sets position
            Vector2 finalVelocity = (velocity *                                 // Actual velocity
                                    speedmulti *                                // Speed multiplier for changing stats
                                    abilitySpeed *                              // Ability specific speed changes
                                    Player.PlayerInstance.TranslationRatio *    
                                    (float)this.Animation.EllapsedTime          // Animation data
                                    );

            // Final position change, includes camera movement
            if (ignoreCamera)
            {
                this.Position += finalVelocity;
            } else
            {
                this.Position += finalVelocity - Camera.Instance.OffSet;
            }
            

            // Returns position
            return this.Position;
        }

        #endregion

    }
}
