using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using SkiaSharp;
using TouchTracking;
using WavesGraphs.Helpers;
using WavesGraphs.Helpers.Constants;
using WavesGraphs.Models;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class DashboardGraph : ContentView
    {
        #region Fields

        // Canvas info
        private Size _size;

        private CanvasInfo _maxValueCanvasInfo;
        private CanvasInfo _hoursCanvasInfo;
        private CanvasInfo _graphCanvasInfo;
        private CanvasInfo _sensorLabelsCanvasInfo;

        // Draw info
        private TitleDraw _titleDraw;
        private MaxValueDraw _maxValueDraw;
        private GraphDraw _graphDraw;
        private HoursDraw _hoursDraw;
        private CurrentValueIndicatorDraw _currentValueIndicatorDraw;

        private DateTime _previousEventDateTime;

        // Sensors
        private List<SensorLabelDraw> _sensorLabelDraws;

        // Text
        private TextConstants _textConstants;
        private TextDraw _maxValueTextDraw;
        private TextDraw _valueTextDraw;
        private TextDraw _titleTextDraw;
        private TextDraw _iconTextDraw;

        // Touch tracking
        private long _touchId;
        private SKPoint _touchPoint;

        #endregion

        #region BindableProperties

        public static readonly BindableProperty ValuesProperty = BindableProperty
            .Create(nameof(Values), typeof(GraphValues), typeof(DashboardGraph), default(GraphValues));

        public GraphValues Values
        {
            get => (GraphValues)GetValue(ValuesProperty);

            set => SetValue(ValuesProperty, value);
        }

        #endregion

        public DashboardGraph()
        {
            InitializeComponent();

            hoursCanvas.PaintSurface += HoursCanvas_PaintSurface;

            InitPaints();

            _textConstants.Hours = "24H";
            _textConstants.Title = "Ventilation";
            _textConstants.Icon = IconsHelper.Ventilator;

            _sensorLabelDraws = new List<SensorLabelDraw>();
        }

        #region Methods

        /// <summary>
        /// Init all canvases
        /// </summary>
        private void InitCanvases()
        {
            maxValueCanvas.PaintSurface += MaxValueCanvas_PaintSurface;
            graphCanvas.PaintSurface += GraphCanvas_PaintSurface;
            sensorLabelsCanvas.PaintSurface += SensorLabelsCanvas_PaintSurface;

            maxValueCanvas.InvalidateSurface();
            graphCanvas.InvalidateSurface();
        }

        /// <summary>
        /// Init draw paints
        /// </summary>
        private void InitPaints()
        {
            // Text
            _maxValueTextDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White
            };

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
            };

            _iconTextDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White,
                Typeface = SkiaSharpHelper.LoadTtfFont(Fonts.Icons)
            };

            _titleDraw.LinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                IsAntialias = true,
                Color = SKColors.White
            };

            _maxValueDraw.Paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
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
            foreach(var datetime in Values.Airflow.Select(v => v.DateTime))
            {
                if (datetime.Minute != 0)
                    continue;
                    
                if (datetime.TimeOfDay.Ticks == 0)
                    DrawMidnightIndicator(point);
                else if(point.X <= _hoursDraw.Bounds.Right)
                    DrawHourCircle(point);

                point.X += _hoursDraw.Step;
            }

            // Draw text
            _hoursCanvasInfo.Canvas.DrawText($"{_hoursDraw.TotalHours}H",
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
        /// Convert airflow value to percentage, in order to find Y cordinate on graph
        /// </summary>
        /// <param name="airflow"></param>
        /// <returns></returns>
        private float AirflowToPercentage(int airflow)
            => ((Values.Scale - airflow) / (float)Values.Scale);

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

            float percentageFromMaxValue;
            float lastAirflowPercentage = 0;

            // Values
            foreach(var graphValue in Values.Airflow)
            {
                // Airflow
                point.X += (_graphDraw.Step / _graphDraw.Bounds.Right) * _graphDraw.Bounds.Right;

                percentageFromMaxValue = AirflowToPercentage(graphValue.Airflow);

                if (percentageFromMaxValue < 0.80f)
                    point.Y = (percentageFromMaxValue * _graphDraw.Bounds.Bottom) + _graphDraw.Bounds.Top;
                else
                    point.Y = _graphDraw.Bounds.Bottom;

                path.LineTo(point);

                lastAirflowPercentage = graphValue.Airflow;
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
            DrawCurrentValuePercentage(point, lastAirflowPercentage);

            // Draw sensor lables
            GetSensorLabels();

            sensorLabelsCanvas.InvalidateSurface();
        }

        /// <summary>
        /// Draw sensors labels 
        /// </summary>
        private void DrawSensorsLabels()
        {
            foreach(var sensorLabelDraw in _sensorLabelDraws)
                DrawSensorLabel(sensorLabelDraw);
        }

        /// <summary>
        /// Get sensor labels position to display on graph
        /// </summary>
        private void GetSensorLabels()
        {
            _sensorLabelDraws.Clear();
            _previousEventDateTime = default(DateTime);

            DateTime lastMeasurement = Values.Airflow.Last().DateTime;
            DateTime firstMeasurement = Values.Airflow.First().DateTime;

            double totalElapsedSeconds = ((lastMeasurement - firstMeasurement).TotalSeconds);

            float x, y;

            SensorLabelDraw sensorLabelDraw;

            foreach (var graphEvent in Values.Events)
            {
                // Check previous drawn sensor label
                if ((graphEvent.Time - _previousEventDateTime).TotalMinutes < 60)
                    continue;

                // Calculate x coordinate
                x = ((float)(totalElapsedSeconds - ((lastMeasurement - graphEvent.Time).TotalSeconds)) /
                    (float)totalElapsedSeconds) * _graphDraw.Bounds.Right;

                y = (AirflowToPercentage(Values.Airflow.First(a => a.DateTime == graphEvent.Time).Airflow)
                        * _graphDraw.Bounds.Bottom) + _graphDraw.Bounds.Top;

                // Create new sensor label
                sensorLabelDraw = new SensorLabelDraw
                {
                    Point = new SKPoint(x, y),
                    DateTime = graphEvent.Time,
                    Icon = IconsHelper.GetIconForDashboardGraph(graphEvent.Icon),
                };

                // Init sensor label paints
                sensorLabelDraw.ActiveColor = SKColors.White;
                sensorLabelDraw.NormalColor = _graphDraw.FirstGradientColor;

                sensorLabelDraw.LinePaint = new SKPaint
                {
                    Color = sensorLabelDraw.NormalColor,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                sensorLabelDraw.IconTextDraw.Paint = new SKPaint
                {
                    IsAntialias = true,
                    Color = SKColors.White,
                    Typeface = SkiaSharpHelper.LoadTtfFont(Fonts.Icons)
                };

                sensorLabelDraw.HourTextDraw.Paint = new SKPaint
                {
                    Color = SKColors.White,
                    IsAntialias = true
                };

                // Text size
                sensorLabelDraw.LinePaint.StrokeWidth = _size._3px;
                sensorLabelDraw.IconTextDraw.Paint.TextSize = _size._50px;
                sensorLabelDraw.HourTextDraw.Paint.TextSize = _size._25px;
                sensorLabelDraw.HourTextDraw.Margin.Top = _size._5px;

                // Add to sensors list
                _sensorLabelDraws.Add(sensorLabelDraw);

                _previousEventDateTime = graphEvent.Time;
            }
        }

        /// <summary>
        /// Draw sensor label
        /// </summary>
        /// <param name="sensorLabelDraw"></param>
        private void DrawSensorLabel(SensorLabelDraw sensorLabelDraw)
        {
            string time = "";

            // Check if label coords
            if (sensorLabelDraw.Point.Y == _graphDraw.Bounds.Bottom)
                return;

            // Measure icon
            sensorLabelDraw.IconTextDraw.Paint.MeasureText(sensorLabelDraw.Icon, ref sensorLabelDraw.IconTextDraw.Bounds);

            sensorLabelDraw.IconTextDraw.Margin.Bottom = (sensorLabelDraw.IconTextDraw.Paint.TextSize / 1.75f);

            float iconTextSize = sensorLabelDraw.IconTextDraw.Paint.TextSize;
            float iconMidX = Math.Abs(sensorLabelDraw.IconTextDraw.Bounds.MidX);
            float iconX = sensorLabelDraw.Point.X - iconMidX;
            float iconY = sensorLabelDraw.Point.Y - sensorLabelDraw.IconTextDraw.Margin.Bottom;

            float lineMarginBottom = 0;

            // Check if sensor label icon is in bounds
            if (iconY - sensorLabelDraw.IconTextDraw.Paint.TextSize < _graphDraw.Bounds.Top)
                iconY = sensorLabelDraw.Point.Y + sensorLabelDraw.IconTextDraw.Paint.TextSize;

            // Get bounds
            sensorLabelDraw.Bounds = new SKRect
            {
                Top = iconY - iconTextSize,
                Left = iconX,
                Right = sensorLabelDraw.Point.X + iconTextSize,
                Bottom = iconY
            };

            // Check if label is active
            if (sensorLabelDraw.IsActive)
            {
                // Measure text
                time = sensorLabelDraw.DateTime.ToString("HH:mm", CultureInfo.InvariantCulture);

                sensorLabelDraw.HourTextDraw.Paint.MeasureText(time, ref sensorLabelDraw.HourTextDraw.Bounds);

                // Draw active label
                sensorLabelDraw.IconTextDraw.Paint.Color = sensorLabelDraw.LinePaint.Color = sensorLabelDraw.ActiveColor;

                // Draw text
                float textMidX = Math.Abs(sensorLabelDraw.HourTextDraw.Bounds.MidX);
                float textX = sensorLabelDraw.Point.X - textMidX;
                float textY = _graphDraw.Bounds.Bottom;

                _sensorLabelsCanvasInfo.Canvas.DrawText(time,
                    new SKPoint(textX, textY),
                    sensorLabelDraw.HourTextDraw.Paint);

                lineMarginBottom = sensorLabelDraw.HourTextDraw.Paint.TextSize + sensorLabelDraw.HourTextDraw.Margin.Top;
            }
            else
            {
                // Draw normal label
                sensorLabelDraw.IconTextDraw.Paint.Color = sensorLabelDraw.LinePaint.Color = sensorLabelDraw.NormalColor;
            }

            // Draw line
            _sensorLabelsCanvasInfo.Canvas.DrawLine(new SKPoint(sensorLabelDraw.Point.X, iconY),
                new SKPoint(sensorLabelDraw.Point.X, _graphDraw.Bounds.Bottom - lineMarginBottom),
                sensorLabelDraw.LinePaint);

            // Draw sensor label icon
            _sensorLabelsCanvasInfo.Canvas.DrawText(sensorLabelDraw.Icon,
                iconX, iconY,
                sensorLabelDraw.IconTextDraw.Paint);
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

        /// <summary>
        /// Draw current airflo percentage indicator
        /// </summary>
        /// <param name="point"></param>
        /// <param name="percentage"></param>
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

        /// <summary>
        /// Draw graph title
        /// </summary>
        private void DrawTitle()
        {
            float offsetX = _titleDraw.Bounds.Left;

            // Icon
            _maxValueCanvasInfo.Canvas.DrawText(_textConstants.Icon,
                offsetX,
                (_titleDraw.CenterY) + (_iconTextDraw.Bounds.Height / 2),
                _iconTextDraw.Paint);

            // Title
            offsetX += _iconTextDraw.Bounds.Width + _titleTextDraw.Margin.Left;

            _maxValueCanvasInfo.Canvas.DrawText(_textConstants.Title,
                offsetX,
                _titleDraw.Bounds.MidY,
                _titleTextDraw.Paint);

            // Line
            offsetX += _titleTextDraw.Bounds.Width + _titleTextDraw.Margin.Right;

            _maxValueCanvasInfo.Canvas.DrawLine(new SKPoint(offsetX, _titleDraw.CenterY),
                new SKPoint(_titleDraw.Bounds.Right, _titleDraw.CenterY),
                _titleDraw.LinePaint);
        }

        /// <summary>
        /// Draw graph max value
        /// </summary>
        private void DrawMaxValue()
        {
            // Background
            if (Values.Scale > 100)
            {
                _maxValueTextDraw.Paint.Color = SKColors.Transparent;
                _maxValueTextDraw.Paint.BlendMode = SKBlendMode.Clear;

                _maxValueCanvasInfo.Canvas.DrawRoundRect(_maxValueDraw.Bounds,
                _maxValueDraw.Radius,
                _maxValueDraw.Radius,
                _maxValueDraw.Paint);
            }

            // Text
            var point = new SKPoint();

            point.X = _maxValueDraw.Bounds.MidX - Math.Abs(_maxValueTextDraw.Bounds.MidX);
            point.Y = _maxValueDraw.Bounds.MidY + Math.Abs(_maxValueTextDraw.Bounds.MidY);

            _maxValueCanvasInfo.Canvas.DrawText($"{Values.Scale}%", point, _maxValueTextDraw.Paint);
        }

        /// <summary>
        /// Make control responsive for different resolutions
        /// </summary>
        /// <param name="eventArgs"></param>
        private void Calculate(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs eventArgs)
        {
            // Constants
            float screenWidth = eventArgs.Info.Width;

            // Iphone 7 original size
            _size._3px = screenWidth * 0.004f;
            _size._5px = screenWidth * 0.006666666666667f;
            _size._10px = screenWidth * 0.013333333333333f;
            _size._15px = screenWidth * 0.02f;
            _size._25px = screenWidth * 0.033333333333333f;
            _size._30px = screenWidth * 0.04f;
            _size._35px = screenWidth * 0.046666666666667f;
            _size._40px = screenWidth * 0.053333333333333f;
            _size._50px = screenWidth * 0.066666666666667f;

            // Text
            _valueTextDraw.Paint.TextSize = _size._25px;
            _valueTextDraw.Margin.Left = _size._30px;

            _titleTextDraw.Paint.TextSize = _size._35px;
            _titleTextDraw.Margin.Left = _size._15px;
            _titleTextDraw.Margin.Right = _size._25px;
            _titleTextDraw.Paint.MeasureText(_textConstants.Title, ref _titleTextDraw.Bounds);

            _iconTextDraw.Paint.TextSize = _size._35px;
            _iconTextDraw.Paint.MeasureText(_textConstants.Icon, ref _iconTextDraw.Bounds);

            _maxValueTextDraw.Paint.TextSize = _size._25px;

            _maxValueTextDraw.Paint.MeasureText($"{Values.Scale}%", ref _maxValueTextDraw.Bounds);

            // Common bounds
            var rightBound = eventArgs.Info.Width * 0.85f;
            var maxValueLinAndGraphLineIntersectionBound = eventArgs.Info.Height * 0.15f;
            var graphLineAndHoursLineIntersectionBound = eventArgs.Info.Height * 0.95f;

            // Max value
            _titleDraw.Bounds.Top = 0;
            _titleDraw.Bounds.Left = _size._15px;
            _titleDraw.Bounds.Bottom = maxValueLinAndGraphLineIntersectionBound;
            _titleDraw.Bounds.Right = rightBound;

            _titleDraw.CenterY = _titleDraw.Bounds.MidY - Math.Abs(_titleTextDraw.Bounds.MidY);

            _maxValueDraw.Height = _size._40px;

            _maxValueDraw.Bounds.Top = _titleDraw.CenterY - (_maxValueDraw.Height / 2);
            _maxValueDraw.Bounds.Left = rightBound + _size._15px;
            _maxValueDraw.Bounds.Bottom = _maxValueDraw.Bounds.Top + _maxValueDraw.Height;
            _maxValueDraw.Bounds.Right = eventArgs.Info.Width - _size._10px;

            // Graph
            _graphDraw.Step = rightBound / Values.Airflow.Count();

            _graphDraw.Bounds.Top = maxValueLinAndGraphLineIntersectionBound;
            _graphDraw.Bounds.Left = 0;
            _graphDraw.Bounds.Bottom = graphLineAndHoursLineIntersectionBound - _size._15px;
            _graphDraw.Bounds.Right = rightBound;

            _graphDraw.Paint.PathEffect = SKPathEffect.CreateCorner(_size._50px);

            _currentValueIndicatorDraw.Paint.StrokeWidth = _size._3px;
            _currentValueIndicatorDraw.Radius = _size._10px;

            // Hours
            _hoursDraw.Bounds.Top = graphLineAndHoursLineIntersectionBound;
            _hoursDraw.Bounds.Left = 0;
            _hoursDraw.Bounds.Bottom = eventArgs.Info.Height;
            _hoursDraw.Bounds.Right = rightBound;

            _hoursDraw.TotalHours = (int)(Values.Airflow.Last().DateTime -
                 Values.Airflow.First().DateTime).TotalHours;

            if (_hoursDraw.TotalHours < 1)
                _hoursDraw.TotalHours = 1;

            _hoursDraw.Step = _hoursDraw.Bounds.Width / _hoursDraw.TotalHours;

            _hoursDraw.CircleRadius = _size._3px;

            _hoursDraw.MidnightIndicatorPaint.StrokeWidth = _size._3px;

            // Max value
            _titleDraw.LinePaint.StrokeWidth = _size._3px;
            _maxValueDraw.Radius = _size._3px;
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
            eventArgs.Surface.Canvas.DrawRect(_titleDraw.Bounds, paint);

            // Graph
            paint.Color = SKColors.Yellow;
            eventArgs.Surface.Canvas.DrawRect(_graphDraw.Bounds, paint);

            // Hours
            paint.Color = SKColors.Red;

            eventArgs.Surface.Canvas.DrawRect(_hoursDraw.Bounds, paint);

            // Max value
            eventArgs.Surface.Canvas.DrawRoundRect(_maxValueDraw.Bounds,
                _maxValueDraw.Radius,
                _maxValueDraw.Radius,
                _maxValueDraw.Paint);
        }

        /// <summary>
        /// Clear canvas with transparent color
        /// </summary>
        /// <param name="e"></param>
        private void ClearCanvas(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
            => e.Surface.Canvas.Clear(SKColors.Transparent);

        /// <summary>
        /// Convert coordinate to SkiaSharp pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private SKPoint ToPixel(float x, float y)
        {
            return new SKPoint((float)(sensorLabelsCanvas.CanvasSize.Width * x / sensorLabelsCanvas.Width)
                , (float)(sensorLabelsCanvas.CanvasSize.Height * y / sensorLabelsCanvas.Height));
        }

        /// <summary>
        /// Activate sensor label and show time
        /// </summary>
        /// <returns></returns>
        private void ActivateSensorLabel()
        {
            foreach (var sensorLabelDraw in _sensorLabelDraws)
                sensorLabelDraw.IsActive = sensorLabelDraw.Bounds.Contains(_touchPoint);
        }

        #endregion

        #region Events

        private void OnTouch
            (object sender, TouchTracking.TouchActionEventArgs args)
        {
            // Convert touch point to pixel point
            _touchPoint = ToPixel(args.Location.X, args.Location.Y);

            switch (args.Type)
            {
                case TouchActionType.Pressed:

                    ActivateSensorLabel();

                    sensorLabelsCanvas.InvalidateSurface();

                break;
            }
        }

        protected override async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        { 
            base.OnPropertyChanged(propertyName);

            // Values
            if (propertyName == ValuesProperty.PropertyName)
            {
                await this.FadeTo(0.25);

                hoursCanvas.InvalidateSurface();

                await this.FadeTo(1, 750);
            }
        }

        #region Canvas

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

        private void SensorLabelsCanvas_PaintSurface
            (object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get cavans info
            _sensorLabelsCanvasInfo.Canvas = e.Surface.Canvas;
            _sensorLabelsCanvasInfo.ImageInfo = e.Info;
            _sensorLabelsCanvasInfo.Surface = e.Surface;

            // Draw
            ClearCanvas(e);

            DrawSensorsLabels();
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

        public int TotalHours;
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
    public struct TitleDraw
    {
        public SKRect Bounds;

        public SKPaint LinePaint;

        public float CenterY;
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
    /// Max value dra helper
    /// </summary>
    public struct MaxValueDraw
    {
        public float Height;

        public SKRect Bounds;

        public SKPaint Paint;

        public float Radius;
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
    /// Sensor label draw helper
    /// </summary>
    public class SensorLabelDraw
    {
        public SKRect Bounds;

        public SKColor ActiveColor;

        public SKColor NormalColor;

        public bool IsActive { get; set; }

        public DateTime DateTime { get; set; }

        public int Airflow { get; set; }

        public SKPoint Point;

        public SKPaint LinePaint { get; set; }

        public TextDraw IconTextDraw;

        public TextDraw HourTextDraw;

        public string Icon { get; set; }
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

    /// <summary>
    /// Store size calculations, for responsive design
    /// </summary>
    public struct Size
    {
        public float _3px;
        public float _5px;
        public float _10px;
        public float _15px;
        public float _25px;
        public float _30px;
        public float _35px;
        public float _40px;
        public float _50px;
    }
}
