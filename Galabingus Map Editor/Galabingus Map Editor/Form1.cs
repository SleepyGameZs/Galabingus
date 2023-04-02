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
    public partial class Form1 : Form
    {
        //16 x 9 
        private int numPages;

        private int pixelDensity;

        private Form mapEditor;

        public Form1()
        {
            InitializeComponent();
        }
        //Create Button
        private void button1_Click(object sender, EventArgs e)
        {
            Error();
        }

        //Load Button
        private void button2_Click(object sender, EventArgs e)
        {
            mapEditor = new MapEditorScreen();

            mapEditor.Show();
        }





        private bool InputCheck(object input)
        {
            int temp;
            TextBox box = (TextBox)input;
            try
            {

                temp = Int32.Parse(box.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Error()
        {
            string output = "Error:";

            bool invalid = false;
            //width
            if (InputCheck(textBox1))
            {
                if (Int32.Parse(textBox1.Text) > 0)
                {
                    numPages = Int32.Parse(textBox1.Text);
                }
                else
                {
                    output += "\n - Width value is not Valid and must be greater then 0";
                    invalid = true;
                }
            }
            else
            {
                output += "\n - Width is not Valid";
                invalid = true;
            }

            //Pixel Density
            if (InputCheck(textBox2))
            {
                if (Int32.Parse(textBox2.Text) > 0 && Int32.Parse(textBox2.Text) < 5)
                {

                }
                else
                {
                    output += "\n - Pixel Density value is not Valid and must be greater then 0 and less then 5";
                    invalid = true;
                }
            }

            if (invalid == false)
            {
                mapEditor = new MapEditorScreen(width, pixelDensity);

                mapEditor.Show();
            }
            else
            {
                MessageBox.Show(output, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

        /*
        private bool DivisibleBy(double input, double divisor)
        {
            if (input != 0 && (input / divisor) % 1 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        */

        
    }
}
