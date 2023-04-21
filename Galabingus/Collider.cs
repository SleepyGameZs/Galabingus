using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;

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
        public object self;
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
            this.self = default;
            this.other = default;
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
            object self,
            GameObject other,
            Vector2 positionOfCollision,
            Vector2 position,
            Vector2 mtv
        )
        {
            this.self = self;
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
        private Rectangle transform;                         // Collider transform: contains the position, scaled height and width
        private Vector2 position;                            // Position of the Collider
        private Color[] pixels;                              // Sprite pixel information
        private Func<Color, Color, Color, bool> pixelCheck;  // The pixel check function
        private Color clearColor;                            // Clear color for the sprite render
        private ushort layer;                                // Collider layer
        private bool resolved;                               // If the Collider collision is resolved
        private Vector2 colldierCurrentMTV;
        private Vector2 colliderNextMTV;
        public GameObject self;
        private Vector2 scale;
        private SpriteEffects? spriteEffects;
        public RenderTarget2D targetSprite;
        public Texture2D copyOfTarget;

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

        public Vector2 Scale
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
            position = Vector2.Zero;
            transform = new Rectangle(0, 0, 0, 0);
            pixels = null;
            pixelCheck = null;
            clearColor = Color.Transparent;
            layer = 0;
            resolved = false;
            spriteEffects = null;
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
            ushort layer,
            ushort instanceNumber
        )
        {
            SpriteEffects effect = SpriteEffects.None;
            Texture2D sprite = self.GetSprite(instanceNumber);
            Vector2 position = self.GetPosition(instanceNumber);
            Rectangle transform = self.GetTransform(instanceNumber);
            float scale = self.GetScale(instanceNumber);
            GraphicsDevice graphicsDevice = GameObject.Instance.GraphicsDevice;
            SpriteBatch spriteBatch = GameObject.Instance.SpriteBatch;

            return UpdateTransform(
                sprite,
                position,
                transform,
                graphicsDevice,
                spriteBatch,
                new Vector2(scale, scale),
                effect,
                layer,
                instanceNumber
            );
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
            ushort layer,
            GameObject self
        )
        {
            this.self = self;
            // Set the transform
            this.transform = transform;
            this.position = position;
            this.layer = layer;
            resolved = false;
            pixels = null;
            pixelCheck = null;
            Scale = new Vector2(scale, scale);

            if (graphicsDevice == null || graphicsDevice.IsDisposed || graphicsDevice.GraphicsDeviceStatus != GraphicsDeviceStatus.Normal)
            {
                // The graphics device is not ready, so don't try to render anything.
                return;
            }

            /*

            // Render the effects and scale
            targetSprite = new RenderTarget2D(
                graphicsDevice,
                (int)Math.Round((transform.Width * scale), MidpointRounding.AwayFromZero) <= 0 ? 1 : (int)Math.Round((transform.Width * scale), MidpointRounding.AwayFromZero),
                (int)Math.Round((transform.Height * scale), MidpointRounding.AwayFromZero) <= 0 ? 1 : (int)Math.Round((transform.Height * scale), MidpointRounding.AwayFromZero)
            );
            clearColor = Color.Transparent;
            graphicsDevice.SetRenderTarget(targetSprite);
            graphicsDevice.Clear(clearColor);
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.PointClamp);
            spriteBatch.Draw(
                sprite,
                Vector2.Zero,
                transform,
                Color.Red,
                0.0f,
                Vector2.Zero,
                scale,
                effect,
                1.0f
            );
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);

            copyOfTarget = new Texture2D(graphicsDevice, this.transform.Width, this.transform.Height);

            // Scale the transform
            // Load pixel data to CPU memory
            this.spriteEffects = null;
            Load(targetSprite);

            */
        }

        /// <summary>
        ///  Loads the pixel data from the GPU to the CPU memory
        /// </summary>
        public void Load(RenderTarget2D renderTarget2D)
        {
            if (!renderTarget2D.IsContentLost && !GameObject.Instance.HoldCollider)
            {
                if (copyOfTarget == null || copyOfTarget.Width != renderTarget2D.Width || copyOfTarget.Height != renderTarget2D.Height)
                {
                    copyOfTarget = new Texture2D(GameObject.Instance.GraphicsDevice, renderTarget2D.Width, renderTarget2D.Height);
                }
                if (pixels == null || pixels.Length != (renderTarget2D.Width * renderTarget2D.Height))
                {
                    pixels = new Color[renderTarget2D.Width * renderTarget2D.Height];
                    renderTarget2D.GetData(pixels);
                }
            }
            else
            {
                GameObject.Instance.HoldCollider = true;
            }
            renderTarget2D.Dispose();
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
            return UpdateTransform(
                sprite,
                position,
                transform,
                graphicsDevice,
                spriteBatch,
                new Vector2(scale, scale),
                effect,
                layer,
                instanceNumber
            );
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
            Vector2 scale,
            SpriteEffects effect,
            ushort layer,
            ushort instanceNumber
        )
        {
            return UpdateTransform(
            sprite,
            position,
            transform,
            graphicsDevice,
            spriteBatch,
            0,
            scale,
            effect,
            layer,
            instanceNumber
            );
        }


        public List<Collision> UpdateTransform(
            Texture2D sprite,
            Vector2 position,
            Rectangle transform,
            GraphicsDevice graphicsDevice,
            SpriteBatch spriteBatch,
            float direction,
            Vector2 scale,
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
            Scale = scale;

            // Create transform from scale and position
            this.transform = new Rectangle(
                (int)(Math.Round(position.X, MidpointRounding.AwayFromZero)),
                (int)(Math.Round(position.Y, MidpointRounding.AwayFromZero)),
                (int)Math.Round((transform.Width * (Scale.X)), MidpointRounding.AwayFromZero),
                (int)Math.Round((transform.Height * (Scale.Y)), MidpointRounding.AwayFromZero)
            );

            // Determine the bounds of the collider
            this.position = position;
            if (position.X - transform.Width * scale.Y > GameObject.Instance.GraphicsDevice.Viewport.Width ||
                position.X + transform.Width * scale.Y < 0 ||
                position.Y - transform.Height * scale.Y > GameObject.Instance.GraphicsDevice.Viewport.Height ||
                position.Y + transform.Height * scale.Y < -1
            )
            {
                //pixels = null;
                if (targetSprite != null)
                {
                    targetSprite.Dispose();
                }
                return new List<Collision>();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C) && copyOfTarget != null)
            {
                GameObject.Instance.Debug += delegate (SpriteBatch spriteBatch)
                {
                    spriteBatch.Draw(
                        copyOfTarget,
                        new Vector2(position.X, position.Y),
                        new Rectangle(0,0,this.transform.Width, this.transform.Height),
                        Color.Blue,
                        0.0f,
                        Vector2.Zero,
                        1.0f,
                        SpriteEffects.None,
                        1.0f
                    );
                };
            }

            List<Collision> result = new List<Collision>();
            ushort layer4 = GameObject.Instance.ColliderLayer4Instance(instanceNumber);
            unsafe
            {
                ref List<Collider> collidersR = ref GameObject.Instance.ColliderCollisions();
                // Go through all collider instances to check for a collision and determine what colliders are active
                for (ushort colliderIndex = 0; colliderIndex < collidersR.Count; colliderIndex++)
                {
                    // The other collider
                    Collider otherCollider = collidersR[colliderIndex];

                    // Determine if the colldier exist as a instance of this content
                    if (layer4 != colliderIndex && collidersR[colliderIndex] != null)
                    {
                        // When the bounds are intercepting and the layer isn't the same and all collisions have been resolved
                        // Then we can activate the collider
                        if (active || this.resolved &&
                            otherCollider.layer != this.layer &&
                            (otherCollider.layer != (ushort)CollisionGroup.Tile || ((ushort)CollisionGroup.Enemy != otherCollider.layer && this.layer != (ushort)CollisionGroup.Tile)
                            ) &&
                            collidersR[colliderIndex].transform.Intersects(this.transform)
                        )
                        {
                            //Debug.WriteLineIf(this.layer == (ushort)CollisionGroup.FromPlayer, "AAA");
                            active = true;
                        }
                        else
                        {
                            active = false;
                        }

                        // Only update the collider once
                        if (active && !updated )
                        {
                            updated = true;

                            // Load pixel data to CPU memory
                            if (!GameObject.Instance.HoldCollider && (pixels == null || spriteEffects != effect))
                            {
                                
                                // Setup the renderTarget
                                targetSprite = new RenderTarget2D(graphicsDevice,
                                    (int)Math.Round((transform.Width * (Scale.X)), MidpointRounding.AwayFromZero) <= 0 ? 1 : (int)Math.Round((transform.Width * (Scale.X)), MidpointRounding.AwayFromZero),
                                    (int)Math.Round((transform.Height * (Scale.Y)), MidpointRounding.AwayFromZero) <= 0 ? 1 : (int)Math.Round((transform.Height * (Scale.Y)), MidpointRounding.AwayFromZero),
                                    true,
                                    SurfaceFormat.Alpha8,
                                    DepthFormat.None,
                                    0,
                                    RenderTargetUsage.PlatformContents
                                );

                                // Render the new sprite 
                                graphicsDevice.SetRenderTarget(targetSprite);
                                graphicsDevice.Clear(clearColor);
                                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, null, RasterizerState.CullNone, null, null);
                                spriteBatch.Draw(
                                    sprite,
                                    new Vector2(0, 0),
                                    transform,
                                    Color.Blue,
                                    0.0f,
                                    Vector2.Zero,
                                    Scale,
                                    effect,
                                    1.0f
                                );

                                spriteBatch.End();
                                graphicsDevice.SetRenderTarget(null);
                                this.colliderNextMTV = Vector2.Zero;
                                this.colldierCurrentMTV = Vector2.Zero;

                                spriteEffects = effect;

                                // Update the transform with the new scale and sprite
                                Load(targetSprite);

                                if (pixels != null && copyOfTarget != null)
                                {
                                    copyOfTarget.SetData(pixels);
                                }

                                GameObject.Instance.HoldCollider = false;
                            }
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
                                result.Add(new Collision(
                                    self,
                                    collidersR[colliderIndex].self,
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
            if (this.pixels == null || other.pixels == null)
            {
                // Exit with off-screen positions
                return new Vector2[] { new Vector2(-1, -1), new Vector2(-1, -1), new Vector2(-1, -1) };
            }

            // Calculate the intersecting rectangle
            int x1 = Math.Max(this.transform.X, other.transform.X);
            int x2 = Math.Min(this.transform.X + (int)(this.transform.Width), other.transform.X + (int)(other.transform.Width));
            int y1 = Math.Max(this.transform.Y, other.transform.Y);
            int y2 = Math.Min(this.transform.Y + (int)(this.transform.Height), other.transform.Y + (int)(other.transform.Height));
            // x1 = x1 - (x2 - x1);
            // y1 = y1 - (y2 - y1);

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                Rectangle intercpetion = new Rectangle(0, 0, x2 - x1, y2 - y1);
                Rectangle hitBox = new Rectangle(0, 0, transform.Width, transform.Height);

                Texture2D pixelWhite = GameObject.Instance.ContentManager.Load<Texture2D>("white_pixel_strip1");
                GameObject.Instance.Debug += delegate (SpriteBatch spriteBatch)
                {
                    /*
                    spriteBatch.Draw(
                        pixelWhite,
                        new Vector2(transform.X, transform.Y),
                        hitBox,
                        new Color(Color.DarkBlue, 0.001f),
                        0, Vector2.Zero,
                        new Vector2(1, 1),
                        SpriteEffects.None,
                        0
                    );
                    /*
                    spriteBatch.Draw(
                        pixelWhite,
                        new Vector2(x1, y1),
                        intercpetion,
                        new Color(Color.Red, 0.1f),
                        0, Vector2.Zero,
                        new Vector2(1, 1),
                        SpriteEffects.None,
                        0
                    );
                    */
                };
            }

            // For each single pixel in the intersecting rectangle
            for (int y = y2 - 1; y >= y1; --y)
            {
                for (int x = x2 - 1; x >= x1; --x)
                {
                    // Get the color from each texture
                    int xIndex1 = (int)((x - this.transform.X));
                    int yIndex1 = (int)((y - this.transform.Y));
                    int index1 = xIndex1 + yIndex1 * this.transform.Width;

                    int xIndex2 = (int)((x - other.transform.X));
                    int yIndex2 = (int)((y - other.transform.Y));
                    int index2 = xIndex2 + yIndex2 * other.transform.Width;

                    if (index1 >= 0 && index1 < this.pixels.Length && index2 >= 0 && index2 < other.pixels.Length)
                    {
                        Color a1 = this.pixels[index1];
                        Color b1 = other.pixels[index2];

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
                                colldierCurrentMTV
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
            Vector2 overlap = new Vector2((float)Math.Sqrt((x2 - x1) * (x2 - x1)), (float)Math.Sqrt((y2 - y1) * (y2 - y1)));

            float ocx = other.position.X + other.Transform.Width / 2.0f;
            float ocy = other.position.Y + other.Transform.Height / 2.0f;

            bool xComparison = ((other.position.X + other.transform.Width * 0.5f) > (this.position.X + this.transform.Width * 0.5f));
            bool yComparison = ((other.position.Y + other.transform.Height * 0.5f) > (this.position.Y + this.transform.Height * 0.5f));
            bool xDiff = ((other.position.X + other.transform.Width * 0.5f) > (this.position.X + this.transform.Width * 0.5f));
            bool yDiff = ((other.position.Y + other.transform.Height * 0.5f) > (this.position.Y + this.transform.Height * 0.5f));

            mtv.X = ocx - x;
            mtv.Y = ocy - y;

            float preMTVY = mtv.Y;

            /*
            if (xComparison && !yComparison)
            {
                mtv.X = Math.Abs(mtv.X);
                mtv.Y = 0;
                // X +
                //Debug.WriteLine("A");
            }
            else if (!xComparison && yComparison)
            {
                // Y and Y +
                mtv.X = 0;
                mtv.Y = Math.Abs(mtv.Y);
                //Debug.WriteLine("B");
            }
            else if (!xComparison && !yComparison)
            {
                mtv.X = -Math.Abs(mtv.X);
                mtv.Y = -Math.Abs(mtv.Y);
                //Debug.WriteLine("C");
            }
            else if (!xComparison && !yComparison)
            {
                //Debug.WriteLine("D");
            }
            else
            {
                // Y and Y -

                //Debug.WriteLine("D");
                if (Math.Abs(overlap.Y) > Math.Abs(overlap.X))
                {
                    mtv.X = -Math.Abs(mtv.X);
                    mtv.Y = 0;
                }
                else if (!yComparison)
                {
                    mtv.X = 0;
                    mtv.Y = -Math.Abs(mtv.Y);
                }
            }
            */

      
            /*
            if (xComparison)
            {
                mtv.X = Math.Abs(mtv.X);
            }

            if (yComparison)
            {
                mtv.Y = Math.Abs(mtv.Y);
            }

            if (!xComparison)
            {
                mtv.X = -Math.Abs(mtv.X);
            }

            if (!yComparison)
            {
                mtv.Y = -Math.Abs(mtv.Y);
            }
            */

            if (mtv == Vector2.Zero)
            {
                return Vector2.Zero;
            }

            mtv = Vector2.Normalize(mtv) * Player.PlayerInstance.Speed;

            return mtv;
        }
    }
}
