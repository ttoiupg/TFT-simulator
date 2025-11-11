using System;
using Microsoft.VisualBasic; // add at top

namespace TFT_simulator
{
    public partial class Form1 : Form
    {
        private int _index = -1;
        void InitUi()
        {
            TftCanvasControl.ElementPropertyChanged += propertyGrid1.Refresh;

            listBox1.SelectedIndexChanged += (_, __) =>
            {
                propertyGrid1.SelectedObject = listBox1.SelectedItem; // auto shows type-specific props
            };
            listBox1.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete && listBox1.SelectedItem is TftElement el)
                {
                    CanvasControl.RemoveElement(el);
                    RefreshListbox();
                }
            };
            propertyGrid1.PropertyValueChanged += (_, __) => CanvasControl.RenderObjects();
        }


        public Form1()
        {
            InitializeComponent();
            CanvasControl.OnZoomChanged += (value) =>
            {
                ZoomLabel.Text = $"x{value}";
            };
            CanvasControl.OnPan += (value) =>
            {
                var x = value.X.ToString("0.0");
                var y = value.Y.ToString("0.0");
                PanningLabel.Text = $"x:{x} y:{y}";
            };
            CanvasControl.OnPointerMove += (value) =>
            {
                var x = Math.Clamp(value.X, 0, CanvasControl.TftWidth).ToString("0.0");
                var y = Math.Clamp(value.Y, 0, CanvasControl.TftHeight).ToString("0.0");
                MousePositionLabel.Text = $"x:{x} y:{y}";
            };
            CanvasControl.refreshListbox += RefreshListbox;
            CanvasControl.SelectElement += (element) =>
            {
                listBox1.SelectedItem = element;
                propertyGrid1.SelectedObject = listBox1.SelectedItem;
            };
            CanvasControl.OnDrag += (value) =>
            {
                if (!value) ControlLabel.Text = "Left mouse to select Right to pan";
            };
            CanvasControl.Dragging += (value) =>
            {
                ControlLabel.Text = $"Offset : x:{value.X} y:{value.Y}";
            };
            //CanvasControl.UpdateProperty += RefreshListbox;
            // Sample objects
            CanvasControl.TftBackground = Color.White;
            CanvasControl.RenderObjects();
            CanvasControl.ResetView();
            InitUi();
            panel1.SendToBack();
        }
        void OpenOutputFrame(List<TftElement> elements)
        {
            string prefix = Interaction.InputBox("Enter code prefix:", "Output to code", "g");
            if (string.IsNullOrWhiteSpace(prefix))
                return; // user canceled
            OutputForm f = new OutputForm(prefix, elements);
            f.ShowDialog();
        }
        void RefreshListbox()
        {
            var obj = listBox1.SelectedItem;
            listBox1.DataSource = null;
            listBox1.DataSource = CanvasControl.Elements;
            listBox1.SelectedItem = obj;
        }
        void AddRectToCanva()
        {
            var e = new RectElement { Name = $"Rect {CanvasControl.Elements.Count}", Position = new Point(8, 8), Size = new Size(40, 24), IsFilled = true, Color = Color.Red };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }
        void AddCircleToCanva()
        {
            var e = new CircleElement { Name = $"Circle {CanvasControl.Elements.Count}", Position = new Point(60, 40), Radius = 16, IsFilled = false, Color = Color.Lime };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }
        void AddLineToCanva()
        {
            var e = new LineElement { Name = $"Line {CanvasControl.Elements.Count}", Position = new Point(50, 50), End = new Point(50, 70), Thickness = 1, Color = Color.Cyan };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }
        void AddTextToCanva()
        {
            var e = new TextElement { Name = $"Text {CanvasControl.Elements.Count}", Position = new Point(10, 100), Size = new Size(60, 10), Text = "Placeholder", Color = Color.Black };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }

        void RefreshListSelect(object item)
        {
            // Rebind so ListBox refreshes display
            RefreshListbox();
            listBox1.SelectedItem = item;
            CanvasControl.RenderObjects();
        }

        private void ResetZoomButton_Click(object sender, EventArgs e)
        {
            CanvasControl.ResetView();
        }

        private void AddRectButton_Click(object sender, EventArgs e)
        {
            AddRectToCanva();
        }

        private void AddCircleButton_Click(object sender, EventArgs e)
        {
            AddCircleToCanva();
        }

        private void AddLineButton_Click(object sender, EventArgs e)
        {
            AddLineToCanva();
        }

        private void AddTextButton_Click(object sender, EventArgs e)
        {
            AddTextToCanva();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            OpenOutputFrame(CanvasControl.Elements);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
