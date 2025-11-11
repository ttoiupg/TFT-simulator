// LineElement.cs
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Text;

namespace TFT_simulator
{
    public sealed class LineElement : TftElement
    {
        private Point prevPosition;
        private Point prevEnd;
        [Category("Shape")] public Point End { get; set; }
        [Browsable(false)] public new Size Size { get; set; } // not used here
        public override void Draw(Graphics g)
        {
            using var pen = new Pen(Color, Thickness)
            {
                StartCap = LineCap.Square,
                EndCap = LineCap.Square,
                Alignment = PenAlignment.Center
            };
           
            g.DrawLine(pen, Position, End);
        }
        public override string Serialize(string prefix)
        {
            var sb = new StringBuilder();
            sb.AppendLine(prefix + $".drawLine({Position.X},{Position.Y},{End.X},{End.Y},{Util.ToRgb565String(Color)});");
            return sb.ToString();
        }
        public override bool IsPointInside(Point point)
        {
            var dist = Util.DistancePointToSegment(Position, End, point);
            System.Diagnostics.Debug.WriteLine(dist);
            return dist < 7;
        }
        public override Rectangle GetRect()
        {
            var topX = Math.Min(Position.X,End.X);
            var bottomX = Math.Max(Position.X,End.X);
            var topY = Math.Min(Position.Y,End.Y);
            var bottomY = Math.Max(Position.Y, End.Y);
            return new Rectangle(topX, topY, bottomX - topX, bottomY - topY);
        }
        public override List<Point> GetHandles()
        {
           return new List<Point>
                    {
                        Util.AddPoints(Position,new Point(-1,-1)),
                        Util.AddPoints(End,new Point(-1,-1))
                    };
        }
        public override void StartDrag()
        {
            prevPosition = Position;
            prevEnd = End;
            IsDragging = true;
        }
        public override void EndDrag()
        {
            IsDragging = false;
        }
        public override void UpdateSelect(Point mousePos, Point startPos, bool f)
        {
            var offset = Util.GetPointOffset(mousePos, startPos);
            if (currentHandleIndex == null || f) 
            {
                Position = Util.AddPoints(prevPosition, offset);
                End = Util.AddPoints(prevEnd, offset);
                return;
            };
            switch (currentHandleIndex)
            {
                case 0:
                    this.Position = mousePos;
                    break;
                case 1:
                    this.End = mousePos;
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
