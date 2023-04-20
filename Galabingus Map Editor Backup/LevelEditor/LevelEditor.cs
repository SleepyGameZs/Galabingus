using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace LevelEditor
{
    public struct ImageData
    {
        
    }

    public partial class LevelEditor : Form
    {
        FileStream? fileStream;
        private Color selectedColor;
        private string fileName;
        private int tilesWidth;
        private int tilesHeight;
        private int tileWidth;
        private int tileHeight;
        private bool isMouseDown;
        private bool isEditited;

        public LevelEditor()
        {
            this.selectedColor = Color.Black;
            this.fileName = "";
            this.tilesWidth = 0;
            this.tilesHeight = 0;
            this.tileWidth = 0;
            this.tileHeight = 0;
            this.isMouseDown = false;
            this.isEditited = false;

            InitializeComponent();
        }

        public LevelEditor( 
            bool load,
            int tilesWidth,
            int tilesHeight
        )
        {
            this.selectedColor = Color.Black;
            this.fileName = "";
            this.tilesWidth = tilesWidth;
            this.tilesHeight = tilesHeight;
            this.isMouseDown = false;
            this.isEditited = false;

            InitializeComponent();

            this.tileWidth = (int)(greenTile.Width * 0.9);
            this.tileHeight = (int)(greenTile.Height * 0.9);

            LoadFile(this, new EventArgs());
        }


        public LevelEditor(
            int tilesWidth,
            int tilesHeight
        )
        {
            this.selectedColor = Color.Black;
            this.fileName = "";
            this.tilesWidth = tilesWidth;
            this.tilesHeight = tilesHeight;
            this.isMouseDown = false;
            this.isEditited = false;

            InitializeComponent();
            this.tileWidth = (int)(greenTile.Width * 0.9);
            this.tileHeight = (int)(greenTile.Height * 0.9);
            mapGroup.SetBounds(
                mapGroup.Bounds.X,
                mapGroup.Bounds.Y,
                this.tilesWidth * this.tileWidth,
                this.tilesHeight * this.tileHeight
            );

            for (int coulmn = 0; coulmn < tilesWidth; coulmn++)
            {
                for (int row = 0; row < tilesHeight; row++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.BackColor = Color.Black;
                    pictureBox.Width = tileWidth;
                    pictureBox.Height = tileHeight;
                    pictureBox.Location = new Point(coulmn * tileWidth, row * tileHeight);
                    pictureBox.MouseClick += ChangeColor;
                    pictureBox.MouseDoubleClick += ChangeColor;
                    pictureBox.MouseDown += ChangeColorHold;
                    pictureBox.MouseEnter += ChangeColorDrag;
                    pictureBox.MouseUp += ResetMouse;
                    mapGroup.Controls.Add(pictureBox);
                }
            }

            this.Width = mapGroup.Bounds.Width + mapGroup.Bounds.X + tileWidth * 2;
            this.Height = mapGroup.Bounds.Height + mapGroup.Bounds.Y + tileHeight * 3;

            mapGroup.MouseLeave += ResetMouse;
        }

        public LevelEditor(
            int tilesWidth,
            int tilesHeight,
            int tileWidth,
            int tileHeight
        )
        {
            this.selectedColor = Color.Black;
            this.fileName = "";
            this.tilesWidth = tilesWidth;
            this.tilesHeight = tilesHeight;
            this.isMouseDown = false;
            this.isEditited = false;

            InitializeComponent();
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            mapGroup.SetBounds(
                mapGroup.Bounds.X,
                mapGroup.Bounds.Y,
                this.tilesWidth * this.tileWidth,
                this.tilesHeight * this.tileHeight
            );

            for (int coulmn = 0; coulmn < tilesWidth; coulmn++)
            {
                for (int row = 0; row < tilesHeight; row++)
                {
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.BackColor = Color.Black;
                    pictureBox.Width = tileWidth;
                    pictureBox.Height = tileHeight;
                    pictureBox.Location = new Point(coulmn * tileWidth, row * tileHeight);
                    pictureBox.MouseClick += ChangeColor;
                    pictureBox.MouseDoubleClick += ChangeColor;
                    pictureBox.MouseDown += ChangeColorHold;
                    pictureBox.MouseEnter += ChangeColorDrag;
                    pictureBox.MouseUp += ResetMouse;
                    mapGroup.Controls.Add(pictureBox);
                }
            }

            this.Width = mapGroup.Bounds.Width + mapGroup.Bounds.X + tileWidth * 2;
            this.Height = mapGroup.Bounds.Height + mapGroup.Bounds.Y + tileHeight * 3;

            mapGroup.MouseLeave += ResetMouse;
        }

        public void ChangeColorDrag(
            object? sender,
            EventArgs? e
        )
        {
            if ( sender != null && isMouseDown && ( MouseButtons.Left == MouseButtons ) )
            {
                if (!isEditited)
                {
                    isEditited = true;
                    ChangeSaveStatus();
                }
                ( (PictureBox)sender ).BackColor = selectedColor;
            }
        }

        public void ChangeColorHold(
            object? sender,
            EventArgs? e
        )
        {
            isMouseDown = true;
            if (sender != null)
            {
                ( (PictureBox)sender ).BackColor = selectedColor;
                ( (PictureBox)sender ).Capture = false;
            }
            if (!isEditited)
            {
                isEditited = true;
                ChangeSaveStatus();
            }
        }

        private void SelectColor(
            object sender,
            EventArgs e
        )
        {
            selectedColor = ( (PictureBox)sender ).BackColor;
            currentTile.BackColor = selectedColor;
        }

        public void ChangeColor(
            object? sender,
            EventArgs? e
        )
        {
            if (sender != null)
            {
                ( (PictureBox)sender ).BackColor = selectedColor;
            }
            if (!isEditited)
            {
                isEditited = true;
                ChangeSaveStatus();
            }
        }

        public void ResetMouse(
            object? sender,
            EventArgs? e
        )
        {
            isMouseDown = false;
        }

        private void ChangeSaveStatus()
        {
            if (fileName == "")
            {
                this.Text = (
                    (!isEditited)
                    ? "Level Editor"
                    : "Level Editor *"
                );
            }
            else
            {
                this.Text = (
                    (!isEditited)
                    ? "Level Editor - "+fileName
                    : "Level Editor - " + fileName + " *"
                );
            }
        }

        private void SaveFile(
            object sender,
            EventArgs e
        )
        {
            List<int> tileRLE = new List<int>();
            List<string> tileRLEValues = new List<string>();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Level Files|*.level";
            saveFileDialog.Title = "Save a Level file.";
            string tileData = "";
            string currentColor = "";
            string path = "";
            int currentRLE = -1;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog.FileName;
                fileName = Path.GetFileName(path);
                fileStream = File.Create(path);
                fileStream.Write(
                    Encoding.UTF8.GetBytes(tilesWidth + "|" + tilesHeight + "|")
                );

                foreach (PictureBox tile in mapGroup.Controls)
                {
                    if (tile is PictureBox)
                    {
                        string color = "" + ((int)tile.BackColor.ToArgb());
                        if (currentColor != color)
                        {
                            currentColor = color;
                            tileRLE.Add(1);
                            tileRLEValues.Add(color);
                            currentRLE++;
                        }
                        else
                        {
                            tileRLE[currentRLE]++;
                        }
                    }
                }

                for (int i = 0; i < tileRLE.Count; i++)
                {
                    tileData += tileRLE[i] + "*" + tileRLEValues[i] + ">";
                }
                tileData = tileData.Remove(tileData.Length - 1);

                fileStream.Write(
                    Encoding.UTF8.GetBytes(tileData)
                );

                fileStream.Close();
                isEditited = false;
                ChangeSaveStatus();

                MessageBox.Show(
                    "File saved successfully",
                    "File saved",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
        }

        private void LoadFile(
            object sender,
            EventArgs e
        )
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Level Files|*.level";
            openFileDialog.Title = "Open a Level file.";
            string path = "";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                fileName = openFileDialog.SafeFileName;
                isEditited = false;
                ChangeSaveStatus();

                fileStream = File.OpenRead(path);
                StreamReader reader = new StreamReader(fileStream);
                string? data = "";
                if ((data = reader.ReadLine()) == null)
                {
                    Debug.WriteLine("Error Reading File");
                }
                reader.Close();
                fileStream.Close();

                List<PictureBox> pictureBoxes = new List<PictureBox>();
                string[] tilesProperties = (data ?? "").Split("|");
                string[] tiles = tilesProperties[2].Split(">");
                int tilesPropertiesWidth = int.Parse(tilesProperties[0]);
                int tilesPropertiesHeight = int.Parse(tilesProperties[1]);
                mapGroup.Controls.Clear();
                int row = -1;
                int column = 0;

                foreach (string tile in tiles)
                {
                    string[] tileInfo = tile.Split("*");
                    Color tileColor = Color.FromArgb(int.Parse(tileInfo[1]));

                    for (int i = 0; i < int.Parse(tileInfo[0]); i++)
                    {
                        if (row + 1 >= tilesPropertiesHeight)
                        {
                            row = 0;
                            column++;
                        }
                        else
                        {
                            row++;
                        }

                        PictureBox pictureBox = new PictureBox();
                        pictureBox.BackColor = tileColor;
                        pictureBox.Width = tileWidth;
                        pictureBox.Height = tileHeight;
                        pictureBox.Location = new Point(column * tileWidth, row * tileHeight);
                        pictureBox.MouseClick += ChangeColor;
                        pictureBox.MouseDoubleClick += ChangeColor;
                        pictureBox.MouseDown += ChangeColorHold;
                        pictureBox.MouseEnter += ChangeColorDrag;
                        pictureBox.MouseUp += ResetMouse;
                        pictureBoxes.Add(pictureBox);
                    }
                }

                mapGroup.Controls.AddRange(pictureBoxes.ToArray());

                mapGroup.SetBounds(
                    mapGroup.Bounds.X,
                    mapGroup.Bounds.Y,
                    tilesPropertiesWidth * this.tileWidth,
                    tilesPropertiesHeight * this.tileHeight
                );

                this.Width = mapGroup.Bounds.Width + mapGroup.Bounds.X + tileWidth * 2;
                this.Height = mapGroup.Bounds.Height + mapGroup.Bounds.Y + tileHeight * 3;

                MessageBox.Show(
                    "File loaded successfully",
                    "File loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation
                );
            }
        }

        private void LevelEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isEditited)
            {
                if (
                    MessageBox.Show(
                        "There are unsaved changes. Are you sure you want to quit?",
                        "Unsaved changes",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    )
                    ==
                    DialogResult.No
                )
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
