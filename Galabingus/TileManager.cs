using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;


namespace Galabingus
{
    public sealed class TileManager
    {
        private static TileManager instance = null;

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

        // -------------------------------------------------
        // Fields
        // -------------------------------------------------
        
        private Vector2 screenSize;
        private Vector2 tileSize;
        private List<Tile> tileList;
        private List<Tile> borderList;
        private List<Tile> backgroundList;
        private List<Tile> tilesBorder;
        private List<ushort> layers;
        private List<ushort> spriteNumbers;
        private ushort currentSpriteNumber;
        private ushort tileInstance;  

        private int counter;
        private bool turn = false;

        // -------------------------------------------------
        // Properties
        // -------------------------------------------------

        public ushort LayerNumber
        {
            get { return layers[spriteNumbers[currentSpriteNumber]]; }
        }


        public ushort CurrentSpriteNumber
        {
            get { return currentSpriteNumber; }
        }

        // -------------------------------------------------
        // Contructors
        // -------------------------------------------------

        public TileManager()
        {
            screenSize = new Vector2(
            GameObject.Instance.GraphicsDevice.Viewport.Width,
            GameObject.Instance.GraphicsDevice.Viewport.Height);

            //ushort contentName = GameObject.Instance.Content.tile_strip26;

            //Tile tile = new Tile(contentName, 0, 0);
            //tileSize = new Vector2 (tile.Transform.Width, tile.Transform.Height);


            layers = new List<ushort>();
            spriteNumbers = new List<ushort>();
            tileList = new List<Tile>();
            borderList = new List<Tile>();
            backgroundList = new List<Tile>();
            tileInstance = 1000;

            // temp counter for scroll
            counter = 0;
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        /// <summary>
        /// Creates borders
        /// </summary>
        /// <param name="spriteNumber"></param>
        public void CreateTile(ushort spriteNumber)
        {
            ushort index = 0;

            switch (spriteNumber)
            {
                case 0:
                    layers.Add(Player.PlayerInstance.ContentName);
                    break;

                case 1:
                    layers.Add(Tile.Instance.Index);
                    break;

                default:
                    break;
            }

            index = (ushort)(layers.Count - 1);

            switch (spriteNumber)
            {
                case 0:
                    spriteNumbers.Add(index);
                    break;

                case 1:
                    spriteNumbers.Add(index);
                    break;

                default:
                    break;
            }

            /*
            // Top
            currentSpriteNumber = (index);
            Tile tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 0, index, true);

            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(screenSize.X, 200);
            tile.Position = new Vector2(0, -200);
            borderList.Add(tile);

            // Bot
            tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 1, index, true);
            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(screenSize.X, 200);
            tile.Position = new Vector2(0, screenSize.Y);
            borderList.Add(tile);


            // Right
            tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 2, index, true);
            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(200, screenSize.Y);
            tile.Position = new Vector2(screenSize.X, 0);
            borderList.Add(tile);

            // Left
            tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 3, index, true);
            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(200, screenSize.Y);
            tile.Position = new Vector2(-200, 0);
            borderList.Add(tile);
            */

        }

        /// <summary>
        /// Creates the background
        /// </summary>
        public void CreateBackground()
        {
            Tile background = new Tile(GameObject.Instance.Content.space_only_background_strip1, 0, 1, true);
            
            background.Transform = new Rectangle(0, 0, background.Sprite.Width, background.Sprite.Height);
          
            background.Scale = GameObject.Instance.GraphicsDevice.Viewport.Height / background.Sprite.Width / (Player.PlayerInstance.Scale * 0.975f);
            background.ScaleVector = new Vector2(background.Scale, background.Scale);
            background.Position = new Vector2(-GameObject.Instance.GraphicsDevice.Viewport.Width * 2, -GameObject.Instance.GraphicsDevice.Viewport.Height * 4f);
            background.Position -= new Vector2(GameObject.Instance.GraphicsDevice.Viewport.Width, 0);
            background.Effect = GameObject.Instance.ContentManager.Load<Effect>("background");
            background.Collider.Unload();
            backgroundList.Add(background);

            Tile background2 = new Tile(GameObject.Instance.Content.space_only_background_strip1, 1, 1, true);
            background2.Position = Vector2.Zero;
            
            background2.Transform = new Rectangle(0, 0, background.Sprite.Width, background.Sprite.Height);
            background2.Scale = GameObject.Instance.GraphicsDevice.Viewport.Height / background.Sprite.Width / (Player.PlayerInstance.Scale * 0.975f);
            background2.ScaleVector = new Vector2(background.Scale, background.Scale);
            background2.Collider.Unload();
            backgroundList.Add(background2);
        }

        /// <summary>
        /// Creates asteriod objects
        /// </summary>
        /// <param name="position"> The position of the asteriod </param>
        public void CreateObject(dynamic content, Vector2 position)
        {
            Tile tile = new Tile(content, tileInstance, 1, true);
            tile.Position = position;
            tile.ScaleVector = new Vector2(tile.Scale, tile.Scale);
            tileList.Add(tile);
            tileInstance++;
        }

        /// <summary>
        /// Creates asteriod objects
        /// </summary>
        /// <param name="position"> The position of the asteriod </param>
        public void CreateObject(dynamic content, Vector2 position, ushort spriteNumber)
        {
            Tile tile = new Tile(content, tileInstance, spriteNumber);
            tile.Position = position;
            tile.ScaleVector = new Vector2(tile.Scale, tile.Scale);
            tileList.Add(tile);
            tileInstance++;
        }


        public void Update(GameTime gameTime)
        {
            #region Border Update
            for (int i = 0; i < borderList.Count; i++)
            {
                //currentSpriteNumber = tilesList[i].SpriteNumber;

                if (borderList[i].IsActive)
                {
                    borderList[i].Collider.Resolved = true;
                    List<Collision> collisions = borderList[i].Collider.UpdateTransform(
                        borderList[i].Sprite,
                        borderList[i].Position,
                        borderList[i].Transform,
                        GameObject.Instance.GraphicsDevice,
                        GameObject.Instance.SpriteBatch,
                        borderList[i].ScaleVector,
                        SpriteEffects.None,
                        (ushort)CollisionGroup.Tile,
                        borderList[i].InstanceNumber
                    );

                    foreach (Collision collision in collisions)
                    {
                        if (collision.other != null)
                        {
                            if (((collision.other as Player) is Player) && collision.self is Tile)
                            {
                                Player.PlayerInstance.Position += collision.mtv;
                                Player.PlayerInstance.Collider.Resolved = true;
                            }
                        }
                    }
                    borderList[i].Collider.Resolved = true;
                }
                else
                {
                    if (i < borderList.Count && borderList[i] != null && borderList[i].Collider != null)
                    {
                        borderList[i].Collider.Resolved = true;
                        borderList[i].Collider.Unload();
                    }
                }
            }
            #endregion

            #region Object Update
            for (int i = 0; i < tileList.Count; i++)
            {
                if (tileList[i].IsActive)
                {
                    tileList[i].Collider.Resolved = true;
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

                    foreach (Collision collision in collisions)
                    {
                        if (collision.other != null)
                        {
                            if (((collision.other as Player) is Player) && collision.self is Tile)
                            {
                                Player.PlayerInstance.Position += collision.mtv;
                                Player.PlayerInstance.Collider.Resolved = true;
                            }
                        }
                    }
                    tileList[i].Collider.Resolved = true;
                    tileList[i].Update(gameTime);
                }
                else
                {
                    if (i < borderList.Count && borderList[i] != null && borderList[i].Collider != null)
                    {
                        borderList[i].Collider.Resolved = true;
                        borderList[i].Collider.Unload();
                    }
                }
            }
            #endregion

            #region Background Scroll
            for (int i = 0; i < backgroundList.Count; i++)
            {
                backgroundList[i].UpdateBackground(gameTime);
            }

            // Scroll the camrea when the enemies are not on the screeen
            if (EnemyManager.Instance.EnemiesOnScreen == 0)
            {
                if (counter != 4)
                {
                    Camera.Instance.Start();
                }
                if (turn == false)
                {
                    if (backgroundList[1].Position.Y >= GameObject.Instance.GraphicsDevice.Viewport.Height)
                    {
                        if (counter == 3)
                        {
                            Player.PlayerInstance.CameraLock = false;
                            Camera.Instance.Reverse();
                            turn = true;
                        }

                        counter++;
                        backgroundList[1].Position = new Vector2(
                            0, backgroundList[1].Position.Y - GameObject.Instance.GraphicsDevice.Viewport.Height * 4
                        );
                    }
                }
            }
            #endregion

            // Stop the camera at the different camera stops
            if (Camera.Instance.CameraLock == false)
            {
                bool stopHit = false;
                int indexOfStop = 0;
                foreach (Vector2 position in GameObject.Instance.GetCameraStopPositions())
                {
                    if ((position.Y) >= Math.Floor(Camera.Instance.Position.Y) && !GameObject.Instance.UniversalShader.Parameters["bossEffect"].GetValueBoolean())
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
        }

        public void Draw()
        {
            backgroundList[0].Draw(
                GameObject.Instance.GraphicsDevice.Viewport.Width * 4f,
                GameObject.Instance.GraphicsDevice.Viewport.Height * 4f

            );

            for (int i = 0; i < borderList.Count; i++)
            {
                if (borderList[i].IsActive)
                {
                    borderList[i].Draw();
                }
            }

            for (int i = 0; i < tileList.Count; i++)
            {
                if (tileList[i].IsActive)
                {
                    tileList[i].Draw();
                }
            }
        }
        public void Reset()
        {
            instance = null;
        }

    }
}
