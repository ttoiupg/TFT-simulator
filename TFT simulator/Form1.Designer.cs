namespace TFT_simulator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            panel1 = new Panel();
            ControlLabel = new Label();
            label1 = new Label();
            MousePositionLabel = new Label();
            PanningLabel = new Label();
            ResetZoomButton = new Button();
            ZoomLabel = new Label();
            CanvasControl = new TftCanvasControl();
            propertyGrid1 = new PropertyGrid();
            listBox1 = new ListBox();
            toolStrip1 = new ToolStrip();
            AddRectButton = new ToolStripButton();
            AddCircleButton = new ToolStripButton();
            AddLineButton = new ToolStripButton();
            AddTextButton = new ToolStripButton();
            ExportButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            BackgroundColorButton = new ToolStripButton();
            BackgroundColorPicker = new ColorDialog();
            panel1.SuspendLayout();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(ControlLabel);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(MousePositionLabel);
            panel1.Controls.Add(PanningLabel);
            panel1.Controls.Add(ResetZoomButton);
            panel1.Controls.Add(ZoomLabel);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 515);
            panel1.Name = "panel1";
            panel1.Size = new Size(954, 24);
            panel1.TabIndex = 0;
            // 
            // ControlLabel
            // 
            ControlLabel.BackColor = Color.Black;
            ControlLabel.BorderStyle = BorderStyle.FixedSingle;
            ControlLabel.Dock = DockStyle.Fill;
            ControlLabel.Font = new Font("Noto Sans TC", 8.249999F, FontStyle.Bold, GraphicsUnit.Point, 136);
            ControlLabel.ForeColor = Color.FromArgb(250, 252, 252);
            ControlLabel.Location = new Point(249, 0);
            ControlLabel.Name = "ControlLabel";
            ControlLabel.Size = new Size(373, 24);
            ControlLabel.TabIndex = 5;
            ControlLabel.Text = "Left to select Right to pan";
            ControlLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Left;
            label1.Font = new Font("Noto Sans TC", 8.249999F);
            label1.ForeColor = Color.FromArgb(33, 33, 33);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(249, 24);
            label1.TabIndex = 4;
            label1.Text = "Made by HALFMOON";
            label1.TextAlign = ContentAlignment.BottomLeft;
            label1.Click += label1_Click;
            // 
            // MousePositionLabel
            // 
            MousePositionLabel.BackColor = Color.Transparent;
            MousePositionLabel.Dock = DockStyle.Right;
            MousePositionLabel.Font = new Font("Noto Sans TC", 8.249999F, FontStyle.Bold);
            MousePositionLabel.ForeColor = Color.FromArgb(33, 33, 33);
            MousePositionLabel.Location = new Point(622, 0);
            MousePositionLabel.Name = "MousePositionLabel";
            MousePositionLabel.Size = new Size(102, 24);
            MousePositionLabel.TabIndex = 3;
            MousePositionLabel.Text = "label1";
            MousePositionLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // PanningLabel
            // 
            PanningLabel.BackColor = Color.Transparent;
            PanningLabel.Dock = DockStyle.Right;
            PanningLabel.Font = new Font("Noto Sans TC", 8.249999F, FontStyle.Bold);
            PanningLabel.ForeColor = Color.FromArgb(33, 33, 33);
            PanningLabel.Location = new Point(724, 0);
            PanningLabel.Name = "PanningLabel";
            PanningLabel.Size = new Size(108, 24);
            PanningLabel.TabIndex = 2;
            PanningLabel.Text = "label1";
            PanningLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // ResetZoomButton
            // 
            ResetZoomButton.Dock = DockStyle.Right;
            ResetZoomButton.Font = new Font("Noto Sans TC", 8.249999F);
            ResetZoomButton.Location = new Point(832, 0);
            ResetZoomButton.Name = "ResetZoomButton";
            ResetZoomButton.Size = new Size(61, 24);
            ResetZoomButton.TabIndex = 1;
            ResetZoomButton.Text = "Reset";
            ResetZoomButton.UseVisualStyleBackColor = true;
            ResetZoomButton.Click += ResetZoomButton_Click;
            // 
            // ZoomLabel
            // 
            ZoomLabel.BackColor = Color.Transparent;
            ZoomLabel.Dock = DockStyle.Right;
            ZoomLabel.Font = new Font("Noto Sans TC", 8.249999F, FontStyle.Bold);
            ZoomLabel.ForeColor = Color.FromArgb(33, 33, 33);
            ZoomLabel.Location = new Point(893, 0);
            ZoomLabel.Name = "ZoomLabel";
            ZoomLabel.Size = new Size(61, 24);
            ZoomLabel.TabIndex = 0;
            ZoomLabel.Text = "label1";
            ZoomLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // CanvasControl
            // 
            CanvasControl.BackColor = Color.Black;
            CanvasControl.Dock = DockStyle.Fill;
            CanvasControl.Location = new Point(132, 44);
            CanvasControl.Name = "CanvasControl";
            CanvasControl.Size = new Size(822, 471);
            CanvasControl.TabIndex = 2;
            CanvasControl.Text = "tftCanvasControl1";
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Right;
            propertyGrid1.Location = new Point(748, 44);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(206, 471);
            propertyGrid1.TabIndex = 3;
            // 
            // listBox1
            // 
            listBox1.Dock = DockStyle.Left;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(0, 44);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(132, 471);
            listBox1.TabIndex = 4;
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.Items.AddRange(new ToolStripItem[] { AddRectButton, AddCircleButton, AddLineButton, AddTextButton, ExportButton, toolStripSeparator1, BackgroundColorButton });
            toolStrip1.LayoutStyle = ToolStripLayoutStyle.Flow;
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.System;
            toolStrip1.Size = new Size(954, 44);
            toolStrip1.TabIndex = 5;
            toolStrip1.Text = "toolStrip1";
            // 
            // AddRectButton
            // 
            AddRectButton.AutoSize = false;
            AddRectButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            AddRectButton.Image = Properties.Resources.Vector;
            AddRectButton.ImageScaling = ToolStripItemImageScaling.None;
            AddRectButton.ImageTransparentColor = Color.Magenta;
            AddRectButton.Margin = new Padding(10, 1, 0, 2);
            AddRectButton.Name = "AddRectButton";
            AddRectButton.Size = new Size(41, 41);
            AddRectButton.Text = "toolStripButton1";
            AddRectButton.ToolTipText = "Add Rectangle";
            AddRectButton.Click += AddRectButton_Click;
            // 
            // AddCircleButton
            // 
            AddCircleButton.AutoSize = false;
            AddCircleButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            AddCircleButton.Image = Properties.Resources.Vector_1;
            AddCircleButton.ImageScaling = ToolStripItemImageScaling.None;
            AddCircleButton.ImageTransparentColor = Color.Magenta;
            AddCircleButton.Name = "AddCircleButton";
            AddCircleButton.Size = new Size(41, 41);
            AddCircleButton.Text = "toolStripButton2";
            AddCircleButton.ToolTipText = "Add Circle";
            AddCircleButton.Click += AddCircleButton_Click;
            // 
            // AddLineButton
            // 
            AddLineButton.AutoSize = false;
            AddLineButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            AddLineButton.Image = Properties.Resources.Vector_3;
            AddLineButton.ImageScaling = ToolStripItemImageScaling.None;
            AddLineButton.ImageTransparentColor = Color.Magenta;
            AddLineButton.Name = "AddLineButton";
            AddLineButton.Size = new Size(41, 41);
            AddLineButton.Text = "toolStripButton3";
            AddLineButton.ToolTipText = "Add Line";
            AddLineButton.Click += AddLineButton_Click;
            // 
            // AddTextButton
            // 
            AddTextButton.AutoSize = false;
            AddTextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            AddTextButton.Image = Properties.Resources.Vector_2;
            AddTextButton.ImageScaling = ToolStripItemImageScaling.None;
            AddTextButton.ImageTransparentColor = Color.Magenta;
            AddTextButton.Name = "AddTextButton";
            AddTextButton.Size = new Size(41, 41);
            AddTextButton.Text = "toolStripButton4";
            AddTextButton.ToolTipText = "Add Text";
            AddTextButton.Click += AddTextButton_Click;
            // 
            // ExportButton
            // 
            ExportButton.AutoSize = false;
            ExportButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ExportButton.Image = Properties.Resources.export;
            ExportButton.ImageScaling = ToolStripItemImageScaling.None;
            ExportButton.ImageTransparentColor = Color.Magenta;
            ExportButton.Margin = new Padding(0, 1, 10, 2);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new Size(41, 41);
            ExportButton.Text = "toolStripButton5";
            ExportButton.ToolTipText = "Export to Code";
            ExportButton.Click += ExportButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.AutoSize = false;
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(10, 41);
            // 
            // BackgroundColorButton
            // 
            BackgroundColorButton.AutoSize = false;
            BackgroundColorButton.BackColor = Color.White;
            BackgroundColorButton.BackgroundImageLayout = ImageLayout.Center;
            BackgroundColorButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BackgroundColorButton.Font = new Font("Noto Sans TC", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            BackgroundColorButton.Image = Properties.Resources.palette;
            BackgroundColorButton.ImageScaling = ToolStripItemImageScaling.None;
            BackgroundColorButton.ImageTransparentColor = Color.Magenta;
            BackgroundColorButton.Name = "BackgroundColorButton";
            BackgroundColorButton.Size = new Size(41, 41);
            BackgroundColorButton.Text = "Change Background Color";
            BackgroundColorButton.Click += toolStripButton1_Click;
            // 
            // BackgroundColorPicker
            // 
            BackgroundColorPicker.AnyColor = true;
            BackgroundColorPicker.Color = Color.White;
            BackgroundColorPicker.FullOpen = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(954, 539);
            Controls.Add(propertyGrid1);
            Controls.Add(CanvasControl);
            Controls.Add(listBox1);
            Controls.Add(panel1);
            Controls.Add(toolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TFT simulator";
            panel1.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label ZoomLabel;
        private Button ResetZoomButton;
        private Label PanningLabel;
        private TftCanvasControl CanvasControl;
        private Label MousePositionLabel;
        private Label label1;
        private Label ControlLabel;
        private PropertyGrid propertyGrid1;
        private ListBox listBox1;
        private ToolStrip toolStrip1;
        private ToolStripButton AddCircle;
        private ToolStripButton AddRect;
        private ToolStripButton AddLine;
        private ToolStripButton AddText;
        private ToolStripButton AddRectButton;
        private ToolStripButton AddCircleButton;
        private ToolStripButton AddLineButton;
        private ToolStripButton AddTextButton;
        private ToolStripButton ExportButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton BackgroundColorButton;
        private ColorDialog BackgroundColorPicker;
    }
}
