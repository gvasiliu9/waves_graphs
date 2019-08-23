using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SkiaSharp;
using WavesGraphs.Controls.Models.Shared;
using WavesGraphs.Models.History;
using Xamarin.Essentials;
using Xamarin.Forms;
using WavesGraphs.Controls.Models.HistoryGraph;

namespace WavesGraphs.Controls
{
    public partial class HistoryGraph : ContentView
    {
        #region Fields

        // Canvas info
        CanvasInfo _graphCanvasInfo;
        CanvasInfo _valueCanvasInfo;

        // Draw
        TitleDraw _titleDraw;
        GraphDraw _graphDraw;

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
            _titleDraw.DescriptionTextDraw.Paint = new SKPaint
            {

            };

            _titleDraw.GraphNameTextDraw.Paint = new SKPaint
            {

            };

            _titleDraw.IconTextDraw.Paint = new SKPaint
            {

            };

            _titleDraw.LevelTextDraw.Paint = new SKPaint
            {

            };

            _titleDraw.ValueTextDraw.Paint = new SKPaint
            {

            };

            // 
        }

        void Calculate(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
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

        }

        void DrawValueIndicator()
        {

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
            {
                scale.ScaleIntervals = ScaleIntervals;
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
            DrawValueIndicator();
        }

        #endregion

        #endregion
    }
}
