using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using SkiaSharp;
using WavesGraphs.Helpers;
using WavesGraphs.Helpers.Constants;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class DashboardGraph : ContentView
    {
        #region Fields

        // Bindable properties
        private float _maxValue;

        private string[] _timeAsString = {
        "2019-08-15T13:00:00Z",
        "2019-08-15T13:15:00Z",
        "2019-08-15T13:30:00Z",
        "2019-08-15T13:45:00Z",
        "2019-08-15T14:00:00Z",
        "2019-08-15T14:15:00Z",
        "2019-08-15T14:30:00Z",
        "2019-08-15T14:45:00Z",
        "2019-08-15T15:00:00Z",
        "2019-08-15T15:15:00Z",
        "2019-08-15T15:30:00Z",
        "2019-08-15T15:45:00Z",
        "2019-08-15T16:00:00Z",
        "2019-08-15T16:15:00Z",
        "2019-08-15T16:30:00Z",
        "2019-08-15T16:45:00Z",
        "2019-08-15T17:00:00Z",
        "2019-08-15T17:15:00Z",
        "2019-08-15T17:30:00Z",
        "2019-08-15T17:45:00Z",
        "2019-08-15T18:00:00Z",
        "2019-08-15T18:15:00Z",
        "2019-08-15T18:30:00Z",
        "2019-08-15T18:45:00Z",
        "2019-08-15T19:00:00Z",
        "2019-08-15T19:15:00Z",
        "2019-08-15T19:30:00Z",
        "2019-08-15T19:45:00Z",
        "2019-08-15T20:00:00Z",
        "2019-08-15T20:15:00Z",
        "2019-08-15T20:30:00Z",
        "2019-08-15T20:45:00Z",
        "2019-08-15T21:00:00Z",
        "2019-08-15T21:15:00Z",
        "2019-08-15T21:30:00Z",
        "2019-08-15T21:45:00Z",
        "2019-08-15T22:00:00Z",
        "2019-08-15T22:15:00Z",
        "2019-08-15T22:30:00Z",
        "2019-08-15T22:45:00Z",
        "2019-08-15T23:00:00Z",
        "2019-08-15T23:15:00Z",
        "2019-08-15T23:30:00Z",
        "2019-08-15T23:45:00Z",
        "2019-08-16T00:00:00Z",
        "2019-08-16T00:15:00Z",
        "2019-08-16T00:30:00Z",
        "2019-08-16T00:45:00Z",
        "2019-08-16T01:00:00Z",
        "2019-08-16T01:15:00Z",
        "2019-08-16T01:30:00Z",
        "2019-08-16T01:45:00Z",
        "2019-08-16T02:00:00Z",
        "2019-08-16T02:15:00Z",
        "2019-08-16T02:30:00Z",
        "2019-08-16T02:45:00Z",
        "2019-08-16T03:00:00Z",
        "2019-08-16T03:15:00Z",
        "2019-08-16T03:30:00Z",
        "2019-08-16T03:45:00Z",
        "2019-08-16T04:00:00Z",
        "2019-08-16T04:15:00Z",
        "2019-08-16T04:30:00Z",
        "2019-08-16T04:45:00Z",
        "2019-08-16T05:00:00Z",
        "2019-08-16T05:15:00Z",
        "2019-08-16T05:30:00Z",
        "2019-08-16T05:45:00Z",
        "2019-08-16T06:00:00Z",
        "2019-08-16T06:15:00Z",
        "2019-08-16T06:30:00Z",
        "2019-08-16T06:45:00Z",
        "2019-08-16T07:00:00Z",
        "2019-08-16T07:15:00Z",
        "2019-08-16T07:30:00Z",
        "2019-08-16T07:45:00Z",
        "2019-08-16T08:00:00Z",
        "2019-08-16T08:15:00Z",
        "2019-08-16T08:30:00Z",
        "2019-08-16T08:45:00Z",
        "2019-08-16T09:00:00Z",
        "2019-08-16T09:15:00Z",
        "2019-08-16T09:30:00Z",
        "2019-08-16T09:45:00Z",
        "2019-08-16T10:00:00Z",
        "2019-08-16T10:15:00Z",
        "2019-08-16T10:30:00Z",
        "2019-08-16T10:45:00Z",
        "2019-08-16T11:00:00Z",
        "2019-08-16T11:15:00Z",
        "2019-08-16T11:30:00Z",
        "2019-08-16T11:45:00Z",
        "2019-08-16T12:00:00Z",
        "2019-08-16T12:15:00Z",
        "2019-08-16T12:30:00Z",
        "2019-08-16T12:45:00Z"
        };

        private int?[] _airflow =
            {
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                126,
                126,
                126,
                126,
                100,
                100,
                100,
                100,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                84,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                100,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                150
            };
    
        private List<GraphValueModel> _graphValues;

        // Text
        private TextConstants _textConstants;

        // Canvas info
        private CanvasInfo _maxValueCanvasInfo;
        private CanvasInfo _hoursCanvasInfo;
        private CanvasInfo _graphCanvasInfo;

        // Draw info
        private MaxValueDraw _maxValueDraw;
        private GraphDraw _graphDraw;
        private HoursDraw _hoursDraw;
        private CurrentValueIndicatorDraw _currentValueIndicatorDraw;

        private TextDraw _valueTextDraw;
        private TextDraw _titleTextDraw;
        private TextDraw _iconTextDraw;

        #endregion

        public DashboardGraph()
        {
            InitializeComponent();

            hoursCanvas.PaintSurface += HoursCanvas_PaintSurface;

            _graphValues = new List<GraphValueModel>();

            InitPaints();

            InitGraphValues();

            _textConstants.Hours = "24H";
            _textConstants.Title = "Ventilation";
            _textConstants.Icon = IconsHelper.Ventilator;
        }

        #region Methods

        private void InitGraphValues()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            for (int i = 0; i < _timeAsString.Length; i++)
            {
                _graphValues.Add(new GraphValueModel
                {
                    DateTime = DateTime.ParseExact(_timeAsString[i], dateFormat, CultureInfo.InvariantCulture),
                    Airflow = _airflow[i] ?? 0
                });
            }

            _maxValue = _graphValues.Select(v => v.Airflow).Max();
        }

        /// <summary>
        /// Init all canvases
        /// </summary>
        private void InitCanvases()
        {
            maxValueCanvas.PaintSurface += MaxValueCanvas_PaintSurface;
            graphCanvas.PaintSurface += GraphCanvas_PaintSurface;

            maxValueCanvas.InvalidateSurface();
            graphCanvas.InvalidateSurface();
        }

        /// <summary>
        /// Init draw paints
        /// </summary>
        private void InitPaints()
        {
            // Text
            _valueTextDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White
            };

            _titleTextDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White,
                FakeBoldText = true,
                Typeface = SKTypeface.FromFamilyName("Arial")
            };

            _iconTextDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White,
                Typeface = SkiaSharpHelper.LoadTtfFont(Fonts.IconsFontName)
            };

            _maxValueDraw.LinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = SKColors.White
            };

            // Graph
            _graphDraw.FirstGradientColor = SKColor.Parse("#A1C6E6");
            _graphDraw.SecondGradientColor = SKColor.Parse("#3A77B8");

            _graphDraw.Paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = _graphDraw.FirstGradientColor,
            };

            _currentValueIndicatorDraw.Paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.White,
            };

            // Hours
            _hoursDraw.Paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            _hoursDraw.MidnightIndicatorPaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
        }

        /// <summary>
        /// Draw hours line, each circle means one hour
        /// </summary>
        private void DrawHoursLine()
        {
            var point = new SKPoint();
            point.X = _hoursDraw.Step;
            point.Y = _hoursDraw.Bounds.MidY;

            // Draw hours circles
            foreach(var datetime in _graphValues.Select(v => v.DateTime))
            {
                if (datetime.Minute != 0)
                    continue;
                    
                if (datetime.TimeOfDay.Ticks == 0)
                    DrawMidnightIndicator(point);
                else
                    DrawHourCircle(point);

                point.X += _hoursDraw.Step;
            }

            // Draw text
            _hoursCanvasInfo.Canvas.DrawText(_textConstants.Hours,
                new SKPoint(_hoursDraw.Bounds.Right + _valueTextDraw.Margin.Left,
                    _hoursDraw.Bounds.MidY + ((_valueTextDraw.Paint.TextSize / 2f) - _hoursDraw.CircleRadius)),
                _valueTextDraw.Paint);
        }

        /// <summary>
        /// Draw hour circle
        /// </summary>
        /// <param name="center"></param>
        private void DrawHourCircle(SKPoint center)
            => _hoursCanvasInfo.Canvas.DrawCircle(center, _hoursDraw.CircleRadius, _hoursDraw.Paint);

        /// <summary>
        /// Draw graph line
        /// </summary>
        private void DrawGraph()
        {
            var path = new SKPath();
            var point = new SKPoint();

            point.X = _graphDraw.Bounds.Left;
            point.Y = _graphDraw.Bounds.Bottom;

            path.MoveTo(point);

            float percentage;

            // Get values
            foreach(var graphValue in _graphValues)
            {
                point.X += (_graphDraw.Step / _graphDraw.Bounds.Right) * _graphDraw.Bounds.Right;

                percentage = ((_maxValue - graphValue.Airflow) / _maxValue);

                if (percentage < 0.80f)
                    point.Y = (percentage * _graphDraw.Bounds.Bottom) +_graphDraw.Bounds.Top;
                else
                    point.Y = _graphDraw.Bounds.Bottom;

                path.LineTo(point);
            }

            // Close graph
            path.LineTo(point.X, _graphDraw.Bounds.Bottom);

            // Add gradient
            _graphDraw.Paint.Shader = SKShader.CreateLinearGradient(
                                new SKPoint(_graphDraw.Bounds.Right, _graphDraw.Bounds.Top),
                                new SKPoint(_graphDraw.Bounds.Right, _graphDraw.Bounds.Bottom),
                                new SKColor[] { _graphDraw.FirstGradientColor, _graphDraw.SecondGradientColor },
                                new float[] { 0f, 0.96f },
                                SKShaderTileMode.Clamp);

            // Draw graph
            _graphCanvasInfo.Canvas.DrawPath(path, _graphDraw.Paint);

            path.Dispose();

            // Draw current value indicator
            DrawCurrentValueIndicator(point);

            // Draw current value percentage
            DrawCurrentValuePercentage(point, _graphValues.Last().Airflow);
        }

        /// <summary>
        /// Draw current value percentage circle indicator and line
        /// </summary>
        /// <param name="point"></param>
        private void DrawCurrentValueIndicator(SKPoint point)
        {
            // Circle
            _graphCanvasInfo.Canvas.DrawCircle(point,
                _currentValueIndicatorDraw.Radius,
                _currentValueIndicatorDraw.Paint);

            // Down line
            if(_graphDraw.Bounds.Bottom - point.Y > _currentValueIndicatorDraw.Radius)
                _graphCanvasInfo.Canvas.DrawLine(new SKPoint(point.X, point.Y + _currentValueIndicatorDraw.Radius),
                    new SKPoint(point.X, _graphDraw.Bounds.Bottom), _currentValueIndicatorDraw.Paint);
        }

        private void DrawCurrentValuePercentage(SKPoint point, float percentage)
        {
            _graphCanvasInfo.Canvas.DrawText($"{percentage}%",
                new SKPoint(point.X + _valueTextDraw.Margin.Left, point.Y),
                _valueTextDraw.Paint);
        }

        /// <summary>
        /// Draw midnight line indicator
        /// </summary>
        /// <param name="point"></param>
        private void DrawMidnightIndicator(SKPoint point)
        {
            _hoursCanvasInfo.Canvas.DrawLine(new SKPoint(point.X, _hoursDraw.Bounds.Top),
                new SKPoint(point.X, _hoursDraw.Bounds.Bottom), _hoursDraw.MidnightIndicatorPaint);
        }

        private void DrawTitle()
        {
            float offsetX = _maxValueDraw.Bounds.Left;

            // Icon
            _maxValueCanvasInfo.Canvas.DrawText(_textConstants.Icon,
                offsetX,
                _maxValueDraw.Bounds.MidY,
                _iconTextDraw.Paint);

            // Title
            offsetX += _iconTextDraw.Bounds.Width + _titleTextDraw.Margin.Left;

            _maxValueCanvasInfo.Canvas.DrawText(_textConstants.Title,
                offsetX,
                _maxValueDraw.Bounds.MidY,
                _titleTextDraw.Paint);

            // Line
            offsetX += _titleTextDraw.Bounds.Width + _titleTextDraw.Margin.Right;

            _maxValueCanvasInfo.Canvas.DrawLine(new SKPoint(offsetX, _maxValueDraw.Bounds.MidY + _titleTextDraw.Bounds.MidY),
                new SKPoint(_maxValueDraw.Bounds.Right, _maxValueDraw.Bounds.MidY + _titleTextDraw.Bounds.MidY),
                _maxValueDraw.LinePaint);
        }

        private void DrawMaxValue()
        {

        }

        /// <summary>
        /// Make control responsive for different resolutions
        /// </summary>
        /// <param name="eventArgs"></param>
        private void Calculate(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs eventArgs)
        {
            // Constants
            var _3px = 0;
            var _5px = 0;
            var _15px = 0;
            var _25px = 0;
            var _30px = 0;
            var _100px = 0;

            // Common bounds
            var rightBound = eventArgs.Info.Width * 0.85f;
            var maxValueLinAndGraphLineIntersectionBound = eventArgs.Info.Height * 0.15f;
            var graphLineAndHoursLineIntersectionBound = eventArgs.Info.Height * 0.95f;

            // Max value
            _maxValueDraw.Bounds.Top = 0;
            _maxValueDraw.Bounds.Left = 15;
            _maxValueDraw.Bounds.Bottom = maxValueLinAndGraphLineIntersectionBound;
            _maxValueDraw.Bounds.Right = rightBound;

            // Graph
            _graphDraw.Step = rightBound / _graphValues.Count();

            _graphDraw.Bounds.Top = maxValueLinAndGraphLineIntersectionBound;
            _graphDraw.Bounds.Left = 0;
            _graphDraw.Bounds.Bottom = graphLineAndHoursLineIntersectionBound - 15;
            _graphDraw.Bounds.Right = rightBound;

            _graphDraw.Paint.PathEffect = SKPathEffect.CreateCorner(50);

            _currentValueIndicatorDraw.Paint.StrokeWidth = 3;
            _currentValueIndicatorDraw.Radius = 10;

            // Hours
            _hoursDraw.Bounds.Top = graphLineAndHoursLineIntersectionBound;
            _hoursDraw.Bounds.Left = 0;
            _hoursDraw.Bounds.Bottom = eventArgs.Info.Height;
            _hoursDraw.Bounds.Right = rightBound;

            _hoursDraw.Step = _hoursDraw.Bounds.Width / 24;
            _hoursDraw.CircleRadius = 3;

            _hoursDraw.MidnightIndicatorPaint.StrokeWidth = 3;

            // Max value
            _maxValueDraw.LinePaint.StrokeWidth = 3;

            // Text
            _valueTextDraw.Paint.TextSize = 25;
            _valueTextDraw.Margin.Left = 30;

            _titleTextDraw.Paint.TextSize = 35;
            _titleTextDraw.Margin.Left = 15;
            _titleTextDraw.Margin.Right = 25;
            _titleTextDraw.Paint.MeasureText(_textConstants.Title, ref _titleTextDraw.Bounds);

            _iconTextDraw.Paint.TextSize = 35;
            _iconTextDraw.Paint.MeasureText(_textConstants.Icon, ref _iconTextDraw.Bounds);

        }

        /// <summary>
        /// Add outline helper for all canvas objects
        /// </summary>
        /// <param name="eventArgs"></param>
        private void DrawBounds(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs eventArgs)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Blue,
                Style = SKPaintStyle.Stroke
            };

            // Max value
            eventArgs.Surface.Canvas.DrawRect(_maxValueDraw.Bounds, paint);

            paint.Color = SKColors.Yellow;

            // Graph
            eventArgs.Surface.Canvas.DrawRect(_graphDraw.Bounds, paint);

            paint.Color = SKColors.Red;

            // Hours
            eventArgs.Surface.Canvas.DrawRect(_hoursDraw.Bounds, paint);
        }

        /// <summary>
        /// Clear canvas with transparent color
        /// </summary>
        /// <param name="e"></param>
        private void ClearCanvas(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
            => e.Surface.Canvas.Clear(SKColors.Transparent);

        #endregion

        #region Events

        #region Canvas

        private void MaxValueCanvas_PaintSurface
            (object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get cavans info
            _maxValueCanvasInfo.Canvas = e.Surface.Canvas;
            _maxValueCanvasInfo.ImageInfo = e.Info;
            _maxValueCanvasInfo.Surface = e.Surface;

            // Draw
            ClearCanvas(e);

            DrawTitle();

            DrawMaxValue();
        }

        private void GraphCanvas_PaintSurface
            (object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get cavans info
            _graphCanvasInfo.Canvas = e.Surface.Canvas;
            _graphCanvasInfo.ImageInfo = e.Info;
            _graphCanvasInfo.Surface = e.Surface;

            // Draw
            ClearCanvas(e);

            DrawGraph();
        }

        /// <summary>
        /// Hours canvas is drawn first, then all remaining canvases are drawn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HoursCanvas_PaintSurface
            (object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get cavans info
            _hoursCanvasInfo.Canvas = e.Surface.Canvas;
            _hoursCanvasInfo.ImageInfo = e.Info;
            _hoursCanvasInfo.Surface = e.Surface;

            Calculate(e);

            // Draw
            ClearCanvas(e);

            //DrawBounds(e);

            DrawHoursLine();

            InitCanvases();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Hours line draw helper
    /// </summary>
    public struct HoursDraw
    {
        public float Step;

        public float CircleRadius;

        public SKRect Bounds;

        public SKPaint Paint;

        public SKPaint MidnightIndicatorPaint;
    }

    /// <summary>
    /// Graph line draw helper
    /// </summary>
    public struct GraphDraw
    {
        public float Step;

        public SKRect Bounds;

        public SKPaint Paint;

        public float CornerRadius;

        public SKColor FirstGradientColor;

        public SKColor SecondGradientColor;
    }

    /// <summary>
    /// Current value indicator draw helper
    /// </summary>
    public struct CurrentValueIndicatorDraw
    {
        public SKPaint Paint;

        public float Radius;
    }

    /// <summary>
    /// Max value line draw helper
    /// </summary>
    public struct MaxValueDraw
    {
        public SKRect Bounds;

        public SKPaint LinePaint;
    }

    /// <summary>
    /// Layout text draw helper
    /// </summary>
    public struct TextDraw
    {
        public SKPaint Paint;

        public Margin Margin;

        public SKRect Bounds;
    }

    /// <summary>
    /// Element margins
    /// </summary>
    public struct Margin
    {
        public float Left;
        public float Top;
        public float Right;
        public float Bottom;
    }

    /// <summary>
    /// Info helper for canvas
    /// </summary>
    public struct CanvasInfo
    {
        public SKCanvas Canvas;

        public SKSurface Surface;

        public SKImageInfo ImageInfo;
    }

    /// <summary>
    /// Graph value used to draw graph line
    /// </summary>
    public class GraphValueModel
    {
        public DateTime DateTime;

        public int Airflow;
    }

    /// <summary>
    /// Store text constant to display in control
    /// </summary>
    public struct TextConstants
    {
        public string Hours;

        public string Title;

        public string Icon;
    }
}
