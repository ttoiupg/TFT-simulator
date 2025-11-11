// TextElement.cs
using System.ComponentModel;
using System.Text;

namespace TFT_simulator
{
    public sealed class TextElement : TftElement
    {
        [Category("Text")] [Browsable(true)] public new string Text { get; set; } = "";
        [Category("Text")] [Browsable(true)] public new int FontSize { get; set; } = 1;
        [Browsable(false)] public new Size Size { get; set; } // not used here

        private Point prevPosition;
        private Size _size = new Size(6,8);
        // Intentionally blank render per your note; leave the plumbing so controller can fill later
        public override void Draw(Graphics g)
        {
            var size = _size + new Size((Text.Length-1) * _size.Width * FontSize,0);
            var r = new Rectangle(Position, size);
            using var pen = new Pen(Color, 1);
            using var brush = new SolidBrush(Color);
            if (IsFilled) g.FillRectangle(brush, r);
            else g.DrawRectangle(pen, r);
        }
        public override string Serialize(string prefix)
        {
            var sb = new StringBuilder();
            sb.AppendLine(prefix + $".setTextColor({Util.ToRgb565String(Color)});");
            sb.AppendLine(prefix + $".setCursor({Position.X},{Position.Y});");
            sb.AppendLine(prefix + $".println(\"{Text}\");");
            return sb.ToString();
        }
        public override bool IsPointInside(Point point)
        {
            var size = _size + new Size((Text.Length - 1) * _size.Width * FontSize, 0);
            int x = Position.X, y = Position.Y, x2 = x + size.Width, y2 = y + size.Height;
            return (point.X <= x2 && point.Y <= y2 && point.X >= x && point.Y >= y);
        }
        public override int? GetSelectedHandle(Point mousePos, int scale,Point offset)
        {
            return IsPointInside(Util.ScreenToCanvasPosition(mousePos,offset,scale))? 1:null;
        }
        public override void DrawHandle(Graphics g)
        {

        }
        public override Rectangle GetRect()
        {
            var size = _size + new Size((Text.Length - 1) * _size.Width * FontSize, 0);
            return new Rectangle(Position, size);
        }
        public override void StartDrag()
        {
            prevPosition = Position;
            IsDragging = true;
        }
        public override void EndDrag()
        {
            IsDragging = false;
        }
        public override void UpdateSelect(Point mousePos, Point startPos,bool f)
        {
            var offset = Util.GetPointOffset(mousePos, startPos);
            Position = Util.AddPoints(prevPosition, offset);
        }
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
