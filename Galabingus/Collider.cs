using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// Matthew Rodriguez
// 2023, 3, 13
// Collision
// Empty - Collision that is empty
// positionOfCollision - Point of the collision
// position - Position of which to avoid the collision
// Collider
// Empty - Collider that is empty
// Sprite - Sprite of the collider
// Transform - Collider specific Rectangle
// PixelCheck - Re-writable function for pixel colors
// Layer - The Collider layer: discribes what not to collide with
// Resolved -  MUST bet set to true after a collision has been resolved
// Load - Loads the pixel data from GPU to CPU memory <!>use with cation<!>
// Unload - Forever gets rid of the colldier pixels <!>use with cation<!>
// UpdateTransform - updates the transform for the collider, after updating MUST set Resolved to true
// PixelCheckFunction - method form of PixelCheck
// PixelIntercepts - Returns the Collision positions of another collider

namespace Galabingus
{
    /// <summary>
    ///  Collision
    ///  Provides what GameObject 
    ///  collided with this one
    ///  Provides the position at which 
    ///  the collision happened
    /// </summary>
    internal struct Collision
    {
        private static Collision empty;     // Singleton for Collision
        public GameObject other;            // GameObject collided with
        public Vector2 positionOfCollision; // Point of the collision Default: (-1,-1)
        public Vector2 position;            // Position of which to avoid the collison Default (-1,-1)
        public Vector2 mtv;

        /// <summary>
        ///  Base collision, default
        /// </summary>
        public static Collision Empty
        {
            // Singleton
            get
            {
                return empty;
            }
        }

        /// <summary>
        ///  Other is null by default
        ///  PositionOfCollision default is the top left off-screen pixel
        ///  Position default is the top left off-screen pixel
        /// </summary>
        public Collision()
        {
            this.other = null;
            this.positionOfCollision = new Vector2(-1, -1);
            this.position = new Vector2(-1, -1);
            this.mtv = new Vector2(-1, -1);
        }

        /// <summary>
        ///  Creates a Collision defining:
        ///  other, positionOfCollision, and position
        /// </summary>
        /// <param name="other">GameObject collided with</param>
        /// <param name="positionOfCollision">position of collision</param>
        /// <param name="position">position of which to avoid the collision</param>
        public Collision(
            GameObject other,
            Vector2 positionOfCollision,
            Vector2 position,
            Vector2 mtv
        )
        {
            this.other = other;
            this.positionOfCollision = positionOfCollision;
            this.position = position;
            this.mtv = mtv;
        }
    }

    /// <summary>
    ///  Collider
    ///  Provides all Colliders the ability to collide
    ///  Implements from GameObject.cs
    /// </summary>
    internal class Collider
    {
        private static Collider empty = null;                // Collider singleton
        private Texture2D sprite;                            // Collider sprite with all applied effects
        private Rectangle transform;                         // Collider transform: contains the position, scaled height and width
        private Vector2 position;                            // Position of the Collider
        private Color[] pixels;                              // Sprite pixel information
        private Func<Color, Color, Color, bool> pixelCheck;  // The pixel check function
        private Color clearColor;                            // Clear color for the sprite render
        private ushort layer;                                // Collider layer
        private bool resolved;                               // If the Collider collision is resolved
        private Vector2 colldierCurrentMTV;
        private Vector2 colliderNextMTV;

        /// <summary>
        ///  Colider that is empty
        /// </summary>
        public static Collider Empty
        {
            // Collider singleton
            get
            {
                if (empty != null)
                {
                    return empty;
                }
                else
                {
                    empty = new Collider();
                    return empty;
                }
            }
        }

        /// <summary>
        ///  Sprite of the collider
        /// </summary>
        public Texture2D Sprite
        {
            // Only allow the sprite to be received
            get
            {
                return sprite;
            }
        }

        /// <summary>
        ///  x: x position of the collider,
        ///  y: y posisition of the collider,
        ///  Width: width of the collider scaled to scale
        ///  Height: height of the collider scaled to scale
        /// </summary>
        public Rectangle Transform
        {
            // Only allow the transform to be received
            get
            {
                return transform;
            }
        }

        /// <summary>
        ///  The Pixel conditioning Funciton
        ///  With the input of: Color A to check against Color B with value of Color C,
        ///  [Color A, Color B, Color C]
        /// </summary>
        public Func<Color, Color, Color, bool> PixelCheck
        {
            // Allow the PixelCheck to be defined
            // When getting the PixelCheck function if it isn't defined define it
            get
            {
                return pixelCheck ?? // Default to checking for clear color
                delegate (Color colorOne, Color colorTwo, Color clearColor)
                {
                    return (colorOne != clearColor && colorTwo != clearColor);
                };
            }
            set
            {
                pixelCheck = value;
            }
        }

        /// <summary>
        ///  Layer to not collide with
        /// </summary>
        public ushort Layer
        {
            // Allow the layer to be modified
            get
            {
                return layer;
            }
            set
            {
                layer = value;
            }
        }

        /// <summary>
        ///  Determines if the collider should be updated
        ///  The collider will not update until the resoltion of a collision
        /// </summary>
        public bool Resolved
        {
            // Collision resolution MUST be able to be set and received 
            get
            {
                return resolved;
            }
            set
            {
                resolved = value;
            }
        }

        /// <summary>
        ///  Only defaults values
        /// </summary>
        public Collider()
        {
            sprite = null;
            position = Vector2.Zero;
            transform = new Rectangle(0, 0, 0, 0);
            pixels = null;
            pixelCheck = null;
            clearColor = Color.Transparent;
            layer = 0;
            resolved = false;
        }

        /// <summary>
        ///  Makes a collider from the information needed 
        ///  to render the final look of the collider sprite
        /// </summary>
        /// <param name="sprite">Sprite of the collider</param>
        /// <param name="position">position of the collider</param>
        /// <param name="transform">Transform for the collider at scale 1</param>
        /// <param name="effect">The SpriteEffect for the collider</param>
        /// <param name="scale">scale of the collider</param>
        /// <param name="graphicsDevice">any: GraphicsDevice</param>
        /// <param name="spriteBatch">any: SpriteBatch</param>
        /// <param name="layer">layer of the collider</param>
        public Collider(
            Texture2D sprite,
            Vector2 position,
            Rectangle transform,
            SpriteEffects effect,
            float scale,
            GraphicsDevice graphicsDevice,
            SpriteBatch spriteBatch,
            ushort layer
        )
        {
            // Set the transform
            this.transform = transform;
            this.position = position;
            this.layer = layer;
            resolved = false;
            pixels = null;
            pixelCheck = null;

            // Render the effects and scale
            RenderTarget2D scaledSprite = new RenderTarget2D(
                graphicsDevice,
                (int)(transform.Width * scale) <= 0 ? 1 : (int)(transform.Width * scale),
                (int)(transform.Height * scale) <= 0 ? 1 : (int)(transform.Height * scale)
            );
            clearColor = Color.Transparent;
            graphicsDevice.SetRenderTarget(scaledSprite);
            graphicsDevice.Clear(clearColor);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            spriteBatch.Draw(
                sprite,
                new Vector2(0, 0),
                this.transform,
                Color.White,
                0.0f,
                Vector2.Zero,
                scale,
                effect,
                1.0f
            );
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

            // Update the sprite to the new scale with effects
            this.sprite = scaledSprite;

            // Scale the transform
            // Load pixel data to CPU memory
            Load();
        }

        /// <summary>
        ///  Loads the pixel data from the GPU to the CPU memory
        /// </summary>
        public void Load()
        {
            pixels = new Color[sprite.Width * sprite.Height];
            sprite.GetData(pixels);
        }

        /// <summary>
        ///  Unloads the pixel data from the CPU memory
        /// </summary>
        public void Unload()
        {
            pixels = null;
        }

        /// <summary>
        ///  Updates the Transform for the Collider
        /// </summary>
        /// <param name="sprite">Sprite for the collider</param>
        /// <param name="position">Position of the collider</param>
        /// <param name="transform">Offset, Height and Width</param>
        /// <param name="graphicsDevice">Any: GraphicsDevice</param>
        /// <param name="spriteBatch">Any: SpriteBatch</param>
        /// <param name="scale">Scale of the collider</param>
        /// <param name="effect">SpriteEffect applied to the collider</param>
        public List<Collision> UpdateTransform(
            Texture2D sprite,
            Vector2 position,
            Rectangle transform,
            GraphicsDevice graphicsDevice,
            SpriteBatch spriteBatch,
            float scale,
            SpriteEffects effect,
            ushort layer,
            ushort instanceNumber
        )
        {
            // Set the temporal states for the collider
            // These determine if the collider needs to update
            bool active = false;  // The collider needs to check for collision
            bool updated = false; // The collider needs to update its pixel data
            this.layer = layer;   // The collider's layer

            // Create transform from scale and position
            this.transform = new Rectangle(
                (int)(position.X),
                (int)(position.Y),
                (int)(transform.Width * scale),
                (int)(transform.Height * scale)
            );

            // Determine the bounds of the collider
            this.position = position;
            if (position.X - transform.Width * scale > GraphicsDeviceManager.DefaultBackBufferWidth ||
                position.X + transform.Width * scale < 0 ||
                position.Y - transform.Height * scale > GraphicsDeviceManager.DefaultBackBufferHeight ||
                position.Y + transform.Height * scale < 0
            )
            {
                this.sprite = null;
                return new List<Collision>();
            }

            List<Collision> result = new List<Collision>();
            ushort layer4 = GameObject.Instance.ColliderLayer4()[instanceNumber];
            unsafe
            {
                ref List<Collider> collidersR = ref GameObject.Instance.ColliderCollisions();

                // Go through all collider instances to check for a collision and determine what colliders are active
                for (ushort colliderIndex = 0; colliderIndex < collidersR.Count; colliderIndex++)
                {
                    // The other collider
                    Collider otherCollider = collidersR[colliderIndex];

                    // Determine if the colldier exist as a instance of this content
                    if (layer4 != colliderIndex)
                    {
                        // When the bounds are intercepting and the layer isn't the same and all collisions have been resolved
                        // Then we can activate the collider
                        if (this.resolved &&
                            otherCollider.layer != this.layer &&
                            otherCollider.transform.Intersects(this.transform)
                        )
                        {
                            active = true;
                        }
                        else
                        {
                            active = false;
                            this.colliderNextMTV = Vector2.Zero;
                            this.colldierCurrentMTV = Vector2.Zero;
                        }

                        // Only update the collider once
                        if (active && !updated)
                        {
                            updated = true;
                            // Setup the renderTarget
                            RenderTarget2D scaledSprite = new RenderTarget2D(graphicsDevice,
                                (int)(transform.Width * scale) <= 0 ? 1 : (int)(transform.Width * scale),
                                (int)(transform.Height * scale) <= 0 ? 1 : (int)(transform.Height * scale)
                            );

                            // Render the new sprite 
                            graphicsDevice.SetRenderTarget(scaledSprite);
                            graphicsDevice.Clear(clearColor);
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                            spriteBatch.Draw(
                                sprite,
                                new Vector2(0, 0),
                                transform,
                                Color.Black,
                                0.0f,
                                Vector2.Zero,
                                scale,
                                effect,
                                1.0f
                            );
                            spriteBatch.End();
                            graphicsDevice.SetRenderTarget(null);
                            //this.colliderNextMTV = Vector2.Zero;
                            //this.colldierCurrentMTV = Vector2.Zero;

                            // Update the transform with the new scale and sprite
                            this.sprite = scaledSprite;
                            // Load pixel data to CPU memory
                            Load();
                        }

                        // When the collider is active check for a collision
                        if (active)
                        {
                            // Define the collision points
                            Vector2[] other = (
                                otherCollider != this ? // We are not the same collider
                                    (this.PixelsIntersects(otherCollider)) : // Pixels intercept points
                                        new Vector2[] { new Vector2(-1, -1), new Vector2(-1, -1), new Vector2(-1, -1) } // otherwise default to a new array
                            );

                            // If there was a collision
                            if (new Vector2(-1, -1) != other[1] && other != null)
                            {
                                // Set the resolved collision to false
                                // Return the Collision
                                this.resolved = false;
                                //GameObject.Instance.SetCollider(instanceNumber,this);
                                result.Add(new Collision(
                                    GameObject.Instance,
                                    other[0],
                                    other[1],
                                    other[2]
                                ));
                            }
                        }
                    }
                }
            }

            // No collision
            return result;
        }

        /// <summary>
        ///  Calls the PixelCheck function
        /// </summary>
        /// <param name="pixelA">Color A</param>
        /// <param name="pixelB">Color B</param>
        /// <param name="test">Color C</param>
        /// <returns>Pixel Intercept Status</returns>
        public bool PixelCheckFunction(Color pixelA, Color pixelB, Color test)
        {
            return PixelCheck(pixelA, pixelB, test);
        }

        /// <summary>
        ///  Calculates the Collision points between two colliders
        /// </summary>
        /// <param name="other">other collider</param>
        /// <returns>Collision points</returns>
        public Vector2[] PixelsIntersects(
            Collider other
        )
        {
            // Exit if sprites have not been defined
            if (this.sprite == null || other.sprite == null)
            {
                // Exit with off-screen positions
                return new Vector2[] { new Vector2(-1, -1), new Vector2(-1, -1), new Vector2(-1, -1) };
            }

            // Calculate the intersecting rectangle
            int x1 = Math.Max(this.transform.X, other.transform.X);
            int x2 = Math.Min(this.transform.X + this.transform.Width, other.transform.X + other.transform.Width);
            int y1 = Math.Max(this.transform.Y, other.transform.Y);
            int y2 = Math.Min(this.transform.Y + this.transform.Height, other.transform.Y + other.transform.Height);

            // For each single pixel in the intersecting rectangle
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    if (((x - this.transform.X) + (y - this.transform.Y) * (this.transform.Width)) < this.pixels.Length &&
                        ((x - other.transform.X) + (y - other.transform.Y) * (other.transform.Width)) < other.pixels.Length
                    )
                    {
                        Color a1 = this.pixels[(x - this.transform.X) + (y - this.transform.Y) * (this.transform.Width)];
                        Color b1 = other.pixels[(x - other.transform.X) + (y - other.transform.Y) * (other.transform.Width)];

                        // Use the PixelCheck to determine intercept
                        if (PixelCheckFunction(a1, b1, clearColor))
                        {
                            Vector2 interceptPosition = new Vector2(x1 + Math.Abs(x2 - x1) / 2.0f, y1 + Math.Abs(x2 - x1) / 2.0f);
                            Vector2 intercept;
                            Vector2 mtv = MTV(this, x2, y2, y1, x1, interceptPosition.Y, interceptPosition.X);
                            Vector2 otherMTV = MTV(other, x2, y2, y1, x1, interceptPosition.Y, interceptPosition.X);

                            intercept = new Vector2(position.X + mtv.X, position.Y + mtv.Y);

                            if (this.colliderNextMTV == Vector2.Zero)
                            {
                                this.colliderNextMTV = otherMTV;
                            }

                            colldierCurrentMTV = colliderNextMTV;
                            
                            if (
                                Math.Abs(otherMTV.X) < Math.Abs(colldierCurrentMTV.Y) && otherMTV.X != 0 && colldierCurrentMTV.Y != 0 ||
                                Math.Abs(otherMTV.Y) < Math.Abs(colldierCurrentMTV.X) && otherMTV.Y != 0 && colldierCurrentMTV.X != 0 ||
                                otherMTV.Y != 0 && colldierCurrentMTV.Y != 0 && otherMTV.X != 0 && colldierCurrentMTV.X != 0
                            )
                            {
                                colliderNextMTV = otherMTV;
                            }

                            // Return the resulting rectangle
                            return new Vector2[]
                            {
                                interceptPosition, // Position of pixel intercept
                                new Vector2(
                                    intercept.X, // Position to avoid intercept X
                                    intercept.Y  // Position to avoid intercept Y
                                ),
                                colliderNextMTV
                            };
                        }
                    }
                }
            }

            // Default to top-left offscreen positions
            return new Vector2[] { new Vector2(-1, -1), new Vector2(-1, -1), new Vector2(-1, -1) };
        }

        /// <summary>
        ///  Calculates the Minimum Translation Vector of the other collider
        /// </summary>
        /// <param name="other"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="y1"></param>
        /// <param name="x1"></param>
        /// <param name="y"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private Vector2 MTV(Collider other, int x2, int y2, int y1, int x1, float y, float x)
        {
            Vector2 mtv = new Vector2();

            int w = Math.Abs(x2 - x1);
            int h = Math.Abs(y2 - y1);

            float ocx = other.position.X + other.Transform.Width / 2.0f;
            float ocy = other.position.Y + other.Transform.Height / 2.0f;

            mtv.X = ocx - x;
            mtv.Y = ocy - y;

            if (mtv == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            mtv = Vector2.Normalize(mtv);

            if (mtv.X != 0 || mtv.Y != 0)
            {
                mtv = mtv * new Vector2((float)Math.Sqrt((x2 - x1)*(x2 - x1))/4.5f,(float)Math.Sqrt((y2 - y1)*(y2 - y1))/4.5f);
            }

            return mtv;
        }
    }
}
