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
        private List<Tile> tilesBorder;
        private List<ushort> layers;
        private List<ushort> spriteNumbers;
        private ushort currentSpriteNumber;

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
            tile.Position = new Vector2(0, -20);
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
            ushort instanceCounter = 0;
            for (ushort i = 0; i < 100; i++)
            {
                for (ushort j = 0; j < 100; j++)
                {
                    Tile tile = new Tile(GameObject.Instance.Content.tile_strip26, instanceCounter, 0);
                    tile.Scale = 3f;
                    tile.ScaleVector = new Vector2(1, 1);
                    tile.Position = new Vector2(tile.Transform.Width * j, tile.Transform.Height* i);
                    tilesList.Add(tile);
                    instanceCounter++;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < tilesList.Count; i++)
            {
                //currentSpriteNumber = tilesList[i].SpriteNumber;

                tilesList[i].Update(gameTime);
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

        }

        public void Draw()
        {
            for (int i = 0; i < tilesList.Count; i++)
            {
                tilesList[i].Draw();
            }
        }
    }
}
