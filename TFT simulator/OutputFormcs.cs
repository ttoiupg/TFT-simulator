using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TFT_simulator
{
    public partial class OutputForm : Form
    {
        public OutputForm(string? prefix,List<TftElement> tftElements)
        {
            var pre = (prefix == null || prefix.Length == 0) ? "g" : prefix;
            InitializeComponent();
            string outputText = string.Empty;
            var sb = new StringBuilder();
            foreach (var element in tftElements){
                sb.AppendLine($"// {element.Name}");
                sb.AppendLine(element.Serialize(prefix));
            }

            textBox1.Text = sb.ToString();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
