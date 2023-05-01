using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

/* The tile manager hold the information of all tile objects in the game,
 * inclduing both obstacle tiles and the background. Updating both the 
 * postion of tiles through the camera scroll and the destruction of tiles
 * by the boss and certain types of bullets. The manager also deals with
 * player collsions with the tiles on screen updating the players postion
 * after each collsion. Stop points in the level are also used in the 
 * manager to cause breaks in the camera scrolling */

namespace Galabingus
{
    public sealed class TileManager
    {
        #region Fields 

        // The instance of the tile manager
        private static TileManager instance = null;

        // The size of the game window
        private Vector2 screenSize;

        // The size of a tile
        private Vector2 tileSize;

        // Lists for the different tiles 
        private List<Tile> tileList;
        private List<Tile> backgroundList;
        private List<Tile> tilesBorder;

        // List for tile layers
        private List<ushort> layers;

        // Sprite numbers for drawing from sprtie sheets
        private List<ushort> spriteNumbers;
        private ushort currentSpriteNumber;
        private ushort tileInstance;  

        // If the camera has turned around
        private bool turn = false;

        #endregion

        #region Properties 

        /// <summary>
        /// Reference to the tile manager
        /// </summary>
        public static TileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TileManager();
                }
                return instance;
            }

        }

        /// <summary>
        /// Current sprite number of the sprite sheet
        /// </summary>
        public ushort CurrentSpriteNumber
        {
            get { return currentSpriteNumber; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates tile lists and necessary reference varibles 
        /// </summary>
        public TileManager()
        {
            // Copying of screen size into varible
            screenSize = new Vector2(
            GameObject.Instance.GraphicsDevice.Viewport.Width,
            GameObject.Instance.GraphicsDevice.Viewport.Height);

            // Instantiation of tile lists
            layers = new List<ushort>();
            spriteNumbers = new List<ushort>();
            tileList = new List<Tile>();
            backgroundList = new List<Tile>();

            // Increasing of instance number to make sure there are no overlaping numbers
            tileInstance = 1000;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the background
        /// </summary>
        public void CreateBackground()
        {
            #region Background 
            // Creation of first background image
            Tile background = new Tile(GameObject.Instance.Content.space_only_background_strip1, 0, 1, true);
            
            // Assigning of tile parameters to the background image
            background.Transform = new Rectangle(0, 0, background.Sprite.Width, background.Sprite.Height);

            background.Scale = GameObject.Instance.GraphicsDevice.Viewport.Height / background.Sprite.Width / (Player.PlayerInstance.Scale * 0.575f);
            background.ScaleVector = new Vector2(background.Scale, background.Scale);

            background.Position = new Vector2(-GameObject.Instance.GraphicsDevice.Viewport.Width * 2, -GameObject.Instance.GraphicsDevice.Viewport.Height * 5f);
            background.Position -= new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Width, 0);

            background.Effect = GameObject.Instance.ContentManager.Load<Effect>("background");

            background.Collider.Unload();

            backgroundList.Add(background);
            #endregion

            #region Spare Background
            // Creation of second background image
            Tile spareBackground = new Tile(GameObject.Instance.Content.space_only_background_strip1, 1, 1, true);

            // Assigning of tile parameters to the second background image
            spareBackground.Position = Vector2.Zero;

            spareBackground.Transform = new Rectangle(0, 0, background.Sprite.Width, background.Sprite.Height);

            spareBackground.Scale = GameObject.Instance.GraphicsDevice.Viewport.Height / background.Sprite.Width / (Player.PlayerInstance.Scale * 0.975f);
            spareBackground.ScaleVector = new Vector2(background.Scale, background.Scale);

            spareBackground.Collider.Unload();

            backgroundList.Add(spareBackground);
            #endregion
        }

        /// <summary>
        /// Creates tile objects
        /// </summary>
        /// <param name="position"> The position of the asteriod </param>
        public void CreateObject(dynamic content, Vector2 position, ushort spriteNumber)
        {
            Tile tile = new Tile(content, tileInstance, spriteNumber);
            tile.Position = position;
            tileList.Add(tile);
            tileInstance++;
        }

        /// <summary>
        /// Updates tiles based on camera scroll and dectets collsions for bullets and the player
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            #region Object Update
            for (int i = 0; i < tileList.Count; i++)
            {
                // Check to see if tile is active
                if (tileList[i].IsActive)
                {
                    // Update colider
                    tileList[i].Collider.Resolved = true;

                    // Update the transform of the object
                    List<Collision> collisions = tileList[i].Collider.UpdateTransform(
                        tileList[i].Sprite,
                        tileList[i].Position,
                        tileList[i].Transform,
                        GameObject.Instance.GraphicsDevice,
                        GameObject.Instance.SpriteBatch,
                        tileList[i].ScaleVector,
                        SpriteEffects.None,
                        (ushort)CollisionGroup.Tile,
                        tileList[i].InstanceNumber
                    );

                    // Ignore collsion if in god mode
                    if (!Player.PlayerInstance.GodMode)
                    {
                        foreach (Collision collision in collisions)
                        {
                            // Check for player and tile collsion
                            if (collision.other != null)
                            {
                                if ((collision.other as Player is Player) && collision.self is Tile)
                                {
                                    // Create two rectangels to represent hitboxes
                                    Rectangle playerHitbox = Player.PlayerInstance.Transform;
                                    Rectangle otherHitbox = tileList[i].Transform;
                                    
                                    // List of intersecting hitboxes
                                    List<Rectangle> intersecting = new List<Rectangle>();

                                    // Parseing of hitbox X and Ys
                                    playerHitbox.X = (int)Player.PlayerInstance.Position.X;
                                    playerHitbox.Y = (int)Player.PlayerInstance.Position.Y;
                                    otherHitbox.X = (int)tileList[i].Position.X;
                                    otherHitbox.Y = (int)tileList[i].Position.Y;

                                    // Multiplying of hitboxes by scale varibles 
                                    playerHitbox.Width = playerHitbox.Width * (int)Player.PlayerInstance.Scale;
                                    playerHitbox.Height = playerHitbox.Height * (int)Player.PlayerInstance.Scale;
                                    otherHitbox.Width = (int)tileList[i].ScaleVector.X * otherHitbox.Width;
                                    otherHitbox.Height = (int)tileList[i].ScaleVector.Y * otherHitbox.Height;

                                    // Check for any collisions
                                    if (playerHitbox.Intersects(otherHitbox))
                                    {
                                        intersecting.Add(otherHitbox);
                                    }

                                    // Adjust player position based on the player intersection
                                    foreach (Rectangle brick in intersecting)
                                    {
                                        Rectangle collisionBox = Rectangle.Intersect(playerHitbox, brick);

                                        // Y adjustment
                                        if (collisionBox.Width > collisionBox.Height)
                                        {
                                            if (playerHitbox.Y < brick.Y)
                                            {   
                                                // Send player down
                                                Player.PlayerInstance.Position -= new Vector2(0, collisionBox.Height);
                                                Player.PlayerInstance.Translation = new Vector2(Player.PlayerInstance.Translation.X, 0);
                                            }
                                            else
                                            {
                                                // Send player up
                                                Player.PlayerInstance.Position += new Vector2(0, collisionBox.Height);
                                                Player.PlayerInstance.Translation = new Vector2(Player.PlayerInstance.Translation.X, 0);
                                            }
                                        }
                                        // X adjustment
                                        else
                                        {
                                            if (playerHitbox.X < brick.X)
                                            {
                                                // Send player left
                                                Player.PlayerInstance.Position -= new Vector2(collisionBox.Width, 0);
                                                Player.PlayerInstance.Translation = new Vector2(0, Player.PlayerInstance.Translation.Y);
                                            }
                                            else
                                            {
                                                // Send player left
                                                Player.PlayerInstance.Position += new Vector2(collisionBox.Width, 0);
                                                Player.PlayerInstance.Translation = new Vector2(0, Player.PlayerInstance.Translation.Y);
                                            }
                                        }

                                        // Update player position
                                        Player.PlayerInstance.Translation = new Vector2(playerHitbox.X, Player.PlayerInstance.Translation.Y);
                                        Player.PlayerInstance.Translation = new Vector2(Player.PlayerInstance.Translation.X, playerHitbox.Y);
                                        Player.PlayerInstance.Collider.Resolved = true;
                                    }
                                }
                            }
                        }
                    }
                    // Update the tile
                    tileList[i].Collider.Resolved = true;
                    tileList[i].Update(gameTime);
                }
            }
            #endregion

            #region Background Scroll

            // Scroll the two backgrounds
            for (int i = 0; i < backgroundList.Count; i++)
            {
                backgroundList[i].UpdateBackground(gameTime);
            }

            // If the camera has not turned
            if (turn == false)
            {
                // Reverse camera when reaching the end of the level
                if (Camera.Instance.Position.Y <= (GameObject.EndPosition.Y * 1.5f - 150))
                {
                    Camera.Instance.Start();
                    Camera.Instance.Reverse();

                    // Update bools 
                    Player.PlayerInstance.CameraLock = true;
                    turn = true;
                }
            }

            // Scroll the camrea when the enemies are not on the screeen
            if (EnemyManager.Instance.EnemiesOnScreen == 0 && turn == false)
            {
                Camera.Instance.Start();
            }
            else if (!(Camera.Instance.Position.Y >= 0) && EnemyManager.Instance.EnemiesOnScreen == 0 && Camera.Instance.OffSet.Y == 0)
            {
                Camera.Instance.Start();
            }


            #endregion

            #region Camera stopping

            // Stop the camera at the different camera stops
            if (Camera.Instance.CameraLock == false)
            {
                bool stopHit = false;
                int indexOfStop = 0;

                // Importing of camera stop points from file
                foreach (Vector2 position in GameObject.Instance.GetCameraStopPositions())
                {
                    // Stopping of camera at set stop points using Y of stop point
                    if (position.Y >= Math.Floor(Camera.Instance.Position.Y) && !GameObject.Instance.UniversalShader.Parameters["bossEffect"].GetValueBoolean())
                    {
                        Camera.Instance.OffSet = Vector2.Zero;
                        Camera.Instance.Stop();
                        stopHit = true;
                        break;
                    }
                    else if (!stopHit)
                    {
                        indexOfStop++;
                    }
                    else
                    {
                        continue;
                    }
                }
                // Removing of stop point after activation
                if (stopHit)
                {
                    GameObject.Instance.CameraStopRemoveAt(indexOfStop);
                }
            }

            // Stop the camera at the end on the way back
            if (turn && (Camera.Instance.Position.Y) >= 0)
            {
                Camera.Instance.Stop();
            }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            // Drawing of background
            backgroundList[0].Draw(
                GameObject.Instance.GraphicsDevice.Viewport.Width * 5f,
                GameObject.Instance.GraphicsDevice.Viewport.Height * 5f
            );

            // Drawing of tiles
            for (int i = 0; i < tileList.Count; i++)
            {
                if (tileList[i].IsActive)
                {
                    tileList[i].Draw();
                }
            }
        }

        /// <summary>
        /// Resets the singelton
        /// </summary>
        public void Reset()
        {
            instance = null;
        }

        #endregion
    }
}
