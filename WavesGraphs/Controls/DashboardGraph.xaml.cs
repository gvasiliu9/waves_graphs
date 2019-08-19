using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SkiaSharp;
using TouchTracking;
using WavesGraphs.Helpers;
using WavesGraphs.Helpers.Constants;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class DashboardGraph : ContentView
    {
        #region Fields

        // Bindable properties
        private GraphValues _graphValues;

        private string[] _timeAsString = {
            "2019-08-18T23:59:59Z",
            "2019-08-19T00:00:00Z",
            "2019-08-19T00:15:00Z",
            "2019-08-19T00:30:00Z",
            "2019-08-19T00:45:00Z",
            "2019-08-19T01:00:00Z",
            "2019-08-19T01:15:00Z",
            "2019-08-19T01:30:00Z",
            "2019-08-19T01:45:00Z",
            "2019-08-19T02:00:00Z",
            "2019-08-19T02:15:00Z",
            "2019-08-19T02:30:00Z",
            "2019-08-19T02:45:00Z",
            "2019-08-19T03:00:00Z",
            "2019-08-19T03:15:00Z",
            "2019-08-19T03:30:00Z",
            "2019-08-19T03:45:00Z",
            "2019-08-19T04:00:00Z",
            "2019-08-19T04:15:00Z",
            "2019-08-19T04:30:00Z",
            "2019-08-19T04:45:00Z",
            "2019-08-19T05:00:00Z",
            "2019-08-19T05:15:00Z",
            "2019-08-19T05:30:00Z",
            "2019-08-19T05:45:00Z",
            "2019-08-19T06:00:00Z",
            "2019-08-19T06:15:00Z",
            "2019-08-19T06:30:00Z",
            "2019-08-19T06:45:00Z",
            "2019-08-19T07:00:00Z",
            "2019-08-19T07:15:00Z",
            "2019-08-19T07:30:00Z",
            "2019-08-19T07:45:00Z",
            "2019-08-19T08:00:00Z",
            "2019-08-19T08:15:00Z",
            "2019-08-19T08:30:00Z",
            "2019-08-19T08:45:00Z",
            "2019-08-19T09:00:00Z",
            "2019-08-19T09:15:00Z",
            "2019-08-19T09:30:00Z",
            "2019-08-19T09:45:00Z",
            "2019-08-19T10:00:00Z"
        };

        private int?[] _airflow =
            {
                100,
                100,
                100,
                100,
                150,
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
                80,
                80,
                80,
                80,
                80,
                80,
                80,
                80,
                80,
                80,
                80
            };

        // Canvas info
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

        // Sensors
        private List<SensorLabelDraw> _sensorLabelDraws;

        //private SensorLabelDraw _temperatureLabelDraw;
        //private SensorLabelDraw _vocLabelDraw;
        //private SensorLabelDraw _co2LabelDraw;

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

        public DashboardGraph()
        {
            InitializeComponent();

            hoursCanvas.PaintSurface += HoursCanvas_PaintSurface;

            InitPaints();

            InitGraphValues();

            _textConstants.Hours = "24H";
            _textConstants.Title = "Ventilation";
            _textConstants.Icon = IconsHelper.Ventilator;

            _sensorLabelDraws = new List<SensorLabelDraw>();
        }

        #region Methods

        private void InitGraphValues()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            _graphValues = new GraphValues();
            _graphValues.Airflow = new List<GraphAirflowModel>();

            for (int i = 0; i < _timeAsString.Length; i++)
            {
                _graphValues.Airflow.Add(new GraphAirflowModel
                {
                    DateTime = DateTime.ParseExact(_timeAsString[i], dateFormat, CultureInfo.InvariantCulture),
                    Airflow = _airflow[i] ?? 0,
                });
            }

            // Check max airflow value, to determine graph scale
            int maxAirflow = _graphValues.Airflow.Select(v => v.Airflow).Max();

            if (maxAirflow <= 100)
                _graphValues.Scale = 100;
            else if (maxAirflow > 100 && maxAirflow <= 150)
                _graphValues.Scale = 150;
            else
                _graphValues.Scale = 200;

            // Events
            _graphValues.Events = new List<GraphEventModel>();

            _graphValues.Events.Add(new GraphEventModel
            {
                 Icon = "CO2",
                 Time = _graphValues.Airflow[5].DateTime,
            });
        }

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

            // Sensors
            //_temperatureLabelDraw.ActiveColor = SKColors.White;
            //_temperatureLabelDraw.NormalColor = _graphDraw.FirstGradientColor;

            //_temperatureLabelDraw.LinePaint = new SKPaint
            //{
            //    Color = _temperatureLabelDraw.NormalColor,
            //    IsAntialias = true,
            //    Style = SKPaintStyle.Stroke
            //};

            //_temperatureLabelDraw.IconTextDraw.Paint = new SKPaint
            //{
            //    IsAntialias = true,
            //    Color = SKColors.White,
            //    Typeface = SkiaSharpHelper.LoadTtfFont(Fonts.Icons)
            //};

            //_temperatureLabelDraw.HourTextDraw.Paint = new SKPaint
            //{
            //    Color = SKColors.White,
            //    IsAntialias = true
            //};

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
            foreach(var datetime in _graphValues.Airflow.Select(v => v.DateTime))
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

        private float AirflowToPercentage(int airflow)
            => ((_graphValues.Scale - airflow) / (float)_graphValues.Scale);

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
            foreach(var graphValue in _graphValues.Airflow)
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
            sensorLabelsCanvas.InvalidateSurface();
        }

        /// <summary>
        /// Draw sensors labels 
        /// </summary>
        private void DrawSensorsLabels()
        {
            DateTime lastMeasurement = _graphValues.Airflow.Last().DateTime;
            DateTime firstMeasurement = _graphValues.Airflow.First().DateTime;

            double totalElapsedSeconds = ((lastMeasurement - firstMeasurement).TotalSeconds);

            float x;

            SensorLabelDraw sensorLabelDraw;

            foreach (var graphEvent in _graphValues.Events)
            {
                x = (float)(totalElapsedSeconds - ((lastMeasurement - graphEvent.Time).TotalSeconds)) / (float)totalElapsedSeconds;

                sensorLabelDraw = new SensorLabelDraw
                {
                    Point = new SKPoint(x * _graphDraw.Bounds.Right,
                        _graphValues.Airflow.First(a => a.DateTime == graphEvent.Time).Airflow),
                    DateTime = graphEvent.Time,
                    Icon = graphEvent.Icon
                };

                _sensorLabelDraws.Add(sensorLabelDraw);

                DrawSensorLabel(ref sensorLabelDraw);
            }
        }

        /// <summary>
        /// Draw sensor label
        /// </summary>
        /// <param name="sensorLabelDraw"></param>
        private void DrawSensorLabel(ref SensorLabelDraw sensorLabelDraw)
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
            if (_graphValues.Scale > 100)
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

            _maxValueCanvasInfo.Canvas.DrawText($"{_graphValues.Scale}%", point, _maxValueTextDraw.Paint);
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
            var _3px = screenWidth * 0.004f;
            var _5px = screenWidth * 0.006666666666667f;
            var _10px = screenWidth * 0.013333333333333f;
            var _15px = screenWidth * 0.02f;
            var _25px = screenWidth * 0.033333333333333f;
            var _30px = screenWidth * 0.04f;
            var _35px = screenWidth * 0.046666666666667f;
            var _40px = screenWidth * 0.053333333333333f;
            var _50px = screenWidth * 0.066666666666667f;

            // Text
            _valueTextDraw.Paint.TextSize = _25px;
            _valueTextDraw.Margin.Left = _30px;

            _titleTextDraw.Paint.TextSize = _35px;
            _titleTextDraw.Margin.Left = _15px;
            _titleTextDraw.Margin.Right = _25px;
            _titleTextDraw.Paint.MeasureText(_textConstants.Title, ref _titleTextDraw.Bounds);

            _iconTextDraw.Paint.TextSize = _35px;
            _iconTextDraw.Paint.MeasureText(_textConstants.Icon, ref _iconTextDraw.Bounds);

            _maxValueTextDraw.Paint.TextSize = _25px;

            _maxValueTextDraw.Paint.MeasureText($"{_graphValues.Scale}%", ref _maxValueTextDraw.Bounds);

            // Common bounds
            var rightBound = eventArgs.Info.Width * 0.85f;
            var maxValueLinAndGraphLineIntersectionBound = eventArgs.Info.Height * 0.15f;
            var graphLineAndHoursLineIntersectionBound = eventArgs.Info.Height * 0.95f;

            // Max value
            _titleDraw.Bounds.Top = 0;
            _titleDraw.Bounds.Left = _15px;
            _titleDraw.Bounds.Bottom = maxValueLinAndGraphLineIntersectionBound;
            _titleDraw.Bounds.Right = rightBound;

            _titleDraw.CenterY = _titleDraw.Bounds.MidY - Math.Abs(_titleTextDraw.Bounds.MidY);

            _maxValueDraw.Height = _40px;

            _maxValueDraw.Bounds.Top = _titleDraw.CenterY - (_maxValueDraw.Height / 2);
            _maxValueDraw.Bounds.Left = rightBound + _15px;
            _maxValueDraw.Bounds.Bottom = _maxValueDraw.Bounds.Top + _maxValueDraw.Height;
            _maxValueDraw.Bounds.Right = eventArgs.Info.Width - _10px;

            // Graph
            _graphDraw.Step = rightBound / _graphValues.Airflow.Count();

            _graphDraw.Bounds.Top = maxValueLinAndGraphLineIntersectionBound;
            _graphDraw.Bounds.Left = 0;
            _graphDraw.Bounds.Bottom = graphLineAndHoursLineIntersectionBound - _15px;
            _graphDraw.Bounds.Right = rightBound;

            _graphDraw.Paint.PathEffect = SKPathEffect.CreateCorner(_50px);

            _currentValueIndicatorDraw.Paint.StrokeWidth = _3px;
            _currentValueIndicatorDraw.Radius = _10px;

            // Hours
            _hoursDraw.Bounds.Top = graphLineAndHoursLineIntersectionBound;
            _hoursDraw.Bounds.Left = 0;
            _hoursDraw.Bounds.Bottom = eventArgs.Info.Height;
            _hoursDraw.Bounds.Right = rightBound;

            _hoursDraw.Step = _hoursDraw.Bounds.Width /
                (float)(_graphValues.Airflow.Last().DateTime -
                 _graphValues.Airflow.First().DateTime).TotalHours;

            _hoursDraw.CircleRadius = _3px;

            _hoursDraw.MidnightIndicatorPaint.StrokeWidth = _3px;

            // Max value
            _titleDraw.LinePaint.StrokeWidth = _3px;
            _maxValueDraw.Radius = _3px;

            // Sensors lables
            //_temperatureLabelDraw.LinePaint.StrokeWidth = _3px;
            //_temperatureLabelDraw.IconTextDraw.Paint.TextSize = _50px;
            //_temperatureLabelDraw.HourTextDraw.Paint.TextSize = _25px;
            //_temperatureLabelDraw.HourTextDraw.Margin.Top = _5px;

            //_vocLabelDraw = _co2LabelDraw = _temperatureLabelDraw;
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
        private bool ActivateSensorLabel()
        {
            bool result;

            //result = _temperatureLabelDraw.IsActive = IsSensorLabelActivated(_temperatureLabelDraw);
            //result = _co2LabelDraw.IsActive = IsSensorLabelActivated(_co2LabelDraw);
            //result = _vocLabelDraw.IsActive = IsSensorLabelActivated(_vocLabelDraw);

            return false;
        }

        /// <summary>
        /// Check if sensor label was touched
        /// </summary>
        /// <param name="sensorLabelDraw"></param>
        /// <returns></returns>
        private bool IsSensorLabelActivated(SensorLabelDraw sensorLabelDraw)
            => sensorLabelDraw.Bounds.Contains(_touchPoint);

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
                    // Check if the inner circle was touched
                    if (ActivateSensorLabel())
                    {
                        _touchId = args.Id;

                        sensorLabelsCanvas.InvalidateSurface();
                    }
                    else
                    {
                        sensorLabelsCanvas.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Moved:
                    if (_touchId == args.Id)
                    {
                        // No action
                    }

                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    {
                        _touchId = -1;

                        // No action
                    }
                    break;
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
    /// Graph value model used to draw graph line
    /// </summary>
    public class GraphAirflowModel
    {
        public DateTime DateTime;

        public int Airflow;
    }

    public class GraphEventModel
    {
        public DateTime Time { get; set; }

        public string Icon { get; set; }
    }

    /// <summary>
    /// Store sensors values
    /// </summary>
    public class GraphValues
    {
        public int Scale;

        public List<GraphAirflowModel> Airflow;

        public List<GraphEventModel> Events;
    }

    /// <summary>
    /// Sensor label draw helper
    /// </summary>
    public struct SensorLabelDraw
    {
        public SKRect Bounds;

        public SKColor ActiveColor;

        public SKColor NormalColor;

        public bool IsActive;

        public DateTime DateTime;

        public int Airflow;

        public SKPoint Point;

        public SKPaint LinePaint;

        public TextDraw IconTextDraw;

        public TextDraw HourTextDraw;

        public string Icon;
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
