namespace LevelEditor
{
    partial class MapCreator
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
            this.loadMapButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tilesHeightTextBox = new System.Windows.Forms.TextBox();
            this.tilesWidthTextBox = new System.Windows.Forms.TextBox();
            this.createMap = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // loadMapButton
            // 
            this.loadMapButton.Location = new System.Drawing.Point(12, 12);
            this.loadMapButton.Name = "loadMapButton";
            this.loadMapButton.Size = new System.Drawing.Size(318, 87);
            this.loadMapButton.TabIndex = 0;
            this.loadMapButton.Text = "Load Map";
            this.loadMapButton.UseVisualStyleBackColor = true;
            this.loadMapButton.Click += new System.EventHandler(this.LoadMap);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tilesHeightTextBox);
            this.groupBox1.Controls.Add(this.tilesWidthTextBox);
            this.groupBox1.Controls.Add(this.createMap);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 119);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 221);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Create New Map";
            // 
            // tilesHeightTextBox
            // 
            this.tilesHeightTextBox.Location = new System.Drawing.Point(141, 88);
            this.tilesHeightTextBox.Name = "tilesHeightTextBox";
            this.tilesHeightTextBox.Size = new System.Drawing.Size(125, 27);
            this.tilesHeightTextBox.TabIndex = 4;
            // 
            // tilesWidthTextBox
            // 
            this.tilesWidthTextBox.Location = new System.Drawing.Point(141, 46);
            this.tilesWidthTextBox.Name = "tilesWidthTextBox";
            this.tilesWidthTextBox.Size = new System.Drawing.Size(125, 27);
            this.tilesWidthTextBox.TabIndex = 3;
            // 
            // createMap
            // 
            this.createMap.Location = new System.Drawing.Point(29, 138);
            this.createMap.Name = "createMap";
            this.createMap.Size = new System.Drawing.Size(258, 66);
            this.createMap.TabIndex = 2;
            this.createMap.Text = "Create Map";
            this.createMap.UseVisualStyleBackColor = true;
            this.createMap.Click += new System.EventHandler(this.CreateMap);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Height (in tiles)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width (in tiles)";
            // 
            // MapCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 350);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.loadMapButton);
            this.Name = "MapCreator";
            this.Text = "Level Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Button loadMapButton;
        private GroupBox groupBox1;
        private TextBox tilesHeightTextBox;
        private TextBox tilesWidthTextBox;
        private Button createMap;
        private Label label2;
        private Label label1;
    }
}