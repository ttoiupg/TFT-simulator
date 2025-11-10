// TftCanvasControl.cs
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace TFT_simulator
{

    // ---- Elements ----
    public abstract class TftElement
    {
        // Public so a separate controller can mutate them at runtime.
        [Category("Data")]public string Name { get; set; }
        [Category("Transform")] public Point Position { get; set; }
        [Category("Transform")] public Size Size { get; set; }
        [Category("Appearance")] public Color Color { get; set; } = Color.Black;
        [Category("Appearance")] public int Thickness { get; set; } = 1;
        [Browsable(false)] public string Text { get; set; } = "";
        [Browsable(false)] public int FontSize { get; set; } = 1;
        [Browsable(false)] public int Radius { get; set; }
        [Category("Appearance")] public bool IsFilled { get; set; }
        public override string ToString() => $"{GetType().Name}";
        public abstract string Serialize(string prefix);
        public abstract void Draw(Graphics g);
    }

    public sealed class RectElement : TftElement
    {
        public override void Draw(Graphics g)
        {
            using var pen = new Pen(Color, Thickness);
            using var brush = new SolidBrush(Color);
            var r = new Rectangle(Position, Size);
            if (IsFilled) g.FillRectangle(brush, r);
            else g.DrawRectangle(pen, r);
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
    }

    public sealed class CircleElement : TftElement
    {
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
    }

    public sealed class LineElement : TftElement
    {
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
    }

    public sealed class TextElement : TftElement
    {
        [Category("Text")] [Browsable(true)] public new string Text { get; set; } = "";
        [Category("Text")] [Browsable(true)] public new int FontSize { get; set; } = 1;
        [Browsable(false)] public new Size Size { get; set; } // not used here

        private Size _size = new Size(6,8);
        // Intentionally blank render per your note; leave the plumbing so controller can fill later
        public override void Draw(Graphics g)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                var pos = new Point(Position.X + i * FontSize * _size.Width, Position.Y);
                var size = _size * FontSize;
                using var pen = new Pen(Color, 1);
                using var brush = new SolidBrush(Color);
                var r = new Rectangle(pos, size);
                if (IsFilled) g.FillRectangle(brush, r);
                else g.DrawRectangle(pen, r);
            }
        }
        public override string Serialize(string prefix)
        {
            var sb = new StringBuilder();
            sb.AppendLine(prefix + $".setTextColor({Util.ToRgb565String(Color)});");
            sb.AppendLine(prefix + $".setCursor({Position.X},{Position.Y});");
            sb.AppendLine(prefix + $".println(\"{Text}\");");
            return sb.ToString();
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
        public PointF Offset { get; private set; } = new PointF(20, 20);

        // Elements to render.
        public readonly List<TftElement> Elements = new List<TftElement>();

        // Background color of TFT buffer.
        public Color TftBackground = Color.Black;

        // Optional border for the TFT area when drawn to the control.
        public bool ShowTftBorder = true;

        Bitmap _offscreen;
        bool _panning;
        Point _panStartMouse;
        PointF _panStartOffset;

        public TftCanvasControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = Color.Black; // control background

            CreateOffscreen();
            // Mouse interactions
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += (_, __) => _panning = false;
            MouseWheel += OnMouseWheel;
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
        public void SetZoom(int zoom, PointF? anchorClientPoint = null)
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
                Offset = new PointF(p.X - tftX * newZoom, p.Y - tftY * newZoom);
            }

            Zoom = newZoom;
            OnZoomChanged?.Invoke(Zoom);
            OnMove?.Invoke(Offset);
            Invalidate();
        }

        public void ResetView()
        {
            Zoom = 3;
            Offset = new PointF(270, 11);
            OnZoomChanged?.Invoke(Zoom);
            OnMove?.Invoke(Offset);
            Invalidate();
        }

        public void CenterView()
        {
            var destSize = new Size(TftWidth * Zoom, TftHeight * Zoom);
            Offset = new PointF((Width - destSize.Width) / 2f, (Height - destSize.Height) / 2f);
            Invalidate();
        }

        // Render all elements into the offscreen TFT bitmap.
        public void RenderObjects()
        {
            if (_offscreen == null) CreateOffscreen();

            using var g = Graphics.FromImage(_offscreen);
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half; // crisp 1px lines in bitmap space
            g.Clear(TftBackground);

            foreach (var el in Elements)
                el.Draw(g);

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
                (int)Math.Round(Offset.X),
                (int)Math.Round(Offset.Y),
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

        // ---- Mouse interaction ----

        void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _panning = true;
                _panStartMouse = e.Location;
                _panStartOffset = Offset;
            }
        }

        public event Action<PointF> OnMove;

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_panning)
            {
                var dx = e.X - _panStartMouse.X;
                var dy = e.Y - _panStartMouse.Y;
                Offset = new PointF(_panStartOffset.X + dx, _panStartOffset.Y + dy);
                OnMove?.Invoke(Offset);
                Invalidate();
            }
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
