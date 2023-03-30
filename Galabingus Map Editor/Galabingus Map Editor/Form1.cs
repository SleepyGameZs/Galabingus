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
        //must be divisible by 9 to get a number without a decimal (Most likely wont need this)
        private int height;

        private int heightLayers;

        //must be divisible by 16 to get a number without a decimal
        private int width;

        private int widthLayers;

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
            if (InputCheck(textBox2))
            {
                if (DivisibleBy(Int32.Parse(textBox2.Text), 18))
                {
                    width = Int32.Parse(textBox2.Text);
                }
                else
                {
                    output += "\n - Width value is not Valid and must be divisable by 18";
                    invalid = true;
                }
            }
            else
            {
                output += "\n - Width is not Valid";
                invalid = true;
            }

            //Height
            if (InputCheck(textBox2))
            {
                if (DivisibleBy(Int32.Parse(textBox2.Text), 12))
                {
                    height = Int32.Parse(textBox2.Text);
                }
                else
                {
                    output += "\n - Height value is not Valid and must be divisable by 12";
                    invalid = true;
                }
            }
            else
            {
                output += "\n - Height is not Valid";
                invalid = true;
            }

            if (invalid == false)
            {
                mapEditor = new MapEditorScreen(width, height);

                mapEditor.Show();
            }
            else
            {
                MessageBox.Show(output, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

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

        
    }
}
