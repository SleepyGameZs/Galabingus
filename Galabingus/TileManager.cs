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
        private List<Tile> tilesList;
        private List<Tile> backgroundList;
        private List<Tile> tilesBorder;
        private List<ushort> layers;
        private List<ushort> spriteNumbers;
        private ushort currentSpriteNumber;

        private int counter;

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

            tilesList = new List<Tile>();
            backgroundList = new List<Tile>();

            // temp counter for scroll
            counter = 0;
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

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

            // Top
            currentSpriteNumber = (index);
            Tile tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 0, index, true);
            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(screenSize.X, 20);
            tile.Position = new Vector2(0, -21);
            tilesList.Add(tile);

            // Bot
            tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 1, index, true);
            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(screenSize.X, 20);
            tile.Position = new Vector2(20, screenSize.Y);
            tilesList.Add(tile);

            // Right
            tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 2, index, true);
            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(20, screenSize.Y);
            tile.Position = new Vector2(screenSize.X, 0);
            tilesList.Add(tile);

            // Left
            tile = new Tile(GameObject.Instance.Content.white_pixel_strip1, 3, index, true);
            tile.Scale = 25f;
            tile.ScaleVector = new Vector2(20, screenSize.Y);
            tile.Position = new Vector2(-20, 0);
            tilesList.Add(tile);
        }

        public void CreateBackground()
        {
            Tile background = new Tile(GameObject.Instance.Content.spacebackground_strip1, 0, 1, true);
            background.Position = Vector2.Zero;
            background.Transform = new Rectangle(0, 0, background.Sprite.Width, background.Sprite.Height);
            background.Scale = 1f;
            background.ScaleVector = new Vector2(background.Scale, 1);
            backgroundList.Add(background);

            Tile background2 = new Tile(GameObject.Instance.Content.spacebackground_strip1, 1, 1, true);
            background2.Position = new Vector2(background.Transform.Width, 0);
            background2.Transform = new Rectangle(background.Transform.Width, 0, background2.Sprite.Width, background2.Sprite.Height);
            background2.Scale = 1f;
            background2.ScaleVector = new Vector2(background2.Scale, 1);
            backgroundList.Add(background2);
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < tilesList.Count; i++)
            {
                //currentSpriteNumber = tilesList[i].SpriteNumber;

                List<Collision> collisions = tilesList[i].Collider.UpdateTransform(
                    tilesList[i].Sprite,
                    tilesList[i].Position,
                    tilesList[i].Transform,
                    GameObject.Instance.GraphicsDevice,
                    GameObject.Instance.SpriteBatch,
                    tilesList[i].ScaleVector,
                    SpriteEffects.None,
                    (ushort)CollisionGroup.Tile,//GameObject.Instance.Content.tile_strip26,
                    tilesList[i].InstanceNumber
                    );

                foreach (Collision collision in collisions)
                {
                    if (collision.other != null)
                    {
                        if ( ((collision.other as Player) is Player) && collision.self is Tile )
                        {
                            Player.PlayerInstance.Position += collision.mtv;
                            Player.PlayerInstance.Collider.Resolved = true;
                        }
                    }
                }
                tilesList[i].Collider.Resolved = true;

            }
            // Background Scroll
            for (int i = 0; i < backgroundList.Count; i++)
            {
                backgroundList[i].Update(gameTime);
            }
            // Background Loop
            for (int i = 0; i < backgroundList.Count; i++)
            {
                if (backgroundList[i].Position.X == -backgroundList[i].Transform.Width)
                {
                    backgroundList[i].Position = new Vector2(backgroundList[i].Transform.Width, 0);
                    counter++;
                    Debug.WriteLine(counter);
                }
                else if (counter == 1) 
                {
                    Camera.Instance.Stop();
                }
            }
        }

        public void Draw()
        {
            for (int i = 0; i < tilesList.Count; i++)
            {
                tilesList[i].Draw();
            }
            for (int i = 0; i < backgroundList.Count; i++)
            {
                backgroundList[i].Draw();
            }
        }
    }
}
