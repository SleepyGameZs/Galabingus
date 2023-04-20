namespace LevelEditor
{
    partial class LevelEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.blackTile = new System.Windows.Forms.PictureBox();
            this.blueTile = new System.Windows.Forms.PictureBox();
            this.redTile = new System.Windows.Forms.PictureBox();
            this.orangeTile = new System.Windows.Forms.PictureBox();
            this.greyTile = new System.Windows.Forms.PictureBox();
            this.greenTile = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.currentTile = new System.Windows.Forms.PictureBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.mapGroup = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.blackTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.orangeTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greyTile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenTile)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.currentTile)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.blackTile);
            this.groupBox1.Controls.Add(this.blueTile);
            this.groupBox1.Controls.Add(this.redTile);
            this.groupBox1.Controls.Add(this.orangeTile);
            this.groupBox1.Controls.Add(this.greyTile);
            this.groupBox1.Controls.Add(this.greenTile);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox1.Location = new System.Drawing.Point(22, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(95, 151);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tile Selector";
            // 
            // blackTile
            // 
            this.blackTile.BackColor = System.Drawing.Color.Black;
            this.blackTile.Location = new System.Drawing.Point(52, 108);
            this.blackTile.Name = "blackTile";
            this.blackTile.Size = new System.Drawing.Size(35, 35);
            this.blackTile.TabIndex = 5;
            this.blackTile.TabStop = false;
            this.blackTile.Click += new System.EventHandler(this.SelectColor);
            // 
            // blueTile
            // 
            this.blueTile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.blueTile.Location = new System.Drawing.Point(6, 108);
            this.blueTile.Name = "blueTile";
            this.blueTile.Size = new System.Drawing.Size(35, 35);
            this.blueTile.TabIndex = 4;
            this.blueTile.TabStop = false;
            this.blueTile.Click += new System.EventHandler(this.SelectColor);
            // 
            // redTile
            // 
            this.redTile.BackColor = System.Drawing.Color.Red;
            this.redTile.Location = new System.Drawing.Point(52, 67);
            this.redTile.Name = "redTile";
            this.redTile.Size = new System.Drawing.Size(35, 35);
            this.redTile.TabIndex = 3;
            this.redTile.TabStop = false;
            this.redTile.Click += new System.EventHandler(this.SelectColor);
            // 
            // orangeTile
            // 
            this.orangeTile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.orangeTile.Location = new System.Drawing.Point(6, 67);
            this.orangeTile.Name = "orangeTile";
            this.orangeTile.Size = new System.Drawing.Size(35, 35);
            this.orangeTile.TabIndex = 2;
            this.orangeTile.TabStop = false;
            this.orangeTile.Click += new System.EventHandler(this.SelectColor);
            // 
            // greyTile
            // 
            this.greyTile.BackColor = System.Drawing.Color.Silver;
            this.greyTile.Location = new System.Drawing.Point(52, 26);
            this.greyTile.Name = "greyTile";
            this.greyTile.Size = new System.Drawing.Size(35, 35);
            this.greyTile.TabIndex = 1;
            this.greyTile.TabStop = false;
            this.greyTile.Click += new System.EventHandler(this.SelectColor);
            // 
            // greenTile
            // 
            this.greenTile.BackColor = System.Drawing.Color.Green;
            this.greenTile.Location = new System.Drawing.Point(6, 26);
            this.greenTile.Name = "greenTile";
            this.greenTile.Size = new System.Drawing.Size(35, 35);
            this.greenTile.TabIndex = 0;
            this.greenTile.TabStop = false;
            this.greenTile.Click += new System.EventHandler(this.SelectColor);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.currentTile);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.groupBox2.Location = new System.Drawing.Point(22, 169);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(87, 89);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Current Tile";
            // 
            // currentTile
            // 
            this.currentTile.BackColor = System.Drawing.Color.Black;
            this.currentTile.Location = new System.Drawing.Point(6, 26);
            this.currentTile.Name = "currentTile";
            this.currentTile.Size = new System.Drawing.Size(75, 57);
            this.currentTile.TabIndex = 6;
            this.currentTile.TabStop = false;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(22, 264);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(110, 84);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save File";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveFile);
            // 
            // loadButton
            // 
            this.loadButton.Location = new System.Drawing.Point(22, 354);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(110, 84);
            this.loadButton.TabIndex = 3;
            this.loadButton.Text = "Load File";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.LoadFile);
            // 
            // mapGroup
            // 
            this.mapGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.mapGroup.Location = new System.Drawing.Point(165, 12);
            this.mapGroup.Name = "mapGroup";
            this.mapGroup.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mapGroup.Size = new System.Drawing.Size(623, 633);
            this.mapGroup.TabIndex = 4;
            this.mapGroup.TabStop = false;
            this.mapGroup.Text = "Map";
            // 
            // LevelEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 657);
            this.Controls.Add(this.mapGroup);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "LevelEditor";
            this.Text = "LevelEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LevelEditor_FormClosing);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.blackTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.orangeTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greyTile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenTile)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.currentTile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button saveButton;
        private Button loadButton;
        private GroupBox mapGroup;
        private PictureBox blackTile;
        private PictureBox blueTile;
        private PictureBox redTile;
        private PictureBox orangeTile;
        private PictureBox greyTile;
        private PictureBox greenTile;
        private PictureBox currentTile;
    }
}