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
            panel1 = new Panel();
            PanningLabel = new Label();
            ResetZoomButton = new Button();
            ZoomLabel = new Label();
            CanvasControl = new TftCanvasControl();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(PanningLabel);
            panel1.Controls.Add(ResetZoomButton);
            panel1.Controls.Add(ZoomLabel);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 515);
            panel1.Name = "panel1";
            panel1.Size = new Size(954, 24);
            panel1.TabIndex = 0;
            // 
            // PanningLabel
            // 
            PanningLabel.BackColor = Color.Transparent;
            PanningLabel.Dock = DockStyle.Right;
            PanningLabel.Font = new Font("Noto Sans TC", 8.249999F);
            PanningLabel.ForeColor = Color.FromArgb(33, 33, 33);
            PanningLabel.Location = new Point(697, 0);
            PanningLabel.Name = "PanningLabel";
            PanningLabel.Size = new Size(135, 24);
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
            ZoomLabel.Font = new Font("Noto Sans TC", 8.249999F);
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
            CanvasControl.Location = new Point(0, 0);
            CanvasControl.Name = "CanvasControl";
            CanvasControl.Size = new Size(954, 515);
            CanvasControl.TabIndex = 2;
            CanvasControl.Text = "tftCanvasControl1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            ClientSize = new Size(954, 539);
            Controls.Add(CanvasControl);
            Controls.Add(panel1);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TFT simulator";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label ZoomLabel;
        private Button ResetZoomButton;
        private Label PanningLabel;
        private TftCanvasControl CanvasControl;
    }
}
