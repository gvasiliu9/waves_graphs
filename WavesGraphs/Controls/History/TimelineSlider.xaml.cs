using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using SkiaSharp;
using TouchTracking;
using WavesGraphs.Controls.Models.Shared;
using Xamarin.Forms;
using WavesGraphs.Controls.Models.History.TimelineSlider;
using WavesGraphs.Helpers;

namespace WavesGraphs.Controls
{
    public partial class TimelineSlider : ContentView
    {
        #region Fields

        TimelineType _type;

        TimelineSegment _currentTimelineSegment;
        TimelineSegment _previousTimelineSegment;

        // Canvas in
        CanvasInfo _timelineCanvasInfo;
        CanvasInfo _thumbCanvasInfo;
        CanvasInfo _thumbTextCanvasInfo;

        // Timeline segments
        List<TimelineSegment> _timelineSegments;

        // Draw helpers
        TimeLineDraw _timeLineDraw;
        ThumbDraw _thumbDraw;
        TextDraw _timeLabelTextDraw;

        ElementSize _size;

        // Touch tracking
        Touch _touch;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty PeriodProperty = BindableProperty
            .Create(nameof(Period), typeof(List<DateTime>), typeof(TimelineSlider), default(List<DateTime>));

        public List<DateTime> Period
        {
            get => (List<DateTime>)GetValue(PeriodProperty);

            set => SetValue(PeriodProperty, value);
        }

        public static readonly BindableProperty ValueProperty = BindableProperty
            .Create(nameof(Value), typeof(double), typeof(TimelineSlider), default(double));

        public double Value
        {
            get => (double)GetValue(ValueProperty);

            set => SetValue(ValueProperty, value);
        }

        public static readonly BindableProperty TimeProperty = BindableProperty
            .Create(nameof(Time), typeof(DateTime), typeof(TimelineSlider), default(DateTime));

        public DateTime Time
        {
            get => (DateTime)GetValue(TimeProperty);

            set => SetValue(TimeProperty, value);
        }

        public static readonly BindableProperty IsMovingProperty = BindableProperty
            .Create(nameof(IsMoving), typeof(bool), typeof(TimelineSlider), default(bool));

        public bool IsMoving
        {
            get => (bool)GetValue(IsMovingProperty);

            set => SetValue(IsMovingProperty, value);
        }

        public static readonly BindableProperty ThumbPositionProperty = BindableProperty
            .Create(nameof(ThumbPosition), typeof(float), typeof(TimelineSlider), default(float));

        public float ThumbPosition
        {
            get => (float)GetValue(ThumbPositionProperty);

            set => SetValue(ThumbPositionProperty, value);
        }

        #endregion

        public TimelineSlider()
        {
            InitializeComponent();

            timelineCanvas.PaintSurface += TimelineCanvas_PaintSurface;

            InitPaints();
        }

        #region Methods

        #region Shared

        void InitPaints()
        {
            // Timeline
            _timeLineDraw.HourCirclePaint = new SKPaint
            {
                Color = SKColor.Parse("#D4D4D4"),
                IsAntialias = true
            };

            _timeLineDraw.DayLinePaint = new SKPaint
            {
                Color = SKColor.Parse("#D4D4D4"),
                Style = SKPaintStyle.Stroke,
                IsAntialias = true
            };

            // Thumb
            _thumbDraw.Paint = new SKPaint
            {
                Color = SKColor.Parse("#494948"),
                IsAntialias = true
            };

            _thumbDraw.TextDraw.Paint = new SKPaint
            {
                Color = SKColor.Parse("#D4D4D4"),
                IsAntialias = true,
                FakeBoldText = true
            };

            // Timeline label
            _timeLabelTextDraw.Paint = new SKPaint
            {
                Color = SKColor.Parse("#D4D4D4"),
                IsAntialias = true,
                FakeBoldText = true,
            };
        }

        void TouchAction()
        {
            GetTime();

            IsMoving = true;

            // Check if touch is in bounds
            if (IsThumbInBounds())
            {
                _thumbDraw.Point.X = _touch.Current.X;
                thumbCanvas.InvalidateSurface();
            }

            thumbTextCanvas.InvalidateSurface();
        }

        void DrawThumb()
        {
            _thumbCanvasInfo.Canvas
                .DrawCircle(_thumbDraw.Point, _thumbDraw.Radius, _thumbDraw.Paint);
        }

        bool IsThumbInBounds()
        {
            var leftBound = _touch.Current.X - (_thumbDraw.Radius + _size._15px);
            var rightBound = _touch.Current.X + _thumbDraw.Radius;

            return rightBound <= _timeLineDraw.Bounds.Right && leftBound >= _timeLineDraw.Bounds.Left;
        }

        void UpdateText()
        {
            // Initial text
            if (!IsMoving)
            {
                // Thumb text
                DrawThumbText("Now");

                // Time label text
                switch (_type)
                {
                    case TimelineType.Day:
                        DrawTimelineLabelText("24H");
                        break;
                    case TimelineType.Week:
                        DrawTimelineLabelText("7D");
                        break;
                }
            }
            else
            {
                // Draw selected time
                DrawTimelineLabelText(Time.ToString("HH:mm"));

                // Week name
                if (_type == TimelineType.Week)
                    DrawThumbText(Time.DayOfWeek.ToString().Substring(0, 3));
            }
        }

        void DrawThumbText(string text)
        {
            _thumbDraw.TextDraw.Paint.MeasureText(text, ref _thumbDraw.TextDraw.Bounds);

            _thumbTextCanvasInfo.Canvas.DrawText(text,
                new SKPoint
                {
                    X = _thumbDraw.Point.X - Math.Abs(_thumbDraw.TextDraw.Bounds.MidX),
                    Y = _thumbDraw.Point.Y + Math.Abs(_thumbDraw.TextDraw.Bounds.MidY),
                },
                _thumbDraw.TextDraw.Paint);
        }

        void DrawHourCircle(SKPoint point)
        {
            _timelineCanvasInfo.Canvas.DrawCircle(point,
                _timeLineDraw.HourCircleRadius,
                _timeLineDraw.HourCirclePaint);
        }

        void DrawDayLine(SKPoint point)
        {
            var point0 = new SKPoint();
            var point1 = new SKPoint();

            point0.X = point1.X = point.X;

            point0.Y = point.Y - _timeLineDraw.DayLineHeight;
            point1.Y = point.Y + _timeLineDraw.DayLineHeight;

            _timelineCanvasInfo.Canvas.DrawLine(point0, point1, _timeLineDraw.DayLinePaint);
        }

        void Calculate(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            _size._3px = e.Info.Width * 0.004109589041096f;
            _size._13px = e.Info.Width * 0.017808219178082f;
            _size._15px = e.Info.Width * 0.020547945205479f;
            _size._20px = e.Info.Width * 0.027397260273973f;

            // Timeline bounds
            _timeLineDraw.Bounds.Top = _size._15px;
            _timeLineDraw.Bounds.Left = 0;
            _timeLineDraw.Bounds.Bottom = e.Info.Height - _size._15px;
            _timeLineDraw.Bounds.Right = e.Info.Width * 0.80f;

            // Get timeline segments
            GetTimelineSegments();

            // Timeline
            _timeLineDraw.HourCircleRadius = _size._3px;
            _timeLineDraw.DayLineHeight = _size._13px;
            _timeLineDraw.DayLinePaint.StrokeWidth = _size._3px;

            // Thumb
            _thumbDraw.Radius = _timeLineDraw.Bounds.Height / 2;
            _thumbDraw.Point = new SKPoint(_timeLineDraw.Bounds.Right - _thumbDraw.Radius,
                _timeLineDraw.Bounds.MidY);

            _thumbDraw.TextDraw.Paint.TextSize = (_thumbDraw.Radius / 2);

            _touch.Current.X = _timeLineDraw.Bounds.Right;

            // Timelabel text
            _timeLabelTextDraw.Paint.TextSize = _timeLineDraw.Bounds.Height / 2.75f;
            _timeLabelTextDraw.Margin.Right = _size._20px;
        }

        void GetTimelineSegments()
        {
            switch (_type)
            {
                case TimelineType.Day: GetDayTimelineSegments(); break;
                case TimelineType.Week: GetWeekTimelineSegments(); break;
            }
        }

        void GetTime()
        {
            float touchX = _touch.Current.X;

            _currentTimelineSegment = _timelineSegments
                .FirstOrDefault(s => touchX >= s.Bounds.Left && touchX <= s.Bounds.Right);

            if (_currentTimelineSegment == null)
                return;

            int index = (int)((_currentTimelineSegment.Hours.Count() - 1) *
                (1 - ((_currentTimelineSegment.Bounds.Right - touchX) / _currentTimelineSegment.Bounds.Width)));

            Time = _currentTimelineSegment.Hours[index];

            Vibrate();

            thumbTextCanvas.InvalidateSurface();
        }

        void DrawTimeline()
        {
            switch (_type)
            {
                case TimelineType.Day: DrawDayTimeline(); break;
                case TimelineType.Week: DrawWeekTimeline(); break;
            }
        }

        void DrawTimelineLabelText(string text)
        {
            // Measure text
            _timeLabelTextDraw.Paint.MeasureText(text, ref _timeLabelTextDraw.Bounds);

            // Draw
            _thumbTextCanvasInfo.Canvas.DrawText(
                text,
                new SKPoint
                {
                    X = (_timelineCanvasInfo.ImageInfo.Width - _timeLabelTextDraw.Margin.Right)
                        - _timeLabelTextDraw.Bounds.Width,
                    Y = _timeLineDraw.Bounds.MidY + Math.Abs(_timeLabelTextDraw.Bounds.MidY)
                }
                ,
                _timeLabelTextDraw.Paint);
        }

        void Vibrate()
        {
            try
            {
                if (_currentTimelineSegment != _previousTimelineSegment)
                {
                    Xamarin.Essentials.Vibration.Vibrate(15);

                    _previousTimelineSegment = _currentTimelineSegment;
                }
            }
            catch
            {
                Debug.WriteLine("ex: Vibration is not supported...");
            }
        }

        #endregion

        #region Week

        void GetWeekTimelineSegments()
        {
            // Init
            _timelineSegments = new List<TimelineSegment>();

            // Get timeline hours
            foreach (var hours in Period.GroupBy(p => new { p.Day, p.Month }).Select(group => group))
            {
                // Add timeline
                _timelineSegments.Add(new TimelineSegment
                {
                    Hours = hours.ToList(),
                });
            }

            // Calculate step
            _timeLineDraw.Step = _timeLineDraw.Bounds.Right / Period.Count();
        }

        void DrawWeekTimeline()
        {
            // Init point
            var point = new SKPoint();
            point.Y = _timeLineDraw.Bounds.MidY;

            int hourIndex;

            // Draw segments
            foreach (var timelineSegment in _timelineSegments)
            {
                // Left bound
                timelineSegment.Bounds.Left = (float)Math.Round(point.X);

                // Draw hours circle
                hourIndex = 0;

                foreach (var hour in timelineSegment.Hours)
                {
                    point.X += _timeLineDraw.Step;

                    hourIndex++;

                    if (hour == timelineSegment.Hours.Last() && timelineSegment != _timelineSegments.Last())
                        DrawDayLine(point);
                    else if (hourIndex % 6 == 0 &&
                        (timelineSegment.Hours.Last() - timelineSegment.Hours.First()).TotalHours >= 8)
                    {
                        DrawHourCircle(point);
                    }
                }

                // Right bound
                timelineSegment.Bounds.Right = (float)Math.Round(point.X);
            }
        }

        #endregion

        #region Day

        void GetDayTimelineSegments()
        {
            // Init
            _timelineSegments = new List<TimelineSegment>();

            var timelineSegment = new TimelineSegment();

            timelineSegment.Bounds.Left = (float)Math.Round(_timeLineDraw.Bounds.Left);
            timelineSegment.Bounds.Right = (float)Math.Round(_timeLineDraw.Bounds.Right);

            // Get hours
            timelineSegment.Hours = Period;

            // Calculate step
            _timeLineDraw.Step = _timeLineDraw.Bounds.Right / (Period?.Count() ?? 1);

            // Add timeline segment
            _timelineSegments.Add(timelineSegment);
        }

        void DrawDayTimeline()
        {
            // Init point
            var point = new SKPoint();
            //point.X = _timeLineDraw.Step;
            point.Y = _timeLineDraw.Bounds.MidY;

            // Draw hours
            foreach (DateTime hour in _timelineSegments.FirstOrDefault().Hours)
            {
                if (hour.TimeOfDay.Ticks == 0)
                    DrawDayLine(point);
                else if (hour.Minute == 0)
                    DrawHourCircle(point);

                point.X += _timeLineDraw.Step;
            }
        }

        #endregion

        #endregion

        #region Events

        private void ThumbCanvas_PaintSurface
            (object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get canvas info
            _thumbCanvasInfo.Canvas = e.Surface.Canvas;
            _thumbCanvasInfo.Surface = e.Surface;
            _thumbCanvasInfo.ImageInfo = e.Info;

            e.Surface.Canvas.Clear(SKColors.Transparent);

            DrawThumb();
        }

        void TimelineCanvas_PaintSurface
            (object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get canvas info
            _timelineCanvasInfo.Canvas = e.Surface.Canvas;
            _timelineCanvasInfo.Surface = e.Surface;
            _timelineCanvasInfo.ImageInfo = e.Info;

            e.Surface.Canvas.Clear(SKColors.Transparent);

            Calculate(e);

            if (Period == null)
                return;

            DrawTimeline();

            thumbCanvas.PaintSurface += ThumbCanvas_PaintSurface;
            thumbTextCanvas.PaintSurface += ThumbTextCanvas_PaintSurface;
            thumbCanvas.InvalidateSurface();
            thumbTextCanvas.InvalidateSurface();
        }

        void ThumbTextCanvas_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Get canvas info
            _thumbTextCanvasInfo.Canvas = e.Surface.Canvas;
            _thumbTextCanvasInfo.Surface = e.Surface;
            _thumbTextCanvasInfo.ImageInfo = e.Info;

            e.Surface.Canvas.Clear(SKColors.Transparent);

            UpdateText();
        }

        void OnTouch(object sender, TouchActionEventArgs args)
        {
            // Convert touch point to pixel point
            _touch.Current = SkiaSharpHelper.ToPixel(args.Location.X, args.Location.Y, ref timelineCanvas);

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    _touch.Matrix = SKMatrix.MakeIdentity();

                    // Get innrer circle rect
                    _thumbDraw.Bounds = new SKRect
                        (_thumbDraw.Point.X - _thumbDraw.Radius, _thumbDraw.Point.Y - _thumbDraw.Radius
                        , _thumbDraw.Point.X + _thumbDraw.Radius, _thumbDraw.Point.Y + _thumbDraw.Radius);

                    _thumbDraw.Bounds = _touch.Matrix.MapRect(_thumbDraw.Bounds);

                    // Check if the inner circle was touched
                    if (_thumbDraw.Bounds.Contains(_touch.Current))
                    {
                        _touch.Id = args.Id;
                        _touch.Previous = _touch.Current;
                    }
                    else
                        TouchAction();

                    break;

                case TouchActionType.Moved:
                    if (_touch.Id == args.Id)
                    {
                        // Adjust the matrix for the new position
                        _touch.Matrix.TransX += _touch.Current.X - _touch.Previous.X;
                        _touch.Matrix.TransY += _touch.Current.Y - _touch.Previous.Y;
                        _touch.Previous = _touch.Current;

                        TouchAction();
                    }

                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    {
                        _touch.Id = -1;
                    }
                    break;
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Period
            if (propertyName == PeriodProperty.PropertyName)
            {
                // Get timeline type
                if ((Period.Last() - Period.First()).TotalDays > 1)
                    _type = TimelineType.Week;

                // Init canvases
                timelineCanvas.InvalidateSurface();

                Time = Period.Last();
            }

            // Thumb position
            if (propertyName == ThumbPositionProperty.PropertyName)
            {
                _touch.Current.X = ThumbPosition;

                TouchAction();
            }
        }

        #endregion
    }
}
