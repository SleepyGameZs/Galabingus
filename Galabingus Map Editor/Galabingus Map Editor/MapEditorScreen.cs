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
        private int totalEditorPageNum;
        private int tileSize;
        private int selectablePage;

        private bool saved;

        private List<Image> tileSet;

        private Image currentSelected;

        private List<Image[]> spritePageSelect;

        private Image[] imageArray;

        private Button[] buttonList; 

        public MapEditorScreen(int numofPage, int pixelDensity)
        {
            InitializeComponent();

            tileSet = new List<Image>();

            totalWidth = 16;

            totalHeight = 9;

            tileSize = 60;

            totalDensity = pixelDensity;

            totalEditorPageNum = numofPage;

            currentSelected = null;

            selectablePage = 1;

            TileSizeDet();

            ImageAdd();

            MapDraw();

            //currentSelected = Properties.Resources.enemy_dblue_strip4_1;

        }

        public MapEditorScreen()
        {
            InitializeComponent();

            currentSelected = null;

            TileSizeDet();

            ImageAdd();

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
            ChangeSelectable(-1);
        }

        //Right Change Selectable
        private void button12_Click(object sender, EventArgs e)
        {
            ChangeSelectable(1);
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

        private void ImageAdd()
        {
            //Enemy Sprites
            tileSet.Add(Properties.Resources.enemy_dblue_strip4_1);
            tileSet.Add(Properties.Resources.enemy_green_strip4_1);
            tileSet.Add(Properties.Resources.enemy_lblue_strip4_1);
            tileSet.Add(Properties.Resources.enemy_orange_strip4_1);
            tileSet.Add(Properties.Resources.enemy_pink_strip4_1);
            tileSet.Add(Properties.Resources.enemy_purple_strip4_1);
            tileSet.Add(Properties.Resources.enemy_red_strip4_1);
            tileSet.Add(Properties.Resources.enemy_yellow_strip4_1);

            //Boss Sprite
            //TileSet.Add(Image.FromFile(@"../Resources/Boss image");

            //Player Sprite
            tileSet.Add(Properties.Resources.player_strip5_1);

            //Asteroid Tiles
            tileSet.Add(Properties.Resources.tile_strip26_1);
            tileSet.Add(Properties.Resources.tile_strip26_2);
            tileSet.Add(Properties.Resources.tile_strip26_3);
            tileSet.Add(Properties.Resources.tile_strip26_4);
            tileSet.Add(Properties.Resources.tile_strip26_5);
            tileSet.Add(Properties.Resources.tile_strip26_6);
            tileSet.Add(Properties.Resources.tile_strip26_7);
            tileSet.Add(Properties.Resources.tile_strip26_8);
            tileSet.Add(Properties.Resources.tile_strip26_9);
            tileSet.Add(Properties.Resources.tile_strip26_10);
            tileSet.Add(Properties.Resources.tile_strip26_11);
            tileSet.Add(Properties.Resources.tile_strip26_12);
            tileSet.Add(Properties.Resources.tile_strip26_13);
            tileSet.Add(Properties.Resources.tile_strip26_14);
            tileSet.Add(Properties.Resources.tile_strip26_15);
            tileSet.Add(Properties.Resources.tile_strip26_16);
            tileSet.Add(Properties.Resources.tile_strip26_17);
            tileSet.Add(Properties.Resources.tile_strip26_18);
            tileSet.Add(Properties.Resources.tile_strip26_19);
            tileSet.Add(Properties.Resources.tile_strip26_20);
            tileSet.Add(Properties.Resources.tile_strip26_21);
            tileSet.Add(Properties.Resources.tile_strip26_22);
            tileSet.Add(Properties.Resources.tile_strip26_23);
            tileSet.Add(Properties.Resources.tile_strip26_24);
            tileSet.Add(Properties.Resources.tile_strip26_25);
            tileSet.Add(Properties.Resources.tile_strip26_26);

            spritePageSelect = new List<Image[]>();

            for (int x = 0; x < 4; x++)
            {
                imageArray = new Image[10];
                for (int y = 0; y < 10; y++)
                {
                    if ((10 * x) + y < 35)
                    {
                        imageArray[y] = tileSet[(10 * x) + y];
                    }
                    else
                    {
                        break;
                    }
                }
                spritePageSelect.Add(imageArray);
            }

            buttonList = new Button[]
            {
                button2,
                button7,
                button3,
                button8,
                button4,
                button9,
                button5,
                button10,
                button6,
                button11
            };
        }

        private void TileSizeDet()
        {
            totalWidth = totalWidth * totalDensity;

            totalHeight = totalHeight * totalDensity;

            tileSize = (int) tileSize / totalDensity;
        }

        private void ChangeSelectable(int change) 
        {
            selectablePage = selectablePage + change;
            if (selectablePage > 0 && selectablePage < 5)
            {
                Image[] temp = spritePageSelect[selectablePage];
                for (int x = 0; x < buttonList.Length; x++)
                {
                    buttonList[x].BackgroundImage = temp[x];
                }
            }
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
