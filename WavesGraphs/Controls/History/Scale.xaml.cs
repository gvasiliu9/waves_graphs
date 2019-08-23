using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SkiaSharp;
using WavesGraphs.Controls.Models.History.Scale;
using WavesGraphs.Controls.Models.Shared;
using WavesGraphs.Models.History;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class Scale : ContentView
    {
        #region Fileds

        CanvasInfo _canvasInfo;

        ScaleLinesDraw _scaleLinesDraw;

        ScaleColorsDraw _scaleColorsDraw;

        List<ScaleIntervalDraw> _scaleIntervalDraws;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty ScaleIntervalsProperty = BindableProperty
            .Create(nameof(ScaleIntervals), typeof(List<ScaleIntervalModel>), typeof(Scale), default(List<ScaleIntervalModel>));

        public List<ScaleIntervalModel> ScaleIntervals
        {
            get => (List<ScaleIntervalModel>)GetValue(ScaleIntervalsProperty);

            set => SetValue(ScaleIntervalsProperty, value);
        }

        #endregion

        public Scale()
        {
            InitializeComponent();

            canvas.PaintSurface += Canvas_PaintSurface;

            InitPaints();

            _scaleIntervalDraws = new List<ScaleIntervalDraw>();
        }

        #region Methods

        void InitPaints()
        {
            // Scale lines
            _scaleLinesDraw.DottedLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#D1D1D1"),
                IsAntialias = true
            };

            _scaleLinesDraw.SeparatorLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
                IsAntialias = true
            };

            // Scale colors
            _scaleColorsDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
            };
        }

        void DrawScale()
        {
            // Init
            Range range;

            float startPercentage;
            float endPercentage;

            var startPoint = new SKPoint();
            var endPoint = new SKPoint();

            var separatorLinesPath = new SKPath();
            var dottedLinesPath = new SKPath();

            // Clear current intervals
            _scaleIntervalDraws.Clear();

            // Get bounds height
            float height = _scaleColorsDraw.Bounds.Height;

            // Line X
            float lineMidX = (_scaleColorsDraw.Paint.StrokeWidth / 2);
            startPoint.X = _scaleColorsDraw.Bounds.Left + lineMidX;
            endPoint.X = _scaleColorsDraw.Bounds.Left + lineMidX;

            // Range
            range.From = ScaleIntervals.First().From;
            range.To = ScaleIntervals.Last().To;

            // Draw intervals
            foreach (var scaleInterval in ScaleIntervals)
            {
                // Set color
                _scaleColorsDraw.Paint.Color = SKColor.Parse(scaleInterval.Color);

                // Calculate percentage
                startPercentage = GetPercentage(scaleInterval.From, range);
                endPercentage = GetPercentage(scaleInterval.To, range);

                // Draw color
                startPoint.Y = _scaleColorsDraw.Bounds.Bottom - (height * startPercentage);
                endPoint.Y = _scaleColorsDraw.Bounds.Bottom - (height * endPercentage);

                _canvasInfo.Canvas.DrawLine(startPoint, endPoint, _scaleColorsDraw.Paint);

                // Color separators
                separatorLinesPath.MoveTo(_scaleColorsDraw.Bounds.Left, endPoint.Y);
                separatorLinesPath.LineTo(_scaleColorsDraw.Bounds.Right, endPoint.Y);

                // Dotted separator lines
                dottedLinesPath.MoveTo(_scaleLinesDraw.Bounds.Left, endPoint.Y);
                dottedLinesPath.LineTo(_scaleLinesDraw.Bounds.Right, endPoint.Y);
            }

            // Draw dotted lines
            _canvasInfo.Canvas.DrawPath(separatorLinesPath, _scaleLinesDraw.SeparatorLinePaint);
            _canvasInfo.Canvas.DrawPath(dottedLinesPath, _scaleLinesDraw.DottedLinePaint);

            separatorLinesPath.Dispose();
            dottedLinesPath.Dispose();
        }

        float GetPercentage(float input, Range range)
        {
            float x = range.To - range.From;

            float result = (100 * (input - range.From)) / x;

            return result / 100;
        }

        void Calculate(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Scale colors
            _scaleColorsDraw.Bounds.Top = e.Info.Height * 0.30f;
            _scaleColorsDraw.Bounds.Left = e.Info.Width * 0.99f;
            _scaleColorsDraw.Bounds.Right = e.Info.Width;
            _scaleColorsDraw.Bounds.Bottom = e.Info.Height;
            _scaleColorsDraw.Paint.StrokeWidth = _scaleColorsDraw.Bounds.Width;

            // Scale lines
            _scaleLinesDraw.Bounds.Top = _scaleColorsDraw.Bounds.Top;
            _scaleLinesDraw.Bounds.Left = 0;
            _scaleLinesDraw.Bounds.Right = e.Info.Width * 0.98f;
            _scaleLinesDraw.Bounds.Bottom = e.Info.Height;

            _scaleLinesDraw.DottedLinePaint.PathEffect = SKPathEffect.CreateDash(new float[] { 3f, 20f }, 20);
            _scaleLinesDraw.DottedLinePaint.StrokeWidth = 3;

            _scaleLinesDraw.SeparatorLinePaint.StrokeWidth = 5;
        }

        void DrawBounds(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Red,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 3
            };

            // Scale color
            e.Surface.Canvas.DrawRect(_scaleColorsDraw.Bounds, paint);

            // Scale lines
            paint.Color = SKColors.Black;
            e.Surface.Canvas.DrawRect(_scaleLinesDraw.Bounds, paint);
        }

        #endregion

        #region Events

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Scale intervals
            if (propertyName == ScaleIntervalsProperty.PropertyName)
                canvas.InvalidateSurface();
        }

        void Canvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get canvas info
            _canvasInfo.Canvas = e.Surface.Canvas;
            _canvasInfo.Surface = e.Surface;
            _canvasInfo.ImageInfo = e.Info;

            e.Surface.Canvas.Clear(SKColors.Transparent);

            Calculate(e);

            // Draw
            DrawScale();

            //DrawBounds(e);
        }

        #endregion
    }
}
