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
            GraphicsDeviceManager.DefaultBackBufferWidth, 
            GraphicsDeviceManager.DefaultBackBufferHeight);

            ushort contentName = GameObject.Instance.Content.tile_strip26;
            Debug.WriteLine(contentName);

            Tile tile = new Tile(contentName, 0, 0);
            tileSize = new Vector2 (tile.Transform.Width, tile.Transform.Height);

            //tileSize = new Vector2(GameObject.Instance.Content.tile_strip26,
                //GameObject.Instance.Content.tile_strip26.Sprite.Height);

            layers = new List<ushort>();
            spriteNumbers = new List<ushort>();

            tilesList = new List<Tile>();
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        public void CreateTile(ushort spriteNumber)
        {
            ushort instanceCounter = 0;
            for (int i = 0; i < 100; i++)
            {
                Tile tile = new Tile(GameObject.Instance.Content.tile_strip26, instanceCounter, spriteNumber);
                tile.Position = new Vector2(tile.Transform.Width * tile.Scale * i, 0);
                

                switch (spriteNumber)
                {
                    case 0:
                        layers.Add(Player.PlayerInstance.ContentName);
                        spriteNumbers.Add(spriteNumber);
                        break;

                    case 1:
                        layers.Add(Tile.Instance.Index);
                        spriteNumbers.Add(spriteNumber);
                        break;

                    default:
                        break;
                }

                tilesList.Add(tile);
                instanceCounter++;
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < tilesList.Count; i++)
            {
                currentSpriteNumber = tilesList[i].SpriteNumber;

                tilesList[i].Update(gameTime);
                List<Collision> collisions = tilesList[i].Collider.UpdateTransform(
                    tilesList[i].Sprite,
                    tilesList[i].Position,
                    tilesList[i].Transform,
                    GameObject.Instance.GraphicsDevice,
                    GameObject.Instance.SpriteBatch,
                    tilesList[i].Scale,
                    SpriteEffects.None,
                    TileManager.Instance.LayerNumber,//GameObject.Instance.Content.tile_strip26,
                    tilesList[i].InstanceNumber
                    );

                foreach (Collision collision in collisions)
                {
                    if (collision.other != null)
                    {
                        if (collision.other.Index == Player.PlayerInstance.Index)
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
