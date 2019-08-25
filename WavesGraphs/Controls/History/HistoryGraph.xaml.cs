using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SkiaSharp;
using WavesGraphs.Controls.Models.Shared;
using WavesGraphs.Models.History;
using Xamarin.Essentials;
using Xamarin.Forms;
using WavesGraphs.Controls.Models.HistoryGraph;
using WavesGraphs.Models.Shared;
using System.Linq;
using WavesGraphs.Helpers;
using System.Diagnostics;
using WavesGraphs.Helpers.Constants;

namespace WavesGraphs.Controls
{
    public partial class HistoryGraph : ContentView
    {
        #region Fields

        // Canvas info
        CanvasInfo _graphCanvasInfo;
        CanvasInfo _valueCanvasInfo;
        CanvasInfo _textCanvasInfo;

        // Draw
        TitleDraw _titleDraw;
        GraphDraw _graphDraw;
        GraphCircleDraw _graphCircleDraw;
        CurrentValueIndicatorDraw _currentValueIndicatorDraw;

        Range _range;

        ElementSize _size;

        GraphValueModel _lastValue;
        ScaleIntervalModel _lastValueScale;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty GraphBoundsProperty = BindableProperty
            .Create(nameof(GraphBounds), typeof(SKRect), typeof(HistoryGraph), default(SKRect));

        public SKRect GraphBounds
        {
            get => (SKRect)GetValue(GraphBoundsProperty);

            set => SetValue(GraphBoundsProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty
            .Create(nameof(Title), typeof(string), typeof(HistoryGraph), default(string));

        public string Title
        {
            get => (string)GetValue(TitleProperty);

            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty DescriptionProperty = BindableProperty
            .Create(nameof(Description), typeof(string), typeof(HistoryGraph), default(string));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);

            set => SetValue(DescriptionProperty, value);
        }

        public static readonly BindableProperty IconProperty = BindableProperty
            .Create(nameof(Icon), typeof(string), typeof(HistoryGraph), default(string));

        public string Icon
        {
            get => (string)GetValue(IconProperty);

            set => SetValue(IconProperty, value);
        }

        public static readonly BindableProperty ScaleIntervalsProperty = BindableProperty
            .Create(nameof(ScaleIntervals), typeof(List<ScaleIntervalModel>), typeof(HistoryGraph), default(List<ScaleIntervalModel>));

        public List<ScaleIntervalModel> ScaleIntervals
        {
            get => (List<ScaleIntervalModel>)GetValue(ScaleIntervalsProperty);

            set => SetValue(ScaleIntervalsProperty, value);
        }

        public static readonly BindableProperty ValuesProperty = BindableProperty
            .Create(nameof(Values), typeof(List<GraphValueModel>), typeof(HistoryGraph), default(List<GraphValueModel>));

        public List<GraphValueModel> Values
        {
            get => (List<GraphValueModel>)GetValue(ValuesProperty);

            set => SetValue(ValuesProperty, value);
        }

        public static readonly BindableProperty ShowValueForProperty = BindableProperty
            .Create(nameof(ShowValueFor), typeof(DateTime), typeof(HistoryGraph), default(DateTime));

        public DateTime ShowValueFor
        {
            get => (DateTime)GetValue(ShowValueForProperty);

            set => SetValue(ShowValueForProperty, value);
        }

        public static readonly BindableProperty PercentageProperty = BindableProperty
            .Create(nameof(Percentage), typeof(double), typeof(HistoryGraph), default(double));

        public double Percentage
        {
            get => (double)GetValue(PercentageProperty);

            set => SetValue(PercentageProperty, value);
        }

        public static readonly BindableProperty ShowValueIndicatorProperty = BindableProperty
            .Create(nameof(ShowValueIndicator), typeof(bool), typeof(HistoryGraph), default(bool));

        public bool ShowValueIndicator
        {
            get => (bool)GetValue(ShowValueIndicatorProperty);

            set => SetValue(ShowValueIndicatorProperty, value);
        }

        public static readonly BindableProperty MeasurementUnitProperty = BindableProperty
            .Create(nameof(MeasurementUnit), typeof(string), typeof(HistoryGraph), default(string));

        public string MeasurementUnit
        {
            get => (string)GetValue(MeasurementUnitProperty);

            set => SetValue(MeasurementUnitProperty, value);
        }

        public static readonly BindableProperty ValueProperty = BindableProperty
            .Create(nameof(Value), typeof(double), typeof(HistoryGraph), default(double));

        public double Value
        {
            get => (double)GetValue(ValueProperty);

            set => SetValue(ValueProperty, value);
        }

        #endregion

        public HistoryGraph()
        {
            InitializeComponent();

            container.PropertyChanged += Container_PropertyChanged;

            graphCanvas.PaintSurface += GraphCanvas_PaintSurface;

            InitPaints();
        }

        #region Methods

        void InitPaints()
        {
            // Top info
            _titleDraw.IconTextDraw.Paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Typeface = SkiaSharpHelper.LoadTtfFont(Fonts.Icons)
            };

            _titleDraw.GraphNameTextDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                FakeBoldText = true,
                Color = SKColor.Parse("#494948")
            };

            _titleDraw.DescriptionTextDraw.Paint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColor.Parse("#333333")
            };

            _titleDraw.LevelTextDraw.Paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
            };

            _titleDraw.ValueTextDraw.Paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
            };

            // Graw draw
            _graphDraw.LinePaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            _graphDraw.BackgroundPaint = new SKPaint
            {
                Color = SKColor.Parse("#FCFCFC"),
                IsAntialias = true
            };

            // Graph circle draw
            _graphCircleDraw.CirclePaint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
            };

            _graphCircleDraw.LinePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            // Current value indicator draw
            _currentValueIndicatorDraw.ValueRectPaint = new SKPaint
            {
                IsAntialias = true
            };

            _currentValueIndicatorDraw.DashedLinePaint = new SKPaint
            {
                IsAntialias = true
            };

            _currentValueIndicatorDraw.ValueTextDraw.Paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                FakeBoldText = true
            };
        }

        void Calculate(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            float screenWidth = e.Info.Width;

            _size._2px = screenWidth * 0.002666666666667f;
            _size._3px = screenWidth * 0.004f;
            _size._5px = screenWidth * 0.006666666666667f;
            _size._8px = screenWidth * 0.010666666666667f;
            _size._10px = screenWidth * 0.013333333333333f;
            _size._15px = screenWidth * 0.02f;
            _size._30px = screenWidth * 0.04f;

            // Title bounds
            _titleDraw.Bounds.Top = 0;
            _titleDraw.Bounds.Left = 0;
            _titleDraw.Bounds.Bottom = e.Info.Height * 0.30f;
            _titleDraw.Bounds.Right = e.Info.Width;

            // Graph bounds
            _graphDraw.Bounds.Top = _titleDraw.Bounds.Bottom;
            _graphDraw.Bounds.Left = 0;
            _graphDraw.Bounds.Bottom = e.Info.Height;
            _graphDraw.Bounds.Right = e.Info.Width * 0.80f;

            _graphDraw.Step = _graphDraw.Bounds.Right / Values.Count;

            // Graph circle
            _graphCircleDraw.LinePaint.StrokeWidth = _size._2px;
            _graphCircleDraw.Radius = _size._8px;

            // Current value indicator
            _currentValueIndicatorDraw.Bounds.Top = _size._15px;

            _currentValueIndicatorDraw.DashedLinePaint.StrokeWidth = _size._3px;
            _currentValueIndicatorDraw.DashedLinePaint.PathEffect = SKPathEffect
                .CreateDash(new float[] { _size._15px, _size._10px }, 0);

            _currentValueIndicatorDraw.ValueTextDraw.Paint.TextSize = _size._30px;

            // Title text
            _titleDraw.IconTextDraw.Paint.TextSize = _titleDraw.Bounds.Height / 2f;
            _titleDraw.IconTextDraw.Margin.Left = 15;

            _titleDraw.GraphNameTextDraw.Paint.TextSize = _titleDraw.Bounds.Height / 3.5f;
            _titleDraw.GraphNameTextDraw.Margin.Left = 20;

            _titleDraw.DescriptionTextDraw.Paint.TextSize = _titleDraw.Bounds.Height / 5f;

            _titleDraw.LevelTextDraw.Paint.TextSize = _titleDraw.GraphNameTextDraw.Paint.TextSize;
            _titleDraw.LevelTextDraw.Margin.Right = 15;

            _titleDraw.ValueTextDraw.Paint.TextSize = _titleDraw.DescriptionTextDraw.Paint.TextSize;
            _titleDraw.ValueTextDraw.Margin.Right = 15;
        }

        #region Draws

        void DrawBounds(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            var paint = new SKPaint
            {
                Color = SKColors.Red,
                StrokeWidth = 3,
                Style = SKPaintStyle.Stroke
            };

            // Title
            e.Surface.Canvas.DrawRect(_titleDraw.Bounds, paint);

            // Graph
            paint.Color = SKColors.Green;
            e.Surface.Canvas.DrawRect(_graphDraw.Bounds, paint);
        }

        void DrawGraph()
        {
            // Get range
            _range.From = scale.ScaleIntervals.First().From;
            _range.To = scale.ScaleIntervals.Last().To;

            // Init paths
            float percentage;
            var linePath = new SKPath();
            var backgroundPath = new SKPath();

            // Initial point
            var point = new SKPoint();
            point.X = _graphDraw.Bounds.Left;
            point.Y = GetYPosition(SkiaSharpHelper.GetPercentage(Values.First().Value, _range));

            linePath.MoveTo(point);

            point.X = _graphDraw.Bounds.Left;
            point.Y = _graphDraw.Bounds.Bottom;

            backgroundPath.MoveTo(point);

            // Get values
            foreach (var model in Values)
            {
                percentage = SkiaSharpHelper.GetPercentage(model.Value, _range);

                point.Y = GetYPosition(percentage);

                linePath.LineTo(point);
                backgroundPath.LineTo(point);

                point.X += _graphDraw.Step;

                model.X = point.X;
                model.Y = point.Y;
            }

            // Draw graph line
            _graphCanvasInfo.Canvas.DrawPath(linePath, _graphDraw.LinePaint);
            linePath.Dispose();

            // Draw background
            backgroundPath.LineTo(point.X, _graphDraw.Bounds.Bottom);
            _graphCanvasInfo.Canvas.DrawPath(backgroundPath, _graphDraw.BackgroundPaint);
            backgroundPath.Dispose();
        }

        float GetYPosition(float percentage)
            => _graphDraw.Bounds.Bottom - (_graphDraw.Bounds.Height * percentage);

        void UpdateGraphCircle()
        {
            GraphValueModel graphValueModel = Values.FirstOrDefault(v => v.DateTime == ShowValueFor);

            if (graphValueModel == null)
                return;

            // Update value
            Value = graphValueModel.Value;

            // Get scale interval
            ScaleIntervalModel scaleIntervalModel = scale.GetScaleInterval(graphValueModel.Value);

            // Calculate point
            var point = new SKPoint(graphValueModel.X - _graphCircleDraw.Radius, graphValueModel.Y);

            // Draw graph circle
            _graphCircleDraw.LinePaint.Color = SKColor.Parse(scaleIntervalModel.Color);
            DrawGraphCircle(point);

            // Draw current value
            if (graphValueModel.DateTime != _lastValue.DateTime)
            {
                // Set indicator color
                _currentValueIndicatorDraw.ValueRectPaint.Color =
                    _currentValueIndicatorDraw.DashedLinePaint.Color =
                    _graphCircleDraw.LinePaint.Color;

                // Draw current value indicator
                DrawCurrentValueIndicator(point);

                // Set level color
            }
        }

        void DrawGraphCircle(SKPoint point)
        {
            // Draw circle
            _valueCanvasInfo.Canvas.DrawCircle(point,
                _graphCircleDraw.Radius,
                _graphCircleDraw.CirclePaint);

            _valueCanvasInfo.Canvas.DrawCircle(point,
                _graphCircleDraw.Radius,
                _graphCircleDraw.LinePaint);

            // Draw line
            _valueCanvasInfo.Canvas.DrawLine(new SKPoint(point.X, point.Y + _graphCircleDraw.Radius),
                new SKPoint(point.X, _graphDraw.Bounds.Bottom),
                _graphCircleDraw.LinePaint);
        }

        void DrawCurrentValueIndicator(SKPoint point)
        {
            // Measure value text
            _currentValueIndicatorDraw.ValueTextDraw.Paint.MeasureText($"{Value}{MeasurementUnit}",
                ref _currentValueIndicatorDraw.ValueTextDraw.Bounds);

            // Set current value indicator new position
            _currentValueIndicatorDraw.Bounds.Left = point.X
                - ((_currentValueIndicatorDraw.ValueTextDraw.Bounds.Width / 2) + _size._15px);

            _currentValueIndicatorDraw.Bounds.Right = point.X
                + ((_currentValueIndicatorDraw.ValueTextDraw.Bounds.Width / 2) + _size._15px);

            _currentValueIndicatorDraw.Bounds.Bottom =
                _currentValueIndicatorDraw.Bounds.Top
                + _currentValueIndicatorDraw.ValueTextDraw.Bounds.Height + _size._30px;

            // Draw value rect
            if (_currentValueIndicatorDraw.Bounds.Left < _graphDraw.Bounds.Left)
            {
                _currentValueIndicatorDraw.Bounds.Left = _graphDraw.Bounds.Left;
                _currentValueIndicatorDraw.Bounds.Right = _currentValueIndicatorDraw.Bounds.Left
                    + (((_currentValueIndicatorDraw.ValueTextDraw.Bounds.Width / 2) + _size._15px) * 2);

                DrawValueIndicatorRect();
            }
            else
                DrawValueIndicatorRect();

            // Pivot point
            var pivot = new SKPoint(point.X,
                _currentValueIndicatorDraw.Bounds.Bottom - _size._2px);

            // Draw triangle
            var trianglePath = new SKPath();

            trianglePath.MoveTo(pivot.X, pivot.Y + _size._15px);

            trianglePath.LineTo(pivot.X - _size._15px, pivot.Y);
            trianglePath.LineTo(pivot.X + _size._15px, pivot.Y);

            trianglePath.Close();

            _valueCanvasInfo.Canvas.DrawPath(trianglePath, _currentValueIndicatorDraw.ValueRectPaint);

            trianglePath.Dispose();

            // Draw dashed line
            pivot.Y += _size._15px;
            point.Y -= _graphCircleDraw.Radius;

            _valueCanvasInfo.Canvas.DrawLine(pivot, point,
                _currentValueIndicatorDraw.DashedLinePaint);
        }

        void DrawValueIndicatorRect()
        {
            // Rect
            _valueCanvasInfo.Canvas.DrawRoundRect(_currentValueIndicatorDraw.Bounds,
                _size._5px,
                _size._5px,
                _currentValueIndicatorDraw.ValueRectPaint);

            // Value
            _valueCanvasInfo.Canvas.DrawText($"{Value}{MeasurementUnit}",
                new SKPoint
                {
                    X = _currentValueIndicatorDraw.Bounds.MidX
                        - Math.Abs(_currentValueIndicatorDraw.ValueTextDraw.Bounds.MidX),

                    Y = _currentValueIndicatorDraw.Bounds.MidY
                        + Math.Abs(_currentValueIndicatorDraw.ValueTextDraw.Bounds.MidY),
                },
                _currentValueIndicatorDraw.ValueTextDraw.Paint);
        }

        void DrawTopText()
        {
            // Icon
            _titleDraw.IconTextDraw.Paint.MeasureText(IconsHelper.Co2Event
                , ref _titleDraw.IconTextDraw.Bounds);

            _titleDraw.IconTextDraw.Paint.Color = SKColor.Parse(_lastValueScale.Color);

            var iconPoint = new SKPoint();
            iconPoint.X = _titleDraw.Bounds.Left + _titleDraw.IconTextDraw.Margin.Left;
            iconPoint.Y = _titleDraw.Bounds.MidY + Math.Abs(_titleDraw.IconTextDraw.Bounds.MidY);

            _textCanvasInfo.Canvas.DrawText(IconsHelper.Co2Event,
                iconPoint,
                _titleDraw.IconTextDraw.Paint);

            // Graph name
            _titleDraw.GraphNameTextDraw.Paint.MeasureText(Title
                , ref _titleDraw.GraphNameTextDraw.Bounds);

            var graphNamePoint = new SKPoint();
            graphNamePoint.X = (iconPoint.X + _titleDraw.IconTextDraw.Bounds.Width)
                + _titleDraw.GraphNameTextDraw.Margin.Left;
            graphNamePoint.Y = (iconPoint.Y - _titleDraw.IconTextDraw.Bounds.Height)
                + _titleDraw.GraphNameTextDraw.Bounds.Height;

            _textCanvasInfo.Canvas.DrawText(Title,
                graphNamePoint,
                _titleDraw.GraphNameTextDraw.Paint);

            // Description
            _titleDraw.DescriptionTextDraw.Paint.MeasureText(Description
                , ref _titleDraw.DescriptionTextDraw.Bounds);

            var descriptionPoint = new SKPoint();
            descriptionPoint.X = graphNamePoint.X;
            descriptionPoint.Y = iconPoint.Y;

            _textCanvasInfo.Canvas.DrawText(Description,
                descriptionPoint,
                _titleDraw.DescriptionTextDraw.Paint);

            // Level
            _titleDraw.LevelTextDraw.Paint.MeasureText(_lastValueScale.Name
                , ref _titleDraw.LevelTextDraw.Bounds);

            var levelNamePoint = new SKPoint();
            levelNamePoint.X = (_titleDraw.Bounds.Right - _titleDraw.LevelTextDraw.Margin.Right)
                - _titleDraw.LevelTextDraw.Bounds.Width;
            levelNamePoint.Y = graphNamePoint.Y;

            if (ShowValueIndicator)
                _titleDraw.LevelTextDraw.Paint.Color = SKColors.Transparent;
            else
                _titleDraw.LevelTextDraw.Paint.Color = _titleDraw.IconTextDraw.Paint.Color;

            _textCanvasInfo.Canvas.DrawText(_lastValueScale.Name,
                levelNamePoint,
                _titleDraw.LevelTextDraw.Paint);

            // Value
            _titleDraw.ValueTextDraw.Paint.MeasureText($"{_lastValue.Value}{MeasurementUnit}"
                , ref _titleDraw.ValueTextDraw.Bounds);

            var valuePoint = new SKPoint();
            valuePoint.X = (_titleDraw.Bounds.Right - _titleDraw.ValueTextDraw.Margin.Right)
                - _titleDraw.ValueTextDraw.Bounds.Width;
            valuePoint.Y = descriptionPoint.Y;

            if (ShowValueIndicator)
            {
                _titleDraw.ValueTextDraw.Paint.Color = SKColors.Transparent;
                textCanvas.Opacity = 0.5;
            }
            else
            {
                _titleDraw.ValueTextDraw.Paint.Color = _titleDraw.IconTextDraw.Paint.Color;
                textCanvas.Opacity = 1;
            }

            _textCanvasInfo.Canvas.DrawText($"{_lastValue.Value}{MeasurementUnit}",
                valuePoint,
                _titleDraw.ValueTextDraw.Paint);
        }

        #endregion

        #endregion

        #region Events

        #region Property Change

        private void Container_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Adjust container height
            if (e.PropertyName == WidthProperty.PropertyName)
            {
                double width = container.Width;
                container.HeightRequest = container.Width * 0.55;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Scale intervals
            if (propertyName == ScaleIntervalsProperty.PropertyName)
                scale.ScaleIntervals = ScaleIntervals;

            // Values
            if (propertyName == ValuesProperty.PropertyName)
            {
                graphCanvas.InvalidateSurface();

                _lastValue = Values.Last();

                _lastValueScale = scale.GetScaleInterval(_lastValue.Value);
            }

            // Show value for selected datetime
            if (propertyName == ShowValueForProperty.PropertyName)
                valueCanvas.InvalidateSurface();

            // Show value indicator
            if (propertyName == ShowValueIndicatorProperty.PropertyName)
                textCanvas.InvalidateSurface();

            // Percentage
            if (propertyName == PercentageProperty.PropertyName)
            {

            }

            // Icon
            if (propertyName == IconProperty.PropertyName)
            {

            }

            // Description
            if (propertyName == DescriptionProperty.PropertyName)
            {

            }

            // Name
            if (propertyName == TitleProperty.PropertyName)
            {

            }
        }

        #endregion

        #region Canvases

        private void GraphCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get canvas info
            _graphCanvasInfo.Canvas = e.Surface.Canvas;
            _graphCanvasInfo.Surface = e.Surface;
            _graphCanvasInfo.ImageInfo = e.Info;

            // Clear 
            e.Surface.Canvas.Clear(SKColors.Transparent);

            // Init
            Calculate(e);

            // Draw
            DrawGraph();

            //DrawBounds(e);

            valueCanvas.PaintSurface += ValueCanvas_PaintSurface;
            valueCanvas.InvalidateSurface();

            textCanvas.PaintSurface += TextCanvas_PaintSurface;
            textCanvas.InvalidateSurface();
        }

        private void TextCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get canvas info
            _textCanvasInfo.Canvas = e.Surface.Canvas;
            _textCanvasInfo.Surface = e.Surface;
            _textCanvasInfo.ImageInfo = e.Info;

            // Clear
            e.Surface.Canvas.Clear(SKColors.Transparent);

            // Draw
            DrawTopText();
        }

        private void ValueCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get canvas info
            _valueCanvasInfo.Canvas = e.Surface.Canvas;
            _valueCanvasInfo.Surface = e.Surface;
            _valueCanvasInfo.ImageInfo = e.Info;

            // Clear
            e.Surface.Canvas.Clear(SKColors.Transparent);

            // Draw
            UpdateGraphCircle();
        }

        #endregion

        #endregion
    }
}
