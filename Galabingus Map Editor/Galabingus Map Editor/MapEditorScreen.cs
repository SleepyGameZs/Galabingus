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

        private int tileSize;

        private bool saved;

        public MapEditorScreen(int width, int height)
        {
            InitializeComponent();

            totalWidth = width;

            totalHeight = height;

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
            
        }

        //Button 2
        private void button7_Click(object sender, EventArgs e)
        {

        }

        //Button 3
        private void button3_Click(object sender, EventArgs e)
        {

        }

        //Button 4
        private void button8_Click(object sender, EventArgs e)
        {

        }

        //Button 5
        private void button4_Click(object sender, EventArgs e)
        {

        }

        //Button 6
        private void button9_Click(object sender, EventArgs e)
        {

        }

        //Button 7
        private void button5_Click(object sender, EventArgs e)
        {

        }

        //Button 8
        private void button10_Click(object sender, EventArgs e)
        {

        }

        //Button 9
        private void button6_Click(object sender, EventArgs e)
        {

        }

        //Button 10
        private void button11_Click(object sender, EventArgs e)
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

        }

        //Right Change Selectable
        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void ChangeSelectable(int change) 
        {
            
        }
    }
}
