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
        private Tile[,] tiles;


        // -------------------------------------------------
        // Properties
        // -------------------------------------------------


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

            tiles = new Tile[(int)(screenSize.Y / tileSize.Y), (int)(screenSize.X / tileSize.X)];
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        public void CreateTile()
        {
            ushort instanceCounter = 0;
            for (ushort i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    Tile tile = new Tile(GameObject.Instance.Content.tile_strip26, instanceCounter, 0);
                    //tile.Position = new Vector2(tile.Sprite.Width*j, 0);
                    tile.Position = new Vector2(0, 0);
                    tile.Transform = new Rectangle(
                        (int)tile.Position.X,
                        (int)tile.Position.Y,
                        tile.Transform.Width,
                        tile.Transform.Height
                    );
                    tiles[i, j] = tile;
                    instanceCounter++;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j].Update(gameTime);

                    List<Collision> collisions = tiles[i, j].Collider.UpdateTransform(
                        tiles[i,j].Sprite, 
                        tiles[i, j].Position, 
                        tiles[i,j].Transform, 
                        GameObject.Instance.GraphicsDevice, 
                        GameObject.Instance.SpriteBatch, 
                        tiles[i,j].Scale, 
                        SpriteEffects.None, 
                        GameObject.Instance.Content.tile_strip26, 
                        tiles[i,j].InstanceNumber
                    );
                    foreach (Collision collision in collisions)
                    {
                        if (collision.other != null)
                        {
                            if (collision.other.Index == Player.PlayerInstance.Index)
                            {
                                //Player.PlayerInstance.Position += collision.mtv;
                                
                                Player.PlayerInstance.Collider.Resolved = true;
                            }
                        }
                    }
                    tiles[i, j].Collider.Resolved = true;
                }
            }


        }

        public void Draw()
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j].Draw();
                }
            }
        }
    }
}
