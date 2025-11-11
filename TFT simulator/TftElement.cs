// TftElement.cs
using System.ComponentModel;

namespace TFT_simulator
{
    // ---- Elements ----
    public abstract class TftElement : INotifyPropertyChanged
    {
        [Category("Data")] public string Name { get; set; } = "";
        [Category("Transform")] public Point Position { get; set; } = new Point(0, 0);
        [Category("Transform")] public Size Size { get; set; } = Size.Empty;
        [Category("Transform")] public int Zindex { get; set; } = 0;
        [Category("Appearance")] public Color Color { get; set; } = Color.Black;
        [Category("Appearance")] public int Thickness { get; set; } = 1;
        [Browsable(false)] public string Text { get; set; } = string.Empty;
        [Browsable(false)] public int FontSize { get; set; } = 1;
        [Browsable(false)] public int Radius { get; set; } = 1;
        [Category("Appearance")] public bool IsFilled { get; set; } = false;
        [Browsable(false)] public bool IsDragging { get; set; } = false;
        [Browsable(false)] public int? currentHandleIndex { get; set; } = null;

        public event PropertyChangedEventHandler? PropertyChanged;

        public override string ToString() => $"{GetType().Name}";
        public abstract string Serialize(string prefix);
        public abstract bool IsPointInside(Point point);
        public abstract void Draw(Graphics g);
        public virtual void DrawHandle(Graphics g)
        {
            var brush = new SolidBrush(Color.Black);
            var size = new Size(1, 1);
            foreach (var point in GetHandles())
            {
                var r = new Rectangle(point, size);
                g.FillEllipse(brush, r);
            }
            brush.Dispose();
        }
        public abstract void UpdateSelect(Point mousePos,Point delta,bool forceMove);
        public virtual List<Point> GetHandles()
        {
            return new List<Point>();
        }
        public virtual int? GetSelectedHandle(Point mousePos, int scale, Point offset)
        {
            var h = GetHandles();
            var closest = 9000.0d;
            int? output = null;
            for (int i = 0; i < h.Count; i++)
            {
                var dist = Util.GetPointDistance(mousePos, Util.CanvasToScreenPos(h[i], offset, scale));
                if ((dist < 15 || (scale < 2 && dist < 8)) && dist < closest)
                {
                    closest = dist;
                    output = i;
                }
            }
            System.Diagnostics.Debug.Write($"{closest} {output}");
            return output;
        }
        public abstract void StartDrag();
        public abstract void EndDrag();

        public abstract Rectangle GetRect();
    }
}

/*
USAGE EXAMPLE (e.g., in Form1.cs):

public partial class Form1 : Form
{
    private readonly TftSim.TftCanvasControl _canvas = new TftSim.TftCanvasControl();

    public Form1()
    {
        InitializeComponent();

        _canvas.Dock = DockStyle.Fill;
        Controls.Add(_canvas);

        // Sample objects
        var r = new TftSim.RectElement
        {
            Position = new Point(10, 10),
            Size = new Size(40, 20),
            IsFilled = true,
            Color = Color.Red
        };
        var c = new TftSim.CircleElement
        {
            Position = new Point(90, 64), // center
            Radius = 18,
            IsFilled = false,
            Thickness = 1,
            Color = Color.Lime
        };
        var ln = new TftSim.LineElement
        {
            Position = new Point(0, 0),
            End = new Point(159, 127),
            Thickness = 1,
            Color = Color.Cyan
        };
        var txt = new TftSim.TextElement
        {
            Position = new Point(5, 100),
            Size = new Size(60, 10),
            Text = "Placeholder",
            Color = Color.White
        };

        _canvas.AddElement(r);
        _canvas.AddElement(c);
        _canvas.AddElement(ln);
        _canvas.AddElement(txt);

        _canvas.RenderObjects();
        _canvas.CenterView();
    }
    // _canvas.RenderObjects();
}
*/
