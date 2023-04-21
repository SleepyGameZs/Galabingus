using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LevelEditor
{
    public partial class MapCreator : Form
    {
        LevelEditor editor;
        int tilesWidth;
        int tilesHeight;

        public MapCreator()
        {
            tilesWidth = 0;
            tilesHeight = 0;
            editor = new LevelEditor();

            InitializeComponent();
        }

        private void CreateMap(
            object sender,
            EventArgs e
        )
        {
            string errors = "Errors:";
            if (!int.TryParse( tilesWidthTextBox.Text, out tilesWidth ))
            {

            }
            if (tilesWidth < 9)
            {
                errors += "\n - Width too small. Minimum is 10";
            }
            else if (tilesWidth > 36)
            {
                errors += "\n - Width too large. Maximum is 30";
            }

            if (!int.TryParse( tilesHeightTextBox.Text, out tilesHeight ))
            {
                
            }
            if (tilesHeight < 9)
            {
                errors += "\n - Height too small. Minimum is 10";
            }
            else if (tilesHeight > 36)
            {
                errors += "\n - Height too large. Maximum is 30";
            }

            if (errors == "Errors:")
            {
                editor = new LevelEditor(
                    tilesWidth,
                    tilesHeight
                );

                editor.Show();
            }
            else
            {
                MessageBox.Show(
                    errors,
                    "Error creating map",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void LoadMap(object sender, EventArgs e)
        {
            editor = new LevelEditor(
                true,
                tilesWidth,
                tilesHeight
            );

            editor.Show();
        }
    }
}
