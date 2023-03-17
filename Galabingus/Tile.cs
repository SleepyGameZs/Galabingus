using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace Galabingus
{
    internal class Tile : GameObject
    {
        // -------------------------------------------------
        // Fields
        // -------------------------------------------------

        private ushort contentName;
        private ushort instanceNumber;
        private ushort spriteNumber;

        // -------------------------------------------------
        // Properties
        // -------------------------------------------------

        public ushort SpriteNumber
        {
            get { return spriteNumber;  }
        }

        public ushort InstanceNumber
        {
            get
            {
                return this.instanceNumber;
            }
        }

        /// <summary>
        ///  Position of the player
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
        ///  Player sprite
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
        ///  Single Player sprite bounds
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
        ///  Scale of the palyer sprite
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
        ///  Player animation
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
        ///  Player collider
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

        // -------------------------------------------------
        // Contructors
        // -------------------------------------------------

        public Tile(ushort contentName, ushort instanceNumber, ushort sprite) : base(contentName, instanceNumber)
        {
            this.contentName = contentName;
            this.instanceNumber = instanceNumber;
            this.Transform = this.Animation.GetFrame(sprite);
            this.Scale = 3.0f;
            this.spriteNumber = sprite;
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        public void Update(GameTime gameTime)
        {
            this.Position -= Camera.Instance.OffSet;
        }

        public void Draw()
        {
            //this.Position = new Vector2(0, 0);
            //Debug.WriteLine(Position.X);
            //Debug.WriteLine(Position.Y);
            //Debug.WriteLine(this.Position);
            GameObject.Instance.SpriteBatch.Draw(
                this.Sprite,                          // The sprite-sheet for the player
                this.Position,                        // The position for the player
                this.Transform,                       // The scale and bounding box for the animation
                Color.White,                     // The color for the palyer
                0.0f,                            // There cannot be any rotation of the player
                Vector2.Zero,                    // Starting render position
                this.Scale,                      // The scale of the sprite
                SpriteEffects.None,              // Which direction the sprite faces
                0.0f                             // Layer depth of the player is 0.0
            );
        }
    }
}
