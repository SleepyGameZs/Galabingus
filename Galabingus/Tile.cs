using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

/* Tiles act as barriers for bullets and the players, blocking
 * the player's movement and destroing specific types of bullets.
 * Tiles scroll with the camera and make up both the level and the background */

namespace Galabingus
{
    internal class Tile : GameObject
    {
        #region Fields

        // Game object identifiers for asset and instance
        private ushort contentName;
        private ushort instanceNumber;

        // Size of the tile
        private Vector2 scale;

        // If the tile is active
        private bool isActive;

        #endregion

        #region Game Object Properties

        /// <summary>
        /// Instance identifier for the object
        /// </summary>
        public ushort InstanceNumber
        {
            get
            {
                return this.instanceNumber;
            }
        }

        /// <summary>
        ///  Position of the tile
        /// </summary>
        public Vector2 Position
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetPosition(instanceNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetPosition(instanceNumber, value);
            }
        }

        /// <summary>
        ///  Tile sprite
        /// </summary>
        public Texture2D Sprite
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetSprite(instanceNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetSprite(instanceNumber, value);
            }
        }

        /// <summary>
        ///  Tile sprite bounds
        /// </summary>
        public Rectangle Transform
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetTransform(instanceNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetTransform(instanceNumber, value);
            }
        }

        /// <summary>
        ///  Scale of the tile sprite
        /// </summary>
        public float Scale
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetScale(instanceNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetScale(instanceNumber, value);
            }
        }

        /// <summary>
        ///  Tile animation
        /// </summary>
        public Animation Animation
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetAnimation(instanceNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetAnimation(instanceNumber, value);
            }
        }

        /// <summary>
        ///  Tile collider
        /// </summary>
        public Collider Collider
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetCollider(instanceNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetCollider(instanceNumber, value);
            }
        }

        /// <summary>
        /// Effects run on the tile
        /// </summary>
        public Effect Effect
        {
            get
            {
                GameObject.Instance.Content = contentName;
                return GetEffect(instanceNumber);
            }
            set
            {
                GameObject.Instance.Content = contentName;
                SetEffect(instanceNumber, value);
            }
        }

        #endregion

        #region Tile Properties
        /// <summary>
        /// Allows for the indvidual scalling of X and Y
        /// </summary>
        public Vector2 ScaleVector
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        /// <summary>
        /// If the tile is currently active 
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }

        #endregion

        #region Contructor

        /// <summary>
        /// Create tiles from a sprite sheet
        /// </summary>
        /// <param name="contentName"></param>
        /// <param name="instanceNumber"></param>
        /// <param name="sprite"></param>
        public Tile(ushort contentName, ushort instanceNumber, ushort sprite) : base(contentName, instanceNumber, CollisionGroup.Tile)
        {
            // Game object parameter passing
            GameObject.Instance.Content = contentName;
            this.thisGameObject = this;
            this.contentName = contentName;
            this.instanceNumber = instanceNumber;

            // Update transfrom based on sprite needed
            this.Transform = this.Animation.GetFrame(sprite);

            // Update scale vector then active tile
            this.ScaleVector = PostScaleRatio(true);
            isActive = true;
        }

        /// <summary>
        /// Creates static tiles
        /// </summary>
        /// <param name="contentName"></param>
        /// <param name="instanceNumber"></param>
        /// <param name="sprite"></param>
        /// <param name="single"> If the tile is static </param>
        public Tile(ushort contentName, ushort instanceNumber, ushort sprite, bool single) : base(contentName, instanceNumber, CollisionGroup.Tile)
        {
            // Game object parameter passing
            GameObject.Instance.Content = contentName;
            this.thisGameObject = this;
            this.contentName = contentName;
            this.instanceNumber = instanceNumber;

            // Update transform based on first sprite
            this.Transform = this.Animation.GetFrame(0);

            // Update scale vector then active tile
            this.ScaleVector = PostScaleRatio(true);
            isActive = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines tile movement based on the camera scroll
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // Final position change, and whether or not to include camera movement
            if (Camera.Instance.CameraLock)
            { // In debug mode
                this.Position = new Vector2(this.Position.X, this.Position.Y - Player.PlayerInstance.Translation.Y);
            }
            else
            { // Normal camera movement
                this.Position = new Vector2(this.Position.X, this.Position.Y - Camera.Instance.OffSet.Y);
            }
        }

        /// <summary>
        /// Updates the postion of the background
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateBackground(GameTime gameTime)
        {
            this.Position = new Vector2(this.Position.X - Camera.Instance.OffSet.X, this.Position.Y - Camera.Instance.OffSet.Y);
        }

        /// <summary>
        /// Draws the tile
        /// </summary>
        public void Draw()
        {
            GameObject.Instance.SpriteBatch.Draw(
                this.Sprite,                           // The sprite-sheet for the player
                this.Position,                         // The position for the player
                this.Transform,                        // The scale and bounding box for the animation
                Color.White,                           // The color for the palyer
                0.0f,                                  // There cannot be any rotation of the player
                Vector2.Zero,                          // Starting render position
                this.ScaleVector,                      // The scale of the sprite
                SpriteEffects.None,                    // Which direction the sprite faces
                0.0f                                   // Layer depth of the player is 0.0
            );
        }

        public void Draw(float xTimes, float yTimes)
        {
            GameObject.Instance.SpriteBatch.End();
            Effect.Parameters["bossEffect"].SetValue(GameObject.Instance.IsBossEffectActive);
            Effect.Parameters["bossShade"].SetValue(GameObject.Instance.TimeShade);
            GameObject.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, effect: Effect);

            GameObject.Instance.SpriteBatch.Draw(
                this.Sprite,                      
                this.Position,                 
                new Rectangle(
                    this.Transform.X, 
                    this.Transform.Y, 
                    (int)Math.Round(this.Transform.Width * xTimes, MidpointRounding.AwayFromZero), 
                    (int)Math.Round(this.Transform.Height * yTimes, MidpointRounding.AwayFromZero)
                ),                     
                Color.White,                 
                0.0f,                        
                Vector2.Zero,           
                this.ScaleVector,
                SpriteEffects.None, 
                0.0f
            );
            GameObject.Instance.SpriteBatch.End();
            GameObject.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, effect: GameObject.Instance.UniversalShader);
        }
        #endregion
    }
}
