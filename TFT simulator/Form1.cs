using System;
using Microsoft.VisualBasic; // add at top

namespace TFT_simulator
{
    public partial class Form1 : Form
    {
        private PropertyGrid _grid;
        private ListBox _list;
        private ToolStrip _tools;
        private int _index = -1;
        void InitUi()
        {
            _tools = new ToolStrip();
            var addRect = new ToolStripButton("Add Rect", null, (_, __) => AddRect());
            var addCircle = new ToolStripButton("Add Circle", null, (_, __) => AddCircle());
            var addLine = new ToolStripButton("Add Line", null, (_, __) => AddLine());
            var addText = new ToolStripButton("Add Text", null, (_, __) => AddText());
            var outputButton = new ToolStripButton("Output", null,(_, __) => OpenOutputFrame(CanvasControl.Elements));
            _tools.Items.AddRange(new ToolStripItem[] { addRect, addCircle, addLine, addText,outputButton });

            _list = new ListBox { Dock = DockStyle.Left, Width = 120 };
            _grid = new PropertyGrid { Dock = DockStyle.Right, Width = 200 };
            Controls.Add(_grid);
            Controls.Add(_list);
            Controls.Add(_tools);

            _list.SelectedIndexChanged += (_, __) =>
            {
                _grid.SelectedObject = _list.SelectedItem; // auto shows type-specific props
            };
            _list.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete && _list.SelectedItem is TftElement el)
                {
                    CanvasControl.Elements.Remove(el);
                    CanvasControl.RenderObjects();

                    _list.DataSource = null;
                    _list.DataSource = CanvasControl.Elements;
                }
            }; ;
            _grid.PropertyValueChanged += (_, __) => CanvasControl.RenderObjects();
        }


        public Form1()
        {
            InitializeComponent();
            CanvasControl.OnZoomChanged += (value) =>
            {
                ZoomLabel.Text = $"x{value}";
            };
            CanvasControl.OnMove += (value) =>
            {
                var x = value.X.ToString("0.0");
                var y = value.Y.ToString("0.0");
                PanningLabel.Text = $"x:{x} y:{y}";
            };
            // Sample objects
            CanvasControl.TftBackground = Color.White;
            CanvasControl.RenderObjects();
            CanvasControl.ResetView();
            InitUi();
            panel1.SendToBack();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void OpenOutputFrame(List<TftElement> elements)
        {
            string prefix = Interaction.InputBox("Enter code prefix:", "Output to code", "g");
            if (string.IsNullOrWhiteSpace(prefix))
                return; // user canceled
            OutputForm f = new OutputForm(prefix, elements);
            f.ShowDialog();
        }
        void BindCanvas(TftCanvasControl canvas)
        {
            CanvasControl = canvas;
            _list.DataSource = null;
            _list.DataSource = CanvasControl.Elements;
        }
        void AddRect()
        {
            var e = new RectElement {Name = $"Rect {CanvasControl.Elements.Count}", Position = new Point(8, 8), Size = new Size(40, 24), IsFilled = true, Color = Color.Red };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }
        void AddCircle()
        {
            var e = new CircleElement { Name = $"Circle {CanvasControl.Elements.Count}", Position = new Point(60, 40), Radius = 16, IsFilled = false, Color = Color.Lime };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }
        void AddLine()
        {
            var e = new LineElement { Name = $"Line {CanvasControl.Elements.Count}", Position = new Point(0, 0), End = new Point(159, 127), Thickness = 1, Color = Color.Cyan };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }
        void AddText()
        {
            var e = new TextElement { Name = $"Text {CanvasControl.Elements.Count}", Position = new Point(10, 100), Size = new Size(60, 10), Text = "Placeholder", Color = Color.Black };
            CanvasControl.AddElement(e);
            RefreshListSelect(e);
        }

        void RefreshListSelect(object item)
        {
            // Rebind so ListBox refreshes display
            _list.DataSource = null;
            _list.DataSource = CanvasControl.Elements;
            _list.SelectedItem = item;
            CanvasControl.RenderObjects();
        }

        private void ResetZoomButton_Click(object sender, EventArgs e)
        {
            CanvasControl.ResetView();
        }

        //private void MenuButton_Click(object sender, EventArgs e)
        //{
        //    if (MenuPanel.Visible)
        //    {
        //        MenuPanel.Hide();
        //    }
        //    else
        //    {
        //        MenuPanel.Show();
        //    }
        //}

        private void AddCircleButton_Click(object sender, EventArgs e)
        {
            var c = new CircleElement
            {
                Position = new Point(90, 64), // center
                Radius = 18,
                IsFilled = false,
                Thickness = 1,
                Color = Color.Lime
            };
            CanvasControl.AddElement(c);
        }

        private void AddRectangleButton_Click(object sender, EventArgs e)
        {
            var r = new RectElement
            {
                Position = new Point(10, 10),
                Size = new Size(40, 20),
                IsFilled = true,
                Color = Color.Red
            };
            CanvasControl.AddElement(r);
        }

        private void AddLineButton_Click(object sender, EventArgs e)
        {
            var ln = new LineElement
            {
                Position = new Point(0, 0),
                End = new Point(159, 127),
                Thickness = 1,
                Color = Color.Cyan
            };
            CanvasControl.AddElement(ln);
        }

        private void AddTextButton_Click(object sender, EventArgs e)
        {
            var txt = new TextElement
            {
                Position = new Point(5, 100),
                Size = new Size(60, 10),
                Text = "Placeholder",
                Color = Color.White
            };
            CanvasControl.AddElement(txt);
        }
    }
}
