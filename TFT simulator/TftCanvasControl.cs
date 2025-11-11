// TftCanvasControl.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection.Metadata;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TFT_simulator
{
    // ---- Canvas control ----
    public class TftCanvasControl : Control
    {
        public static Action ElementPropertyChanged;
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

        public Point mouseCanvasPos { get; private set; }

        Bitmap _offscreen;
        bool _panning;
        Point _panStartMouse;
        Point _panStartOffset;
        bool _selecting;
        bool _draggingElement;
        SelectionElement _selectionElement;
        Point _selectedOffset;
        Point _dragStartPos;
        List<TftElement> _selectedElements = new List<TftElement>();
        Region selectRegions;
        RectangleF selectBound;

        public event Action refreshListbox;
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
                if (_selecting)CheckSelection();
                _selecting = false;
                _draggingElement = false;
                refreshListbox?.Invoke();
                RenderObjects();
            };
            MouseWheel += OnMouseWheel;
        }
        public void CheckSelection()
        {
            var list = new List<TftElement>();
            var selectRect = _selectionElement.GetRegion();
            foreach(var el in Elements)
            {
                var rg = el.GetRect();
                if (rg.IntersectsWith(selectRect))
                {
                    list.Add(el);
                }
            }
            _selectedElements = list;
        }
        public void RemoveElement(TftElement element)
        {
            Elements.Remove(element);
            var index = _selectedElements.IndexOf(element);
            if (index != -1)
            {
                _selectedElements[index].EndDrag();
                _selectedElements.RemoveAt(index);
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
            OnPan?.Invoke(Offset);
            Invalidate();
        }

        public void ResetView()
        {
            Zoom = 3;
            Offset = new Point(270, 11);
            OnZoomChanged?.Invoke(Zoom);
            OnPan?.Invoke(Offset);
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
            if (_selectedElements.Count != 0)
            {
                Region selectRegion = new Region(_selectedElements[0].GetRect());
                foreach (var ele in _selectedElements)
                {
                    var rect = ele.GetRect();
                    var rg = new Region(rect);
                    selectRegion.Union(rg);
                }
                selectRegions = selectRegion;
                selectBound = selectRegions.GetBounds(g);
                using var brush = new SolidBrush(Color.FromArgb(40, 30, 150, 255));
                using var pen = new Pen(Color.LightBlue);
                pen.DashStyle = DashStyle.Dot;
                g.FillRectangle(brush, (RectangleF)selectBound);
                selectBound.X -= 1; selectBound.Y -= 1; selectBound.Width += 1; selectBound.Height += 1;
                g.DrawRectangle(pen, (RectangleF)selectBound);
            }
            if (_draggingElement)
            {
                using var pen = new Pen(Color.FromArgb(150,0,0,0));
                Point pt = new Point(mouseCanvasPos.X, 0)
                    , pd = new Point(mouseCanvasPos.X, TftHeight)
                    , pl = new Point(0, mouseCanvasPos.Y)
                    , pr = new Point(TftWidth, mouseCanvasPos.Y);
                g.DrawLine(pen,pt,pd);
                g.DrawLine(pen, pl, pr);
            }
            if (_selectedElements.Count == 1)
            {
                _selectedElements[0]?.DrawHandle(g);
            }
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
        public event Action<TftElement> SelectElement;
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            bool shift = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            mouseCanvasPos = ScreenToCanvasPosition(e.Location);

            if (e.Button == MouseButtons.Right)
            {
                StartPanning(e.Location);
                return;
            }
            if (e.Button != MouseButtons.Left) return;

            var hit = Util.GetPointerOverlapElement(Elements, mouseCanvasPos);

            if (_selectedElements.Count > 0)
            {
                bool overSelected = IsMouseOverSelectionOrHandle(e, hit);

                if (!overSelected)
                {
                    if (hit != null)
                    {
                        SelectElement?.Invoke(hit);
                        if (!_selectedElements.Contains(hit) && !shift)
                        {
                            _selectedElements.Clear();
                        }
                        if (_selectedElements.Contains(hit))
                            _selectedElements.Remove(hit);
                        else
                            _selectedElements.Add(hit);
                        ClearHandles(_selectedElements);
                        StartDragSelected(mouseCanvasPos);
                        return;
                    }

                    BeginSelection(mouseCanvasPos);
                    return;
                }

                StartDragSelected(mouseCanvasPos);
                return;
            }

            if (hit == null)
            {
                BeginSelection(mouseCanvasPos);
                return;
            }

            SelectElement?.Invoke(hit);
            _selectedElements.Clear();
            _selectedElements.Add(hit);
            hit.currentHandleIndex = null;
            StartDragSelected(mouseCanvasPos);
        }

        private void StartPanning(Point mouse)
        {
            _panning = true;
            _panStartMouse = mouse;
            _panStartOffset = Offset;
        }

        private void BeginSelection(Point startPos)
        {
            _selecting = true;
            _selectionElement.Position = startPos;
            _selectionElement.End = startPos;
            _draggingElement = false;

            foreach (var el in _selectedElements) el.EndDrag();
            _selectedElements.Clear();

            OnDrag?.Invoke(false);
        }

        private void StartDragSelected(Point startPos)
        {
            OnDrag?.Invoke(true);
            _draggingElement = true;

            foreach (var el in _selectedElements) el.StartDrag();

            _dragStartPos = startPos;
        }

        private void ClearHandles(List<TftElement> elements)
        {
            foreach (var el in elements) el.currentHandleIndex = null;
        }

        private bool IsMouseOverSelectionOrHandle(MouseEventArgs e, TftElement? hit)
        {
            if (hit == null) return false;
            // Single selection: respect handles and precise hit
            if (_selectedElements.Count == 1 && hit != null && _selectedElements[0] == hit)
            {
                var sel = _selectedElements[0];
                sel.currentHandleIndex = sel.GetSelectedHandle(e.Location, Zoom, Offset);
                return sel.currentHandleIndex != null || sel.IsPointInside(mouseCanvasPos);
            }

            // Multi-select: fallback to selection bounds containment
            ClearHandles(_selectedElements);
            return Util.IsPointInsideRect(mouseCanvasPos, selectBound);
        }
        public event Action<Point> OnPointerMove;
        public event Action<PointF> OnPan;
        public event Action<bool> OnDrag;
        public event Action<Point> Dragging;

        Point ScreenToCanvasPosition(Point screenPoint)
        {
            var p = new Point((screenPoint.X - Offset.X)/Zoom,(screenPoint.Y - Offset.Y)/Zoom);
            return p;
        }
        public event Action UpdateProperty;
        void OnMouseMove(object sender, MouseEventArgs e)
        {
            mouseCanvasPos  = ScreenToCanvasPosition(e.Location);
            if (_selectedElements.Count != 0 && _draggingElement)
            {
                foreach(var element in _selectedElements)
                {
                    element.UpdateSelect(mouseCanvasPos, _dragStartPos, false);
                }
                //UpdateProperty?.Invoke();
                //TftCanvasControl.ElementPropertyChanged?.Invoke();
                Dragging?.Invoke(Util.SubtractPoint(mouseCanvasPos, _dragStartPos));
                RenderObjects();
            }
            if (_panning)
            {
                var dx = e.X - _panStartMouse.X;
                var dy = e.Y - _panStartMouse.Y;
                Offset = new Point(_panStartOffset.X + dx, _panStartOffset.Y + dy);
                OnPan?.Invoke(Offset);
                Invalidate();
            }
            if (_selecting)
            {
                _selectionElement.End = mouseCanvasPos;
                RenderObjects();
            }
            OnPointerMove?.Invoke(mouseCanvasPos);
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