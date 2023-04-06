using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

        private int currentColorPage;

        private List<Image[]> spritePages;

        private int tileSize;

        private bool saved;

        private Image currentImage;

        public MapEditorScreen(int width, int height, int pixelDensity)
        {
            InitializeComponent();

            totalWidth = width;

            totalHeight = height;

            totalDensity = pixelDensity;

            tileSize = 60;
        }

        public MapEditorScreen()
        {
            InitializeComponent();

            ImageList tiles = new ImageList();

            tiles.ImageSize = new Size(80, 80);
            tiles.TransparentColor = Color.White;

            tileSize = 60;
        }

        //Button 1
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button2.BackgroundImage;
            currentImage = button2.BackgroundImage;
        }

        //Button 2
        private void button7_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button7.BackgroundImage;
            currentImage = button7.BackgroundImage;
        }

        //Button 3
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button3.BackgroundImage;
            currentImage = button3.BackgroundImage;
        }

        //Button 4
        private void button8_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button8.BackgroundImage;
            currentImage = button8.BackgroundImage;
        }

        //Button 5
        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button4.BackgroundImage;
            currentImage = button4.BackgroundImage;
        }

        //Button 6
        private void button9_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button9.BackgroundImage;
            currentImage = button9.BackgroundImage;
        }

        //Button 7
        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button5.BackgroundImage;
            currentImage = button5.BackgroundImage;
        }

        //Button 8
        private void button10_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button10.BackgroundImage;
            currentImage = button10.BackgroundImage;
        }

        //Button 9
        private void button6_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button6.BackgroundImage;
            currentImage = button6.BackgroundImage;
        }

        //Button 10
        private void button11_Click(object sender, EventArgs e)
        {
            pictureBox1.BackgroundImage = button11.BackgroundImage;
            currentImage = button11.BackgroundImage;
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

        private void ChangeSelectable(int change) 
        {
            if ()
            {

            }
        }

        private void ScaleUp()
        {

        }


        private void ImageChanger(object tile, EventArgs click)
        {
            if(tile != null)
            {
                PictureBox pixel = (PictureBox)tile;
                pixel.BackgroundImage = currentImage;
            }
        }

        private

    }
}
