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
            currentSelected = button11.BackgroundImage;
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
            TileSet.Add("dblue" ,Properties.Resources.enemy_dblue_strip4_1);
            TileSet.Add("green" ,Properties.Resources.enemy_green_strip4_1);
            TileSet.Add("lblue", Properties.Resources.enemy_lblue_strip4_1);
            TileSet.Add("orange", Properties.Resources.enemy_orange_strip4_1);
            TileSet.Add("pink", Properties.Resources.enemy_pink_strip4_1);
            TileSet.Add("purple", Properties.Resources.enemy_purple_strip4_1);
            TileSet.Add("red", Properties.Resources.enemy_red_strip4_1);
            TileSet.Add("yellow", Properties.Resources.enemy_yellow_strip4_1);

            //Boss Sprite
            //TileSet.Add("boss", Image.FromFile(@"../Resources/Boss image");

            //Player Sprite
            TileSet.Add("player" , Properties.Resources.player_strip5_1);

            //Asteroid Tiles
            TileSet.Add("tile_1", Properties.Resources.tile_strip26_1);
            TileSet.Add("tile_2", Properties.Resources.tile_strip26_2);
            TileSet.Add("tile_3", Properties.Resources.tile_strip26_3);
            TileSet.Add("tile_4", Properties.Resources.tile_strip26_4);
            TileSet.Add("tile_5", Properties.Resources.tile_strip26_5);
            TileSet.Add("tile_6", Properties.Resources.tile_strip26_6);
            TileSet.Add("tile_7", Properties.Resources.tile_strip26_7);
            TileSet.Add("tile_8", Properties.Resources.tile_strip26_8);
            TileSet.Add("tile_9", Properties.Resources.tile_strip26_9);
            TileSet.Add("tile_10", Properties.Resources.tile_strip26_10);
            TileSet.Add("tile_11", Properties.Resources.tile_strip26_11);
            TileSet.Add("tile_12", Properties.Resources.tile_strip26_12);
            TileSet.Add("tile_13", Properties.Resources.tile_strip26_13);
            TileSet.Add("tile_14", Properties.Resources.tile_strip26_14);
            TileSet.Add("tile_15", Properties.Resources.tile_strip26_15);
            TileSet.Add("tile_16", Properties.Resources.tile_strip26_16);
            TileSet.Add("tile_17", Properties.Resources.tile_strip26_17);
            TileSet.Add("tile_18", Properties.Resources.tile_strip26_18);
            TileSet.Add("tile_19", Properties.Resources.tile_strip26_19);
            TileSet.Add("tile_20", Properties.Resources.tile_strip26_20);
            TileSet.Add("tile_21", Properties.Resources.tile_strip26_21);
            TileSet.Add("tile_22", Properties.Resources.tile_strip26_22);
            TileSet.Add("tile_23", Properties.Resources.tile_strip26_23);
            TileSet.Add("tile_24", Properties.Resources.tile_strip26_24);
            TileSet.Add("tile_25", Properties.Resources.tile_strip26_25);
            TileSet.Add("tile_26", Properties.Resources.tile_strip26_26);

            //spritePageSelect = new List<Image[]>;
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
