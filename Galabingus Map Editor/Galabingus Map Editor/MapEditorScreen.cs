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

        private List<Image> TileSet;

        private Image currentSelected;

        private List<Image[]> spritePageSelect;

        private Image[] imageArray; 

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

            currentSelected = Properties.Resources.enemy_dblue_strip4_1;

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

        //Change Level Section Left
        private void button17_Click(object sender, EventArgs e)
        {

        }

        //Change Level Section Right
        private void button16_Click(object sender, EventArgs e)
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
            TileSet.Add(Properties.Resources.enemy_dblue_strip4_1);
            TileSet.Add(Properties.Resources.enemy_green_strip4_1);
            TileSet.Add(Properties.Resources.enemy_lblue_strip4_1);
            TileSet.Add(Properties.Resources.enemy_orange_strip4_1);
            TileSet.Add(Properties.Resources.enemy_pink_strip4_1);
            TileSet.Add(Properties.Resources.enemy_purple_strip4_1);
            TileSet.Add(Properties.Resources.enemy_red_strip4_1);
            TileSet.Add(Properties.Resources.enemy_yellow_strip4_1);

            //Boss Sprite
            //TileSet.Add(Image.FromFile(@"../Resources/Boss image");

            //Player Sprite
            TileSet.Add(Properties.Resources.player_strip5_1);

            //Asteroid Tiles
            TileSet.Add(Properties.Resources.tile_strip26_1);
            TileSet.Add(Properties.Resources.tile_strip26_2);
            TileSet.Add(Properties.Resources.tile_strip26_3);
            TileSet.Add(Properties.Resources.tile_strip26_4);
            TileSet.Add(Properties.Resources.tile_strip26_5);
            TileSet.Add(Properties.Resources.tile_strip26_6);
            TileSet.Add(Properties.Resources.tile_strip26_7);
            TileSet.Add(Properties.Resources.tile_strip26_8);
            TileSet.Add(Properties.Resources.tile_strip26_9);
            TileSet.Add(Properties.Resources.tile_strip26_10);
            TileSet.Add(Properties.Resources.tile_strip26_11);
            TileSet.Add(Properties.Resources.tile_strip26_12);
            TileSet.Add(Properties.Resources.tile_strip26_13);
            TileSet.Add(Properties.Resources.tile_strip26_14);
            TileSet.Add(Properties.Resources.tile_strip26_15);
            TileSet.Add(Properties.Resources.tile_strip26_16);
            TileSet.Add(Properties.Resources.tile_strip26_17);
            TileSet.Add(Properties.Resources.tile_strip26_18);
            TileSet.Add(Properties.Resources.tile_strip26_19);
            TileSet.Add(Properties.Resources.tile_strip26_20);
            TileSet.Add(Properties.Resources.tile_strip26_21);
            TileSet.Add(Properties.Resources.tile_strip26_22);
            TileSet.Add(Properties.Resources.tile_strip26_23);
            TileSet.Add(Properties.Resources.tile_strip26_24);
            TileSet.Add(Properties.Resources.tile_strip26_25);
            TileSet.Add(Properties.Resources.tile_strip26_26);

            spritePageSelect = new List<Image[]>();

            for ()
            {
                imageArray = new Image[10];
                for (int y = 0; y < 10; y++)
                {
                    imageArray[y] = 
                }
            }
            imageArray = new Image[] 
            { 
                
            };

            spritePageSelect.Add(imageArray);

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

        private void ChangeLevelSection(int change)
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
