using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Galabingus_Map_Editor
{
    //Justin Tong
    //4/30/2023
    //The Map Editor Screen that makes an editable map win a 9 x 36 with a little under 40 usable tiles for the editor, with a function saving and loading
    public partial class MapEditorScreen : Form
    {
        private List<PictureBox> boxes = new List<PictureBox>();

        private int totalWidth;
        private int totalHeight;
        private int totalDensity;
        private int tileSize;
        private int currentSelectablePageNum;

        private bool newMap;
        private bool changed;
        private bool saved;
        private bool moving;
        private bool drag;

        private string filePath;

        private List<ImageData> tileSet;

        private ImageData currentSelected;

        private List<Image[]> spritePageSelect;

        private Image[] imageArray;

        private PictureBox[] buttonList;

        //Stores all the changes to the picture boxes for each page
        private List<ImageData> boxImages;

        #region Constructors

        //The constructor used for 
        public MapEditorScreen(bool load)
        {
            InitializeComponent();

            tileSet = new List<ImageData>();

            boxImages = new List<ImageData>();

            //currentSelected = null;

            moving = false;

            filePath = @"..\..\..\";

            LoadFile();
        }

        //The contructor used for creating a new and empty editor
        public MapEditorScreen(int pixelDensity)
        {
            InitializeComponent();

            boxImages = new List<ImageData>();

            tileSet = new List<ImageData>();

            totalWidth = 9;

            totalHeight = 36;

            tileSize = 20;

            totalDensity = pixelDensity;

            currentSelectablePageNum = 0;

            moving = false;

            filePath = @"..\..\..\";

            TileSizeDet();

            ImageAdd();

            ButtonStuff();

            MapDraw();

            ChangeSelectable(0);

        }

        //
        public MapEditorScreen()
        {
            InitializeComponent();

            //currentSelected = null;

            TileSizeDet();

            ImageAdd();

            MapDraw();
        }

        #endregion
        /// <summary>
        /// Matches the image to the same image
        /// </summary>
        /// <param name="image">The image thats being referenced</param>
        /// <returns>
        /// Returns the ImageData of that image
        /// or the first image
        /// </returns>
        private ImageData MatchImageData(System.Drawing.Image image)
        {
            foreach(ImageData img in tileSet)
            {
                if (img.Image == image)
                {
                    return img;
                }
            }

            return tileSet[0];
        }

        /// <summary>
        /// Matches the number inputed to the image with the ID number
        /// </summary>
        /// <param name="image">The number that represents an image</param>
        /// <returns>returns the image based on the index, or just a empty one</returns>
        private ImageData MatchImageData(int image)
        {
            foreach (ImageData img in tileSet)
            {
                if (img.ImageNumber == image)
                {
                    return img;
                }
            }

            return tileSet[0];
        }

        #region Buttons

        //Buttons
        //The buttons that make up the tile selector and the Save and Load Buttons 

        #region Selector Buttons
        //Button 1
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox1.Image;
            currentSelected = MatchImageData(pictureBox1.Image);
        }

        //Button 2
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox2.Image;
            currentSelected = MatchImageData(pictureBox2.Image);
        }

        //Button 3
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox3.Image;
            currentSelected = MatchImageData(pictureBox3.Image);
        }

        //Button 4
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox4.Image;
            currentSelected = MatchImageData(pictureBox4.Image);
        }

        //Button 5
        private void pictureBox5_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox5.Image;
            currentSelected = MatchImageData(pictureBox5.Image);
        }

        //Button 6
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox6.Image;
            currentSelected = MatchImageData(pictureBox6.Image);
        }

        //Button 7
        private void pictureBox7_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox7.Image;
            currentSelected = MatchImageData(pictureBox7.Image);
        }

        //Button 8
        private void pictureBox8_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox8.Image;
            currentSelected = MatchImageData(pictureBox8.Image);
        }

        //Button 9
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox9.Image;
            currentSelected = MatchImageData(pictureBox9.Image);
        }

        //Button 10
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            pictureBox11.Image = pictureBox10.Image;
            currentSelected = MatchImageData(pictureBox10.Image);
        }

        #endregion

        //Save Button
        private void button14_Click(object sender, EventArgs e)
        {
            Savefile();
        }

        //Load Button
        private void button15_Click(object sender, EventArgs e)
        {
            LoadFile();
        }

        //The buttons that can change the page of the selectable

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

        #endregion

        /// <summary>
        /// The method that allows the edit of the picture boxes in the editor
        /// </summary>
        /// <param name="tile">The picture box being clicked on</param>
        /// <param name="click">Detects if the mouse is being clicked</param>
        private void ImageChanger(object tile, EventArgs click)
        {
            if (tile != null)
            {
                PictureBox pixel = (PictureBox)tile;
                pixel.Image = currentSelected.Image;
                boxImages[(int)pixel.Tag] = currentSelected;
                //changed = true;
                drag = true;
            }
        }

        /// <summary>
        /// Draws a blank map editor while adding the picture boxes to needed events
        /// </summary>
        private void MapDraw()
        {
            /*
            this.SetBounds(
                this.Bounds.X,
                this.Bounds.Y,
                mapGroup.Bounds.X + this.totalWidth * tileSize + 30,
                mapGroup.Bounds.Y + this.totalHeight * tileSize + 60
            );
            
            mapGroup.SetBounds(
                mapGroup.Bounds.X,
                mapGroup.Bounds.Y,
                this.totalWidth * tileSize,
                this.totalHeight * tileSize
            );
            */
            for (int y = 0; y < totalHeight; y++)
            {
                for (int x = 0; x < totalWidth; x++)
                {
                    PictureBox tileBox = new PictureBox();
                    tileBox.Size = new Size(tileSize, tileSize);
                    tileBox.Location = new Point(65 + (tileSize * x), 0 + (tileSize * y));
                    tileBox.BackColor = Color.White;
                    tileBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    tileBox.Click += ImageChanger;
                    tileBox.MouseDown += MouseReady;
                    tileBox.MouseEnter += MouseDrag;
                    tileBox.MouseUp += ResetMouse;
                    tileBox.BringToFront();
                    tileBox.Tag = boxes.Count;
                    boxImages.Add(MatchImageData(-1));
                    boxes.Add(tileBox);
                    mapGroup.Controls.Add(tileBox);
                }
            }

            mapGroup.MouseLeave += ResetMouse;
        }

        /// <summary>
        /// Clears the whole editor with only a blank editor remaining
        /// </summary>
        private void ClearEditor()
        {
            for (int x = 0; x < totalHeight * totalWidth; x++)
            {
                boxes[x].Image = null;
            }
        }

        #region Drag Click
        //The methods used to allow for dragging on the map editor 


        /// <summary>
        /// throws an Exception if te mouse has entered a tile box
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void TileBox_MouseEnter(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// sets drag to false
        /// </summary>
        private void ResetMouse(object sender,EventArgs e)
        {
            drag = false;
        }

        /// <summary>
        /// sets drag to true while changing the capture value of the picuture box
        /// </summary>
        private void MouseReady(object sender,EventArgs e)
        {
            drag = true;
            ((PictureBox)sender).Capture = false;
        }

        /// <summary>
        /// if drag is true will change the image of the tile 
        /// </summary>
        private void MouseDrag(object sender,EventArgs e)
        {
            if (drag)
            {
                boxImages[(int)((PictureBox)sender).Tag] = currentSelected;
                ((PictureBox)sender).Image = currentSelected.Image;
            }
        }

        #endregion

        /// <summary>
        /// adds the used images to a list and adds the images that would be used in the selectable list
        /// </summary>
        private void ImageAdd()
        {
            #region Adding Imagees
            tileSet.Add(new ImageData("", -1, null));

            //Enemy Sprites
            tileSet.Add(new ImageData("red", 0,Properties.Resources.enemy_red_strip4_1));
            tileSet.Add(new ImageData("orange", 1, Properties.Resources.enemy_orange_strip4_1));
            tileSet.Add(new ImageData("yellow", 2,Properties.Resources.enemy_yellow_strip4_1));
            tileSet.Add(new ImageData("green", 3, Properties.Resources.enemy_green_strip4_1));
            tileSet.Add(new ImageData("violet", 4, Properties.Resources.enemy_violet_strip4_1));

            //Bomb
            tileSet.Add(new ImageData("bomb", 5, Properties.Resources.bomb_strip4_1));

            //Boss Sprite
            tileSet.Add(new ImageData("boss", 6,Properties.Resources.boss_green_strip4_1));

            //Player Sprite
            tileSet.Add(new ImageData("player", 8, Properties.Resources.player_strip4_1));

            //Camera Stop
            tileSet.Add(new ImageData("tile 1", 9, Properties.Resources.camera_stop_strip1_1));

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

            #endregion

            spritePageSelect = new List<Image[]>();

            for (int x = 0; x < 4; x++)
            {
                imageArray = new Image[10];
                for (int y = 0; y < 10; y++)
                {
                    if ((10 * x) + y < tileSet.Count)
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

        }

        /// <summary>
        /// Adds the buttons to a list to be accessed 
        /// </summary>
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




        /// <summary>
        /// Changes the size of the tiles based on the pixel density 
        /// </summary>
        private void TileSizeDet()
        {
            totalWidth = totalWidth * totalDensity;

            totalHeight = totalHeight * totalDensity;

            tileSize = (int) tileSize / totalDensity;
        }

        /// <summary>
        /// Changes the current selectable tiles
        /// </summary>
        /// <param name="change"></param>
        private void ChangeSelectable(int change) 
        {
            
            if (currentSelectablePageNum + change >= 0 && currentSelectablePageNum + change < 4)
            {
                
                currentSelectablePageNum += change;
                Image[] temp = spritePageSelect[currentSelectablePageNum];
                for (int x = 0; x < buttonList.Length; x++)
                {
                    buttonList[x].Image = temp[x];
                }
            }
        }




        #region Save/Load Methods

        /// <summary>
        /// The Method that saves the map data to a .level file in the a location of choice
        /// Saves map data inlcuding changes done to the map, the current pixel density
        /// </summary>
        public void Savefile()
        {
            SaveFileDialog filesSave = new SaveFileDialog();
            filesSave.Title = "Save a Level File";
            filesSave.Filter = "Level Files|*.level";
            boxes = new List<PictureBox>();


            newMap = false;

            changed = false;

            saved = true;



            if (filesSave.ShowDialog() == DialogResult.OK)
            {
                string fileName = filesSave.FileName;

                try
                {
                    StreamWriter writer = new StreamWriter(fileName);
                    writer.WriteLine(tileSize);
                    writer.WriteLine(totalHeight);
                    writer.WriteLine(totalWidth);
                    writer.WriteLine(1);
                    writer.WriteLine(totalDensity);

                    int tileNumber = 0;
                    int currentY = 0;
                    for (int y = 0; y < totalHeight; y++)
                    {
                        for (int x = 0; x < totalWidth; x++)
                        {
                            if (currentY != y)
                            {
                                currentY = y;
                                writer.Write("\n");
                                writer.Write($"{boxImages[tileNumber].ImageNumber}");
                                tileNumber++;
                            }
                            else if (y == 0 && x == 0)
                            {
                                writer.Write($"{boxImages[tileNumber].ImageNumber}");
                                tileNumber++;
                            }
                            else
                            {
                                writer.Write($"|{boxImages[tileNumber].ImageNumber}");
                                tileNumber++;
                            }

                        }
                    }
                    writer.Close();
                }
                catch
                {

                }

            }
        }
        
        /// <summary>
        /// Loads only .level files
        /// reads in the in the pixel density and the map data that includes all changes done to the map
        /// </summary>
        public void LoadFile()
        {
            boxes = new List<PictureBox>();

            totalWidth = 0;
            totalHeight = 0;
            totalDensity = 0;
            tileSize = 0;
            currentSelectablePageNum = 0;


            imageArray = null;
            buttonList = null;
            tileSet = new List<ImageData>();
            boxImages = new List<ImageData>();
            //currentSelected = null;
            moving = false;
            filePath = @"..\..\..\";

            OpenFileDialog loadSave = new OpenFileDialog();
            loadSave.Title = "Load a Level File";
            loadSave.Filter = "Level Files|*.level";

            newMap = true;

            saved = true;

            if (loadSave.ShowDialog() == DialogResult.OK)
            {
                int tempX = mapGroup.Bounds.X;
                int tempY = mapGroup.Bounds.Y;
                int tempW = mapGroup.Bounds.Width;
                int tempH = mapGroup.Bounds.Height;
                this.Controls.Remove(mapGroup);
                mapGroup = new GroupBox();
                mapGroup.SetBounds(tempX,tempY,tempW,tempH);
                mapGroup.Visible = false;
                this.Controls.Add(mapGroup);


                string fileName = loadSave.FileName;

                StreamReader reader = new StreamReader(fileName);
                string data = "";
     
                int lineNumber = 0;
                int yInput = 0;
                int xInput = 0;
                int boxIdentifier = 0;

                while ((data = reader.ReadLine()) != null)
                {
                    //Debug.WriteLine(data);

                    if (lineNumber < 5)
                    {
                        switch (lineNumber)
                        {
                            case 0:
                                tileSize = int.Parse(data);
                                data = "";
                                break;
                            case 1:
                                totalHeight = int.Parse(data);
                                data = "";
                                break;
                            case 2:
                                totalWidth = int.Parse(data);
                                data = "";
                                break;
                            case 3:
                                totalDensity = int.Parse(data);
                                data = "";
                                ImageAdd();
                                ButtonStuff();
                                /*
                                mapGroup.SetBounds(
                                    mapGroup.Bounds.X,
                                    mapGroup.Bounds.Y,
                                    this.totalWidth * tileSize,
                                    this.totalHeight * tileSize
                                );
                                */
                                break;
                        }
                    }
                    else
                    {
                        string[] row = data.Split('|');
                        //Debug.WriteLine(data);
                        foreach (string num in row) 
                        {
                            PictureBox tileBox = new PictureBox();
                            tileBox.Size = new Size(tileSize, tileSize);
                            tileBox.Location = new Point(65 + (tileSize * xInput), 20 + (tileSize * yInput));
                            tileBox.BackColor = Color.White;
                            tileBox.SizeMode = PictureBoxSizeMode.StretchImage;
                            tileBox.Click += ImageChanger;
                            tileBox.MouseDown += MouseReady;
                            tileBox.MouseEnter += MouseDrag;
                            tileBox.MouseUp += ResetMouse;
                            tileBox.BringToFront();
                            tileBox.Tag = boxes.Count;
                            boxImages.Add(MatchImageData(int.Parse(num)));
                            boxes.Add(tileBox);
                            tileBox.Image = boxImages[boxIdentifier].Image;
                        
                            xInput++;
                            boxIdentifier++;
                        }
                        xInput = 0;
                        yInput++;
                        data = "";
                    }
                    lineNumber++;
                }

                reader.Close();
            }
            else
            {
                totalWidth = 9;
                totalHeight = 36;
                totalDensity = 1;
                tileSize = 25;

                TileSizeDet();

                ImageAdd();

                ButtonStuff();

                MapDraw();

                ChangeSelectable(0);
            }

            /*
            this.SetBounds(
                this.Bounds.X,
                this.Bounds.Y,
                mapGroup.Bounds.X + this.totalWidth * tileSize + 30,
                mapGroup.Bounds.Y + this.totalHeight * tileSize + 60
            );

            mapGroup.SetBounds(
                mapGroup.Bounds.X,
                mapGroup.Bounds.Y,
                this.totalWidth * tileSize,
                this.totalHeight * tileSize
            );
            */
            mapGroup.MouseLeave += ResetMouse;
            mapGroup.Controls.AddRange(boxes.ToArray());
            mapGroup.Visible = true;

            ChangeSelectable(0);
        }

        #endregion

        

       
    }
}
