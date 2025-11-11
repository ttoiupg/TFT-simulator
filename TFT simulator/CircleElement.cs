// CircleElement.cs
using System.ComponentModel;
using System.Text;

namespace TFT_simulator
{
    public sealed class CircleElement : TftElement
    {
        private Point prevPosition;
        private int prevRadius;
        [Category("Shape")][Browsable(true)] public new int Radius { get; set; }
        [Browsable(false)] public new Size Size { get; set; } // not used here
        // Position = center; Radius in pixels
        public override void Draw(Graphics g)
        {
            int d = Radius * 2 +1;
            var r = new Rectangle(Position.X - Radius, Position.Y - Radius, d, d);
            using var pen = new Pen(Color, Thickness);
            using var brush = new SolidBrush(Color);
            if (IsFilled) g.FillEllipse(brush, r);
            else g.DrawEllipse(pen, r);
        }
        public override string Serialize(string prefix)
        {
            var sb = new StringBuilder();
            if (IsFilled)
            {
                sb.AppendLine(prefix + $".fillCircle({Position.X},{Position.Y},{Radius},{Util.ToRgb565String(Color)});");
            }
            else
            {
                sb.AppendLine(prefix + $".drawCircle({Position.X},{Position.Y},{Radius},{Util.ToRgb565String(Color)});");
            }
            return sb.ToString();
        }
        public override void StartDrag()
        {
            prevPosition = Position;
            prevRadius = Radius;
            IsDragging = true;
        }
        public override void EndDrag()
        {
            IsDragging = false;
        }
        public override List<Point> GetHandles()
        {
            return new List<Point>
                    {
                Position,
                        Util.AddPoints(Position, new Point(Radius, 0)),
                    }
            ;
        }
        public override Rectangle GetRect()
        {
            int d = Radius * 2 + 1;
            var r = new Rectangle(Position.X - Radius, Position.Y - Radius, d, d);
            return r;
        }
        public override bool IsPointInside(Point point)
        {
            return (Math.Pow(point.X - Position.X,2) + Math.Pow(point.Y - Position.Y,2)) <= Math.Pow(Radius,2);
        }
        public override void UpdateSelect(Point mousePos, Point delta,bool f)
        {
            var offset = Util.GetPointOffset(mousePos,delta);
            if (currentHandleIndex == null || f) {
                Position = Util.AddPoints(prevPosition, offset);
                return;
            }
            switch (currentHandleIndex)
            {
                case 0:
                    this.Position = mousePos;
                    break;
                case 1:
                    this.Radius = (int)Util.GetPointDistance(Position, mousePos);
                    break;
            }
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
