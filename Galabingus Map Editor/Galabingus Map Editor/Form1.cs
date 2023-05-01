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
    //Justin Tong
    //4/30/2023
    //The input screen where you input the pixel density between 1 and 4 and will create a editor that is 9 by 36 multiplied by the pixel density 
    public partial class Form1 : Form
    {
        //9 x 36

        private int pixelDensity;

        private MapEditorScreen mapEditor;

        public Form1()
        {
            InitializeComponent();
        }

        #region
        //Create Button
        /// <summary>
        /// Creates a blank editor based on the pixel density
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            Error();
        }

        //Load Button
        /// <summary>
        /// Asks for a file to load and will load the file in the editor 
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            mapEditor = new MapEditorScreen(true);
            //mapEditor.LoadFile();
            mapEditor.Show();
        }
        #endregion

        /// <summary>
        /// The method used to check if the text input inside of a form object is a valid number
        /// </summary>
        /// <param name="input">the object that will have its text checked</param>
        /// <returns>
        /// True if the text is just a valid number
        /// false if the text isnt a valid number
        /// </returns>
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

        /// <summary>
        /// Checks if the input for the text box is actully a number and is between 1 and 4
        /// </summary>
        private void Error()
        {
            string output = "Error:";

            bool invalid = false;
            //Pixel Density
            if (InputCheck(textBox2))
            {
                if (Int32.Parse(textBox2.Text) > 0 && Int32.Parse(textBox2.Text) < 5)
                {
                    pixelDensity = Int32.Parse(textBox2.Text);
                }
                else
                {
                    output += "\n - Pixel Density value is not Valid and must be greater then 0 and less then 5";
                    invalid = true;
                }
            }

            if (invalid == false)
            {

                mapEditor = new MapEditorScreen(pixelDensity);
                mapEditor.Show();
            }
            else
            {
                MessageBox.Show(output, "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

    }
}
