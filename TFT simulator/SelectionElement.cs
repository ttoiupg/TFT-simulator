// SelectionElement.cs
namespace TFT_simulator
{
    public sealed class SelectionElement
    {
        public Point Position { get; set; }
        public Point End { get; set; }
        public void Draw(Graphics g)
        {
            using var pen = new Pen(Color.LightBlue, 1);
            using var brush = new SolidBrush(Color.FromArgb(40,30,150,255));
            var size = new Size(Math.Abs(End.X - Position.X), Math.Abs(End.Y - Position.Y));
            var pos = Position;
            if (End.X < Position.X)
            {
                pos.X = End.X;
            };
            if (End.Y < Position.Y)
            {
                pos.Y = End.Y;
            }
                var r = new Rectangle(pos, size);
            g.FillRectangle(brush, r);
            g.DrawRectangle(pen, r);
        }
        public Rectangle GetRegion()
        {
            var size = new Size(Math.Abs(End.X - Position.X), Math.Abs(End.Y - Position.Y));
            var pos = Position;
            if (End.X < Position.X)
            {
                pos.X = End.X;
            }
            ;
            if (End.Y < Position.Y)
            {
                pos.Y = End.Y;
            }
            var r = new Rectangle(pos, size);
            return r;
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
