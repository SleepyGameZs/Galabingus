using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galabingus_Map_Editor
{
    enum TileState 
    {
        Empty,
        //Player
        Player,
        //Enemies
        Enemy1,
        Enemy2,
        Enemy3,
        Enemy4,
        Enemy5,
        Enemy6,
        Enemy7,
        Enemy8,
        Enemy9,
        Boss,
        //Tiles
        rock1,
        rock2,
        rock3,
        rock4,
        rock5,
        rock6,
        rock7,
        rock8,
        rock9,
        rock10,
        rock11,
        rock12,
        rock13,
        rock14,
        rock15,
        rock16,
        rock17,
        rock18,
        rock19,
        rock20,
        rock21,
        rock22,
        rock23,
        rock24,
        rock25,
    }
    public partial class MapEditorScreen : Form
    {
        private List<PictureBox> boxes = new List<PictureBox>();

        private int totalWidth;
        private int totalHeight;
        private int totalDensity;
        private int totalPageNum;

        private int tileSize;

        private bool saved;

        private Dictionary<string ,Image> TileSet;

        private Image currentSelected;

        private List<Image[]> spritePageSelect;

        public MapEditorScreen(int numofPage, int pixelDensity)
        {
            InitializeComponent();

            TileSet = new Dictionary<string, Image>();

            totalWidth = 16;

            totalHeight = 9;

            tileSize = 60;

            totalDensity = pixelDensity;

            totalPageNum = numofPage;

            currentSelected = null;

            TileSizeDet();

            DicImageAdd();

            MapDraw();

        }

        public MapEditorScreen()
        {
            InitializeComponent();

            currentSelected = null;

            TileSizeDet();

            DicImageAdd();

            MapDraw();
        }

        //Button 1
        private void button2_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button2.BackgroundImage;
            currentSelected = button2.BackgroundImage;
        }

        //Button 2
        private void button7_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button7.BackgroundImage;
            currentSelected = button7.BackgroundImage;
        }

        //Button 3
        private void button3_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button3.BackgroundImage;
            currentSelected = button3.BackgroundImage;
        }

        //Button 4
        private void button8_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button8.BackgroundImage;
            currentSelected = button8.BackgroundImage;
        }

        //Button 5
        private void button4_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button4.BackgroundImage;
            currentSelected = button4.BackgroundImage;
        }

        //Button 6
        private void button9_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button9.BackgroundImage;
            currentSelected = button9.BackgroundImage;
        }

        //Button 7
        private void button5_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button5.BackgroundImage;
            currentSelected = button5.BackgroundImage;
        }

        //Button 8
        private void button10_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button10.BackgroundImage;
            currentSelected = button10.BackgroundImage;
        }

        //Button 9
        private void button6_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button6.BackgroundImage;
            currentSelected = button6.BackgroundImage;
        }

        //Button 10
        private void button11_Click(object sender, EventArgs e)
        {
            button1.BackgroundImage = button11.BackgroundImage;
            currentSelected = button11.BackgroundImage
        }

        //Save Button
        private void button14_Click(object sender, EventArgs e)
        {

        }

        //Load Button
        private void button15_Click(object sender, EventArgs e)
        {

        }

        //Left Change Selectable
        private void button13_Click(object sender, EventArgs e)
        {

        }

        //Right Change Selectable
        private void button12_Click(object sender, EventArgs e)
        {

        }


        private void ImageChanger(object tile, EventArgs click)
        {
            if (tile != null)
            {
                PictureBox pixel = (PictureBox)tile;
                pixel.Image = currentSelected;
                //changed = true;
            }
        }

        private void MapDraw()
        {
            for (int y = 0; y < totalHeight; y++)
            {
                for (int x = 0; x < totalWidth; x++)
                {
                    PictureBox tileBox = new PictureBox();

                    tileBox.Size = new Size(tileSize, tileSize);
                    tileBox.Location = new Point(305 + (tileSize * x), 30 + (tileSize * y));
                    tileBox.BackColor = Color.White ;

                    Controls.Add(tileBox);
                    boxes.Add(tileBox);
                    tileBox.BringToFront();
                    tileBox.Click += ImageChanger;
                    Debug.Write(x);
                    //tileBox.Click += Change;
                }
            }
        }

        private void DicImageAdd()
        {
            //Enemy Sprites
            TileSet.Add("dblue" ,Image.FromFile(@"../Resources/enemy_dblue_strip4-1.png"));
            TileSet.Add("green" ,Image.FromFile(@"../Resources/enemy_green_strip4-1.png"));
            TileSet.Add("lblue", Image.FromFile(@"../Resources/enemy_lblue_strip4-1.png"));
            TileSet.Add("orange", Image.FromFile(@"../Resources/enemy_orange_strip4-1.png"));
            TileSet.Add("pink", Image.FromFile(@"../Resources/enemy_pink_strip4-1.png"));
            TileSet.Add("purple", Image.FromFile(@"../Resources/enemy_purple_strip4-1.png"));
            TileSet.Add("red", Image.FromFile(@"../Resources/enemy_red_strip4-1.png"));
            TileSet.Add("yellow", Image.FromFile(@"../Resources/enemy_yellow_strip4-1.png"));

            //Boss Sprite
            //TileSet.Add("boss", Image.FromFile(@"../Resources/Boss image");

            //Player Sprite
            TileSet.Add("player" , Image.FromFile(@"../Resources/player_strip5-1.png"));

            //Asteroid Tiles
            TileSet.Add("tile_1", Image.FromFile(@"../Resources/tile_strip26-1.png"));
            TileSet.Add("tile_2", Image.FromFile(@"../Resources/tile_strip26-2.png"));
            TileSet.Add("tile_3", Image.FromFile(@"../Resources/tile_strip26-3.png"));
            TileSet.Add("tile_4", Image.FromFile(@"../Resources/tile_strip26-4.png"));
            TileSet.Add("tile_5", Image.FromFile(@"../Resources/tile_strip26-5.png"));
            TileSet.Add("tile_6", Image.FromFile(@"../Resources/tile_strip26-6.png"));
            TileSet.Add("tile_7", Image.FromFile(@"../Resources/tile_strip26-7.png"));
            TileSet.Add("tile_8", Image.FromFile(@"../Resources/tile_strip26-8.png"));
            TileSet.Add("tile_9", Image.FromFile(@"../Resources/tile_strip26-9.png"));
            TileSet.Add("tile_10", Image.FromFile(@"../Resources/tile_strip26-10.png"));
            TileSet.Add("tile_11", Image.FromFile(@"../Resources/tile_strip26-11.png"));
            TileSet.Add("tile_12", Image.FromFile(@"../Resources/tile_strip26-12.png"));
            TileSet.Add("tile_13", Image.FromFile(@"../Resources/tile_strip26-13.png"));
            TileSet.Add("tile_14", Image.FromFile(@"../Resources/tile_strip26-14.png"));
            TileSet.Add("tile_15", Image.FromFile(@"../Resources/tile_strip26-15.png"));
            TileSet.Add("tile_16", Image.FromFile(@"../Resources/tile_strip26-16.png"));
            TileSet.Add("tile_17", Image.FromFile(@"../Resources/tile_strip26-17.png"));
            TileSet.Add("tile_18", Image.FromFile(@"../Resources/tile_strip26-18.png"));
            TileSet.Add("tile_19", Image.FromFile(@"../Resources/tile_strip26-19.png"));
            TileSet.Add("tile_20", Image.FromFile(@"../Resources/tile_strip26-20.png"));
            TileSet.Add("tile_21", Image.FromFile(@"../Resources/tile_strip26-21.png"));
            TileSet.Add("tile_22", Image.FromFile(@"../Resources/tile_strip26-22.png"));
            TileSet.Add("tile_23", Image.FromFile(@"../Resources/tile_strip26-23.png"));
            TileSet.Add("tile_24", Image.FromFile(@"../Resources/tile_strip26-24.png"));
            TileSet.Add("tile_25", Image.FromFile(@"../Resources/tile_strip26-25.png"));
            TileSet.Add("tile_26", Image.FromFile(@"../Resources/tile_strip26-26.png"));

            spritePageSelect = new List<Image[]>;
        }

        private void TileSizeDet()
        {
            totalWidth = totalWidth * totalDensity;

            totalHeight = totalHeight * totalDensity;

            tileSize = (int) tileSize / totalDensity;
        }

        private void ChangeSelectable(int change) 
        {
            
        }

        private void ScaleUp()
        {

        }

        /*
        private Image ButtonImagesDisplay(Image image)
        {
            

        }
        */
    }
}
