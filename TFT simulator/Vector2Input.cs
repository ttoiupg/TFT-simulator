using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFT_simulator
{
    public partial class Vector2Input : UserControl
    {
        public Vector2 Value
        {
            get
            {
                return new Vector2((float)Xinput.Value, (float)Yinput.Value);
            }
            set
            {
                Xinput.Value = (decimal)value.X;
                Yinput.Value = (decimal)value.Y;
            }
        }
        public event Action<Vector2> OnValueChanged;
        public Vector2Input()
        {
            InitializeComponent();
            OnValueChanged?.Invoke(Value);
        }
        //Yinput
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged?.Invoke(Value);
        }
        //Xinput
        private void Xinput_ValueChanged(object sender, EventArgs e)
        {
            OnValueChanged?.Invoke(Value);
        }
    }
}
