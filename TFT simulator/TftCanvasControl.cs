// TftCanvasControl.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TFT_simulator
{

    // ---- Elements ----
    public abstract class TftElement
    {
        // Public so a separate controller can mutate them at runtime.
        [Category("Data")]public string Name { get; set; }
        [Category("Transform")] public Point Position { get; set; }
        [Category("Transform")] public Size Size { get; set; }
        [Category("Transform")] public int Zindex { get; set; }
        [Category("Appearance")] public Color Color { get; set; } = Color.Black;
        [Category("Appearance")] public int Thickness { get; set; } = 1;
        [Browsable(false)] public string Text { get; set; } = "";
        [Browsable(false)] public int FontSize { get; set; } = 1;
        [Browsable(false)] public int Radius { get; set; }
        [Category("Appearance")] public bool IsFilled { get; set; }
        public bool IsDragging { get; set; }
        public int? currentHandleIndex { get; set; }
        public override string ToString() => $"{GetType().Name}";
        public abstract string Serialize(string prefix);
        public abstract bool IsPointInside(Point point);
        public abstract void Draw(Graphics g);
        public abstract void DrawHandle(Graphics g);
        public abstract void UpdateSelect(Point mousePos,Point delta);
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
                if (dist < 15 && dist < closest)
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
    }

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
            int x = Position.X, y = Position.Y, x2 = x + Size.Width,y2 = y + Size.Height;
            return (point.X <= x2 && point.Y <= y2 && point.X >= x && point.Y >= y);
        }
        public override List<Point> GetHandles()
        {
            return new List<Point>
                    {
                        Position + new Size(-1,-1),
                        Util.AddPoints(Position,new Point(Size.Width/2-1,0 - 1)),
                        Util.AddPoints(Position, new Point(Size.Width-1, 0 - 1)),
                        Util.AddPoints(Position, new Point(Size.Width-1, Size.Height / 2 - 1)),
                        Position + Size + new Size(-1,-1),
                        Util.AddPoints(Position, new Point(Size.Width / 2 - 1, Size.Height-1)),
                        Util.AddPoints(Position, new Point(0 - 1, Size.Height - 1)),
                        Util.AddPoints(Position, new Point(0 - 1, Size.Height/2-1))
                    };
        }
        public override void StartDrag()
        {
            prevPosition = Position;
            prevSize = Size;
            IsDragging = true;
            currentHandleIndex = null;
        }
        public override void EndDrag()
        {
            IsDragging = false;
        }
        public override void DrawHandle(Graphics g)
        {
            var brush = new SolidBrush(Color.Black);
            var size = new Size(2, 2);
            foreach (var point in GetHandles())
            {
                var r = new Rectangle(point, size);
                g.FillRectangle(brush, r);
            }
            brush.Dispose();
        }
        public override void UpdateSelect(Point mousePos, Point startPos)
        {
            var offset = Util.GetPointOffset(mousePos, startPos);
            if (currentHandleIndex == null) {
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
            currentHandleIndex = null;
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
        public override bool IsPointInside(Point point)
        {
            return (Math.Pow(point.X - Position.X,2) + Math.Pow(point.Y - Position.Y,2)) <= Math.Pow(Radius,2);
        }
        public override void DrawHandle(Graphics g)
        {
            var brush = new SolidBrush(Color.Black);
            var size = new Size(2, 2);
            foreach (var point in GetHandles())
            {
                var r = new Rectangle(point, size);
                g.FillEllipse(brush, r);
            }
            brush.Dispose();
        }
        public override void UpdateSelect(Point mousePos, Point delta)
        {
            var offset = Util.GetPointOffset(mousePos,delta);
            if (currentHandleIndex == null) {
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
            return Util.PointLineDistanceInt(Position.X,Position.Y,End.X,End.Y,point.X,point.Y) < 5;
        }
        public override void DrawHandle(Graphics g)
        {
            var brush = new SolidBrush(Color.Black);
            var size = new Size(2, 2);
            foreach (var point in GetHandles())
            {
                var r = new Rectangle(point, size);
                g.FillEllipse(brush, r);
            }
            brush.Dispose();
        }
        public override List<Point> GetHandles()
        {
           return new List<Point>
                    {
                        Position,
                        End
                    };
        }
        public override void StartDrag()
        {
            prevPosition = Position;
            prevEnd = End;
            currentHandleIndex = null;
            IsDragging = true;
        }
        public override void EndDrag()
        {
            IsDragging = false;
        }
        public override void UpdateSelect(Point mousePos, Point startPos)
        {
            var offset = Util.GetPointOffset(mousePos, startPos);
            if (currentHandleIndex == null) 
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
        public override void StartDrag()
        {
            prevPosition = Position;
            IsDragging = true;
        }
        public override void EndDrag()
        {
            IsDragging = false;
        }
        public override void UpdateSelect(Point mousePos, Point startPos)
        {
            var offset = Util.GetPointOffset(mousePos, startPos);
            Position = Util.AddPoints(prevPosition, offset);
        }
    }
    public sealed class SelectionElement : TftElement
    {
        public Point End { get; set; }
        public override void Draw(Graphics g)
        {
            using var pen = new Pen(Color.LightBlue, Thickness);
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

        public override string Serialize(string prefix)
        {
            return "";
        }
        public override bool IsPointInside(Point point)
        {
            return false;
        }
        public override int? GetSelectedHandle(Point mousePos, int scale, Point offset)
        {
            return null;
        }
        public override void DrawHandle(Graphics g)
        {

        }
        public override void StartDrag()
        {

        }
        public override void EndDrag()
        {
        }
        public override void UpdateSelect(Point mousePos, Point delta)
        {

        }
    }

    // ---- Canvas control ----
    public class TftCanvasControl : Control
    {
        // TFT logical size (ST7735 typical 160x128). Change if you use another variant.
        public int TftWidth { get; private set; } = 160;
        public int TftHeight { get; private set; } = 128;

        // Integer zoom. 1 = 1:1; pixels remain perfect at all integer scales.
        public int Zoom { get; private set; } = 4;

        // Pan offset in device pixels.
        public Point Offset { get; private set; } = new Point(20, 20);

        // Elements to render.
        public readonly List<TftElement> Elements = new List<TftElement>();

        // Background color of TFT buffer.
        public Color TftBackground = Color.Black;

        // Optional border for the TFT area when drawn to the control.
        public bool ShowTftBorder { get; private set; }

        public TftElement? SelectedElement { get; private set; }

        Bitmap _offscreen;
        bool _panning;
        Point _panStartMouse;
        Point _panStartOffset;
        bool _selecting;
        bool _draggingElement;
        SelectionElement _selectionElement;
        Point _selectedOffset;
        Point _dragStartPos;
        public TftCanvasControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Black; // control background
            _selectionElement = new SelectionElement();
            CreateOffscreen();
            // Mouse interactions
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += (_, __) =>{
                _panning = false;
                _selecting = false;
                _draggingElement = false;
                RenderObjects();
            };
            MouseWheel += OnMouseWheel;
        }
        public void RemoveElement(TftElement element)
        {
            Elements.Remove(element);
            if (element == SelectedElement)
            {
                SelectedElement.EndDrag();
                SelectedElement = null;
            }
            RenderObjects();
        }
        public void SetTftSize(int width, int height)
        {
            if (width <= 0 || height <= 0) return;
            TftWidth = width;
            TftHeight = height;
            CreateOffscreen();
            Invalidate();
        }
        public event Action<int> OnZoomChanged;
        public void SetZoom(int zoom, Point? anchorClientPoint = null)
        {
            if (zoom < 1) zoom = 1;
            if (zoom > 64) zoom = 64;

            if (zoom == Zoom) return;

            // Anchor zoom at mouse point to keep focus stable.
            var oldZoom = Zoom;
            var newZoom = zoom;

            if (anchorClientPoint.HasValue)
            {
                var p = anchorClientPoint.Value;
                // Compute where the anchor maps to in TFT pixels before/after zoom and adjust offset.
                // p = Offset + (tft * Zoom)  =>  tft = (p - Offset)/Zoom
                var tftX = (p.X - Offset.X) / oldZoom;
                var tftY = (p.Y - Offset.Y) / oldZoom;
                Offset = new Point(p.X - tftX * newZoom, p.Y - tftY * newZoom);
            }

            Zoom = newZoom;
            OnZoomChanged?.Invoke(Zoom);
            OnDrag?.Invoke(Offset);
            Invalidate();
        }

        public void ResetView()
        {
            Zoom = 3;
            Offset = new Point(270, 11);
            OnZoomChanged?.Invoke(Zoom);
            OnDrag?.Invoke(Offset);
            Invalidate();
        }

        public void CenterView()
        {
            var destSize = new Size(TftWidth * Zoom, TftHeight * Zoom);
            Offset = new Point((Width - destSize.Width) / 2, (Height - destSize.Height) / 2);
            Invalidate();
        }

        // Render all elements into the offscreen TFT bitmap.
        public void RenderObjects()
        {
            if (_offscreen == null) CreateOffscreen();
            var list = Elements.OrderBy(x => x.Zindex);
            using var g = Graphics.FromImage(_offscreen);
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half; // crisp 1px lines in bitmap space
            g.Clear(TftBackground);

            foreach (var el in list)
                el.Draw(g);
            if (_selecting)
            {
                _selectionElement.Draw(g);
            }
            SelectedElement?.DrawHandle(g);
            Invalidate(); // trigger OnPaint to show updated scaled image
        }

        // Helper for controllers
        public void AddElement(TftElement el)
        {
            Elements.Add(el);
        }

        public void ClearElements()
        {
            Elements.Clear();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.CompositingMode = CompositingMode.SourceOver;

            if (_offscreen == null)
                CreateOffscreen();

            var dest = new Rectangle(
                Offset.X,
                Offset.Y,
                TftWidth * Zoom,
                TftHeight * Zoom);

            // Draw scaled using nearest-neighbor so pixels remain crisp.
            g.DrawImage(_offscreen, dest, new Rectangle(0, 0, TftWidth, TftHeight), GraphicsUnit.Pixel);

            if (ShowTftBorder)
            {
                using var borderPen = new Pen(Color.LightGray, 1);
                g.DrawRectangle(borderPen, dest);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _offscreen?.Dispose();
            }
            base.Dispose(disposing);
        }

        void CreateOffscreen()
        {
            _offscreen?.Dispose();
            _offscreen = new Bitmap(TftWidth, TftHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        void OnMouseDown(object sender, MouseEventArgs e)
        {
            var canvasPos = ScreenToCanvasPosition(e.Location);
            if (e.Button == MouseButtons.Left)
            {
                if (SelectedElement == null)
                {
                    var element = Util.GetPointerOverlapElement(Elements, canvasPos);
                    if (element == null)
                    {
                        _selecting = true;
                        _selectionElement.Position = canvasPos;
                        _selectionElement.End = canvasPos;
                        SelectedElement = null;
                        _draggingElement = false;
                    }
                    else
                    {
                        _draggingElement = true;
                        SelectedElement = element;
                        SelectedElement.StartDrag();
                        SelectedElement.currentHandleIndex = SelectedElement.GetSelectedHandle(e.Location, Zoom, Offset);
                        System.Diagnostics.Debug.WriteLine(SelectedElement.currentHandleIndex);
                        _dragStartPos = canvasPos;
                        _selectedOffset = Util.GetPointOffset(element.Position, canvasPos);
                    }
                }
                else
                {
                    SelectedElement.currentHandleIndex = SelectedElement.GetSelectedHandle(e.Location, Zoom, Offset);
                    if (SelectedElement.currentHandleIndex == null && !SelectedElement.IsPointInside(canvasPos))
                    {
                        _selecting = true;
                        _selectionElement.Position = canvasPos;
                        _selectionElement.End = canvasPos;
                        SelectedElement.EndDrag();
                        SelectedElement = null;
                        _draggingElement = false;
                    }
                    else
                    {
                        _draggingElement = true;
                        SelectedElement.StartDrag();
                        SelectedElement.currentHandleIndex = SelectedElement.GetSelectedHandle(e.Location, Zoom, Offset);
                        _dragStartPos = canvasPos;
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                _panning = true;
                _panStartMouse = e.Location;
                _panStartOffset = Offset;
            }
        }

        public event Action<Point> OnPointerMove;
        public event Action<PointF> OnDrag;

        Point ScreenToCanvasPosition(Point screenPoint)
        {
            var p = new Point((screenPoint.X - Offset.X)/Zoom,(screenPoint.Y - Offset.Y)/Zoom);
            return p;
        }
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            var canvasPos = ScreenToCanvasPosition(e.Location);
            if (SelectedElement != null && _draggingElement)
            {
                SelectedElement.UpdateSelect(canvasPos, _dragStartPos);
                RenderObjects();
            }
            if (_panning)
            {
                var dx = e.X - _panStartMouse.X;
                var dy = e.Y - _panStartMouse.Y;
                Offset = new Point(_panStartOffset.X + dx, _panStartOffset.Y + dy);
                OnDrag?.Invoke(Offset);
                Invalidate();
            }
            if (_selecting)
            {
                _selectionElement.End = canvasPos;
                RenderObjects();
            }
            OnPointerMove?.Invoke(canvasPos);
        }

        void OnMouseWheel(object sender, MouseEventArgs e)
        {
            // Integer zoom steps. Ctrl speeds it up if you want.
            int step = (ModifierKeys & Keys.Control) == Keys.Control ? 2 : 1;
            int z = Zoom + Math.Sign(e.Delta) * step;
            SetZoom(z, e.Location);
        }
    }
}

/*
USAGE EXAMPLE (e.g., in your Form1.cs):

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

    // Later, your controller can mutate any element fields then call:
    // _canvas.RenderObjects();
}
*/
