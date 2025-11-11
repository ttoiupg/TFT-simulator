// RectElement.cs
using System.Text;

namespace TFT_simulator
{
    public sealed class RectElement : TftElement
    {
        private Point prevPosition;
        private Size prevSize;
        public override void Draw(Graphics g)
        {
            using var pen = new Pen(Color, Thickness);
            using var brush = new SolidBrush(Color);
            var r = new Rectangle(Position, Size);
            var rline = new Rectangle(Position + new Size(1,1), Size - new Size(1, 1));
            if (IsFilled) g.FillRectangle(brush, r);
            else g.DrawRectangle(pen, rline);
        }
        public override string Serialize(string prefix)
        {
            var sb = new StringBuilder();
            if (IsFilled)
            {
                sb.AppendLine(prefix + $".fillRect({Position.X},{Position.Y},{Size.Width},{Size.Height},{Util.ToRgb565String(Color)});");
            }
            else
            {
                sb.AppendLine(prefix + $".drawRect({Position.X},{Position.Y},{Size.Width},{Size.Height},{Util.ToRgb565String(Color)});");
            }
            return sb.ToString();
        }
        public override bool IsPointInside(Point point)
        {
            return Util.IsPointInsideRect(point, new Rectangle(Position, Size));
        }
        public override List<Point> GetHandles()
        {
            return new List<Point>
                    {
                        Position,
                        Util.AddPoints(Position,new Point(Size.Width/2-1,0)),
                        Util.AddPoints(Position, new Point(Size.Width-1, 0)),
                        Util.AddPoints(Position, new Point(Size.Width-1, Size.Height / 2)),
                        Position + Size + new Size(-1,-1),
                        Util.AddPoints(Position, new Point(Size.Width / 2, Size.Height-1)),
                        Util.AddPoints(Position, new Point(0, Size.Height - 1)),
                        Util.AddPoints(Position, new Point(0, Size.Height/2-1))
                    };
        }
        public override Rectangle GetRect()
        {
            return new Rectangle(Position, Size);
        }
        public override void StartDrag()
        {
            prevPosition = Position;
            prevSize = Size;
            IsDragging = true;
        }
        public override void EndDrag()
        {
            IsDragging = false;
        }
        public override void UpdateSelect(Point mousePos, Point startPos, bool f)
        {
            var offset = Util.GetPointOffset(mousePos, startPos);
            if (currentHandleIndex == null || f) {
                Position = Util.AddPoints(prevPosition, offset);
                return;
            }
            var pos = Position;
            var si = Size;
            switch (currentHandleIndex) {
                case 0://origin
                    Position = mousePos;
                    si.Width = prevSize.Width - offset.X;
                    si.Height = prevSize.Height - offset.Y;
                    Size = si;
                    break;
                case 1://top middle
                    pos.Y = mousePos.Y;
                    Position = pos;
                    si.Height = prevSize.Height - offset.Y;
                    Size = si;
                    break;
                case 2://top right
                    pos.Y = mousePos.Y;
                    Position = pos;
                    si.Width = prevSize.Width + offset.X;
                    si.Height = prevSize.Height - offset.Y;
                    Size = si;
                    break;
                case 3://middle right
                    si.Width = prevSize.Width + offset.X;
                    Size = si;
                    break;
                case 4://bottom right 
                    si.Width = prevSize.Width + offset.X;
                    si.Height = prevSize.Height + offset.Y;
                    Size = si;
                    break;
                case 5://bottom middle
                    si.Height = prevSize.Height + offset.Y;
                    Size = si;
                    break;
                case 6://bottom left
                    pos.X = mousePos.X;
                    Position = pos;
                    si.Height = prevSize.Height + offset.Y;
                    si.Width = prevSize.Width - offset.X;
                    Size = si;
                    break;
                case 7://middle left
                    pos.X = mousePos.X;
                    Position = pos;
                    si.Width = prevSize.Width - offset.X;
                    Size = si;
                    break;
            };
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
