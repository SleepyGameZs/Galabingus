using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Tile tile = new Tile(GameObject.Instance.Content.tile_strip26, 0, 0);
            tileSize = new Vector2 (tile.Sprite.Width, tile.Sprite.Height);

            //tileSize = new Vector2(GameObject.Instance.Content.tile_strip26,
                //GameObject.Instance.Content.tile_strip26.Sprite.Height);

            tiles = new Tile[(int)(screenSize.Y / tileSize.Y), (int)(screenSize.X / tileSize.X)];
        }

        // -------------------------------------------------
        // Meathods 
        // -------------------------------------------------

        public void CreateTile()
        {
            ushort instanceCounter = 1;
            for (ushort i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    Tile tile = new Tile(GameObject.Instance.Content.tile_strip26, instanceCounter, 0);
                    tile.Position = new Vector2(tile.Sprite.Width+100, tile.Sprite.Height+100);
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
