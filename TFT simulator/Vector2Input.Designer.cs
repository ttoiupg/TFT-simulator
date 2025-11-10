namespace TFT_simulator
{
    partial class Vector2Input
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Yinput = new NumericUpDown();
            Xinput = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)Yinput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Xinput).BeginInit();
            SuspendLayout();
            // 
            // Yinput
            // 
            Yinput.Dock = DockStyle.Right;
            Yinput.Location = new Point(137, 0);
            Yinput.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            Yinput.Name = "Yinput";
            Yinput.Size = new Size(76, 23);
            Yinput.TabIndex = 0;
            Yinput.Value = new decimal(new int[] { 10, 0, 0, 0 });
            Yinput.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // Xinput
            // 
            Xinput.Dock = DockStyle.Right;
            Xinput.Location = new Point(20, 0);
            Xinput.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            Xinput.Name = "Xinput";
            Xinput.Size = new Size(75, 23);
            Xinput.TabIndex = 0;
            Xinput.Value = new decimal(new int[] { 10, 0, 0, 0 });
            Xinput.ValueChanged += Xinput_ValueChanged;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Right;
            label1.Font = new Font("Noto Sans TC Medium", 9.75F, FontStyle.Bold);
            label1.Location = new Point(-22, 0);
            label1.Name = "label1";
            label1.Size = new Size(42, 25);
            label1.TabIndex = 1;
            label1.Text = "X";
            label1.TextAlign = ContentAlignment.TopRight;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Right;
            label2.Font = new Font("Noto Sans TC Medium", 9.75F, FontStyle.Bold);
            label2.Location = new Point(95, 0);
            label2.Name = "label2";
            label2.Size = new Size(42, 25);
            label2.TabIndex = 3;
            label2.Text = "Y";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // Vector2Input
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label1);
            Controls.Add(Xinput);
            Controls.Add(label2);
            Controls.Add(Yinput);
            Name = "Vector2Input";
            Size = new Size(213, 25);
            ((System.ComponentModel.ISupportInitialize)Yinput).EndInit();
            ((System.ComponentModel.ISupportInitialize)Xinput).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private NumericUpDown Yinput;
        private NumericUpDown Xinput;
        private Label label1;
        private Label label2;
    }
}
