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
        private int currentSelectablePage;
        private int currentEditorPage;

        private bool newMap;
        private bool changed;
        private bool saved;
        private bool moving;

        private string filePath;

        private List<Image> tileSet;

        private Image currentSelected;

        private List<Image[]> spritePageSelect;

        private Image[] imageArray;

        private PictureBox[] buttonList; 

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

            currentSelectablePage = 1;

            moving = false;

            filePath = @"..\..\..\";

            ImageAdd();

            TileSizeDet();

            ButtonStuff();

            MapDraw();

            ChangeSelectable(0);

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
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox1.BackgroundImage;
            currentSelected = pictureBox1.BackgroundImage;
        }

        //Button 2
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox2.BackgroundImage;
            currentSelected = pictureBox2.BackgroundImage;
        }

        //Button 3
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox3.BackgroundImage;
            currentSelected = pictureBox3.BackgroundImage;
        }

        //Button 4
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox4.BackgroundImage;
            currentSelected = pictureBox4.BackgroundImage;
        }

        //Button 5
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox5.BackgroundImage;
            currentSelected = pictureBox5.BackgroundImage;
        }

        //Button 6
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox6.BackgroundImage;
            currentSelected = pictureBox6.BackgroundImage;
        }

        //Button 7
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox7.BackgroundImage;
            currentSelected = pictureBox7.BackgroundImage;
        }

        //Button 8
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox8.BackgroundImage;
            currentSelected = pictureBox8.BackgroundImage;
        }

        //Button 9
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox9.BackgroundImage;
            currentSelected = pictureBox9.BackgroundImage;
        }

        //Button 10
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            pictureBox11.BackgroundImage = pictureBox10.BackgroundImage;
            currentSelected = pictureBox10.BackgroundImage;
        }

        //Current Image
        private void pictureBox11_Click(object sender, EventArgs e)
        {

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
                    tileBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    tileBox.Capture = false;
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

            
        }

        private void TileSizeDet()
        {
            totalWidth = totalWidth * totalDensity;

            totalHeight = totalHeight * totalDensity;

            tileSize = (int) tileSize / totalDensity;
        }

        private void ChangeSelectable(int change) 
        {
            if (currentSelectablePage + change > 0 && currentSelectablePage + change < 4)
            {
                currentSelectablePage = currentSelectablePage + change;
                Image[] temp = spritePageSelect[currentSelectablePage];
                for (int x = 0; x < buttonList.Length; x++)
                {
                    buttonList[x].BackgroundImage = temp[x];
                }
            }
        }

        private void ChangeLevelSection(int change)
        {
            if (currentEditorPage + change > 0 && currentEditorPage + change < totalEditorPageNum)
            {
                currentEditorPage = currentEditorPage + change;
            }
        }

        private void ButtonStuff()
        {
            buttonList = new PictureBox[]
            {
                pictureBox1,
                pictureBox2,
                pictureBox3,
                pictureBox4,
                pictureBox5,
                pictureBox6,
                pictureBox7,
                pictureBox8,
                pictureBox9,
                pictureBox10
            };

            for (int x = 0; x < buttonList.Count(); x++)
            {
                buttonList[x].SizeMode = PictureBoxSizeMode.StretchImage;
            }

            pictureBox11.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void TempMapSave()
        {
            for (int x = 0; x < boxes.Count; x++)
            {

            }
        }

        public void Savefile()
        {
            SaveFileDialog filesSave = new SaveFileDialog();
            filesSave.Title = "Save a Level File";
            filesSave.Filter = "Level Files|*.level";

            newMap = false;

            changed = false;

            saved = true;

            if (filesSave.ShowDialog() == DialogResult.OK)
            {
                string fileName = filesSave.FileName;

            }
        }

        public void LoadFile()
        {
            OpenFileDialog loadSave = new OpenFileDialog();
            loadSave.Title = "Load a Level File";
            loadSave.Filter = "Level Files|*.level";

            newMap = true;

            saved = true;

            if (loadSave.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
