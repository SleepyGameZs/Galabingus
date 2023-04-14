using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Galabingus_Map_Editor
{
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

        private List<ImageData> tileSet;

        private Image currentSelected;

        private List<Image[]> spritePageSelect;

        private Image[] imageArray;

        private PictureBox[] buttonList;

        //Stores all the changes to the picture boxes for each page
        private List<List<Image>> pagesData;

        public MapEditorScreen(int numofPage, int pixelDensity)
        {
            InitializeComponent();

            tileSet = new List<ImageData>();

            totalWidth = 16;

            totalHeight = 9;

            tileSize = 60;

            totalDensity = pixelDensity;

            totalEditorPageNum = numofPage;

            currentSelected = null;

            pagesData = new List<List<Image>>();

            currentSelectablePage = 0;

            currentEditorPage = 0;

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
            pictureBox11.Image = pictureBox1.Image;
            currentSelected = pictureBox1.Image;
        }

        //Button 2
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox2.Image;
            currentSelected = pictureBox2.Image;
        }

        //Button 3
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox3.Image;
            currentSelected = pictureBox3.Image;
        }

        //Button 4
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox4.Image;
            currentSelected = pictureBox4.Image;
        }

        //Button 5
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox5.Image;
            currentSelected = pictureBox5.Image;
        }

        //Button 6
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox6.Image;
            currentSelected = pictureBox6.Image;
        }

        //Button 7
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox7.Image;
            currentSelected = pictureBox7.Image;
        }

        //Button 8
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox8.Image;
            currentSelected = pictureBox8.Image;
        }

        //Button 9
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox9.Image;
            currentSelected = pictureBox9.Image;
        }

        //Button 10
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox10.Image;
            currentSelected = pictureBox10.Image;
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
            //ClearEditor();
            ChangeLevelSection(-1);
        }

        //Change Level Section Right
        private void button16_Click(object sender, EventArgs e)
        {
            //ClearEditor();
            ChangeLevelSection(1);
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
                    tileBox.BackColor = Color.White;
                    tileBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    tileBox.Capture = false;
                    Controls.Add(tileBox);
                    boxes.Add(tileBox);
                    tileBox.BringToFront();
                    tileBox.Click += ImageChanger;
                    //tileBox.Click += Change;
                }
            }
        }



        private void ImageAdd()
        {
            

            //Enemy Sprites
            tileSet.Add(new ImageData("dark blue", 1, Properties.Resources.enemy_dblue_strip4_1) );
            tileSet.Add(new ImageData("green", 2, Properties.Resources.enemy_green_strip4_1));
            tileSet.Add(new ImageData("light blue", 3, Properties.Resources.enemy_lblue_strip4_1));
            tileSet.Add(new ImageData("pink", 4,Properties.Resources.enemy_pink_strip4_1));
            tileSet.Add(new ImageData("purple", 5,Properties.Resources.enemy_purple_strip4_1));
            tileSet.Add(new ImageData("red", 6,Properties.Resources.enemy_red_strip4_1));
            tileSet.Add(new ImageData("yellow", 7,Properties.Resources.enemy_yellow_strip4_1));
            tileSet.Add(new ImageData("orange", 8,Properties.Resources.enemy_orange_strip4_1));

            //Boss Sprite
            //TileSet.Add(Image.FromFile(@"../Resources/Boss image");

            //Player Sprite
            tileSet.Add(new ImageData("player", 9, Properties.Resources.player_strip5_1));

            //Asteroid Tiles
            tileSet.Add(new ImageData("tile 1", 10, Properties.Resources.tile_strip26_1));
            tileSet.Add(new ImageData("tile 2", 11, Properties.Resources.tile_strip26_2));
            tileSet.Add(new ImageData("tile 3", 12, Properties.Resources.tile_strip26_3));
            tileSet.Add(new ImageData("tile 4", 13, Properties.Resources.tile_strip26_4));
            tileSet.Add(new ImageData("tile 5", 14, Properties.Resources.tile_strip26_5));
            tileSet.Add(new ImageData("tile 6", 15, Properties.Resources.tile_strip26_6));
            tileSet.Add(new ImageData("tile 7", 16, Properties.Resources.tile_strip26_7));
            tileSet.Add(new ImageData("tile 8", 17, Properties.Resources.tile_strip26_8));
            tileSet.Add(new ImageData("tile 9", 18, Properties.Resources.tile_strip26_9));
            tileSet.Add(new ImageData("tile 10", 19, Properties.Resources.tile_strip26_10));
            tileSet.Add(new ImageData("tile 11", 20, Properties.Resources.tile_strip26_11));
            tileSet.Add(new ImageData("tile 12", 21, Properties.Resources.tile_strip26_12));
            tileSet.Add(new ImageData("tile 13", 22, Properties.Resources.tile_strip26_13));
            tileSet.Add(new ImageData("tile 14", 23, Properties.Resources.tile_strip26_14));
            tileSet.Add(new ImageData("tile 15", 24, Properties.Resources.tile_strip26_15));
            tileSet.Add(new ImageData("tile 16", 25, Properties.Resources.tile_strip26_16));
            tileSet.Add(new ImageData("tile 17", 26, Properties.Resources.tile_strip26_17));
            tileSet.Add(new ImageData("tile 18", 27, Properties.Resources.tile_strip26_18));
            tileSet.Add(new ImageData("tile 19", 28, Properties.Resources.tile_strip26_19));
            tileSet.Add(new ImageData("tile 20", 29, Properties.Resources.tile_strip26_20));
            tileSet.Add(new ImageData("tile 21", 30, Properties.Resources.tile_strip26_21));
            tileSet.Add(new ImageData("tile 22", 31, Properties.Resources.tile_strip26_22));
            tileSet.Add(new ImageData("tile 23", 32, Properties.Resources.tile_strip26_23));
            tileSet.Add(new ImageData("tile 24", 33, Properties.Resources.tile_strip26_24));
            tileSet.Add(new ImageData("tile 25", 34, Properties.Resources.tile_strip26_25));
            tileSet.Add(new ImageData("tile 26", 35, Properties.Resources.tile_strip26_26));

            spritePageSelect = new List<Image[]>();

            for (int x = 0; x < 4; x++)
            {
                imageArray = new Image[10];
                for (int y = 0; y < 10; y++)
                {
                    if ((10 * x) + y < 35)
                    {
                        imageArray[y] = tileSet[(10 * x) + y].Image;
                    }
                    else
                    {
                        break;
                    }
                }
                spritePageSelect.Add(imageArray);
            }

            List<Image> temp = new List<Image>();
            for (int x = 0; x < pagesData.Count; x++)
            {
                for (int y = 0; y < boxes.Count; y++)
                {
                   temp.Add(boxes[y].Image);
                }
                pagesData.Add(temp);
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





        private void TileSizeDet()
        {
            totalWidth = totalWidth * totalDensity;

            totalHeight = totalHeight * totalDensity;

            tileSize = (int) tileSize / totalDensity;
        }

        private void ChangeSelectable(int change) 
        {
            if (currentSelectablePage + change >= 0 && currentSelectablePage + change < 4)
            {
                currentSelectablePage = currentSelectablePage + change;
                Image[] temp = spritePageSelect[currentSelectablePage];
                for (int x = 0; x < buttonList.Length; x++)
                {
                    buttonList[x].Image = temp[x];
                }
            }
        }

        private void ChangeLevelSection(int change)
        {
            if (currentEditorPage + change >= 0 && currentEditorPage + change < totalEditorPageNum)
            {
                TempMapSave(currentEditorPage);
                
                ClearEditor();

                PageLoad();
               
                
            }
        }

        private void ClearEditor()
        {
            for (int x = 0; x < totalHeight * totalWidth; x++)
            {
                boxes[x].Image = null;
            }
        }

        private void TempMapSave(int currentPage)
        {
            List<Image> temp = new List<Image>();
            pagesData.Add(temp);

            for (int x = 0; x < totalHeight * totalWidth; x++)
            {
                pagesData[currentPage].Add(boxes[x].Image);
            }
           
        }

        private void PageLoad()
        {

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

                try
                {
                    using (FileStream fs = File.Create(filesSave.FileName))
                    {
                        StreamWriter writer = new StreamWriter(fs);
                        writer.WriteLine(totalEditorPageNum);
                        writer.WriteLine(totalDensity);

                        
                    }
                }
                catch
                {

                }

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
                string fileName = loadSave.FileName;
            }
        }

        

    }
}
