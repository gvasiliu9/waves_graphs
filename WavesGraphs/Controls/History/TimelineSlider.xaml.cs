using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using SkiaSharp;
using TouchTracking;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class TimelineSlider : ContentView
    {
        #region Fields

        TimelineType _type;

        // Canvas info
        CanvasInfo _timelineCanvasInfo;
        CanvasInfo _thumbCanvasInfo;
        CanvasInfo _thumbTextCanvasInfo;

        // Timeline segments
        List<TimelineSegment> _timelineSegments;

        // Draw helpers
        TimeLineDraw _timeLineDraw;
        ThumbDraw _thumbDraw;

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
        }

        void TouchAction()
        {
            // Calculate percentage
            GetPercentage();

            GetTime();

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
            var leftBound = _touch.Current.X - _thumbDraw.Radius;
            var rightBound = _touch.Current.X + _thumbDraw.Radius;

            return rightBound <= _timeLineDraw.Bounds.Right && leftBound >= _timeLineDraw.Bounds.Left;
        }

        bool IsMaxRight()
            => _touch.Current.X == _timeLineDraw.Bounds.Right;

        void UpdateText()
        {
            if (IsMaxRight())
                DrawThumbText("Now");
            else if (_type == TimelineType.Week)
                DrawThumbText(Time.DayOfWeek.ToString().Substring(0, 3));

            if (IsMaxRight())
            {
                switch (_type)
                {
                    case TimelineType.Day: timeLabel.Text = "24H"; break;
                    case TimelineType.Week: timeLabel.Text = "7D"; break;
                }
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

        SKPoint ToPixel(float x, float y)
        {
            return new SKPoint((float)(thumbCanvas.CanvasSize.Width * x / thumbCanvas.Width)
                , (float)(thumbCanvas.CanvasSize.Height * y / thumbCanvas.Height));
        }

        void GetPercentage()
            => Value = (1 - Math.Round((_touch.Current.X / _timeLineDraw.Bounds.Right), 2));

        void Calculate(SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            float _3px = e.Info.Width * 0.004109589041096f;
            float _13px = e.Info.Width * 0.017808219178082f;
            float _15px = e.Info.Width * 0.020547945205479f;
            float _20px = e.Info.Width * 0.027397260273973f;

            // Timeline bounds
            _timeLineDraw.Bounds.Top = _15px;
            _timeLineDraw.Bounds.Left = 0;
            _timeLineDraw.Bounds.Bottom = e.Info.Height - _15px;
            _timeLineDraw.Bounds.Right = e.Info.Width * 0.8f;

            // Get timeline segments
            GetTimelineSegments();

            // Timeline
            _timeLineDraw.HourCircleRadius = _3px;
            _timeLineDraw.DayLineHeight = _13px;
            _timeLineDraw.DayLinePaint.StrokeWidth = _3px;

            // Thumb
            _thumbDraw.Radius = _timeLineDraw.Bounds.Height / 2;
            _thumbDraw.Point = new SKPoint(_timeLineDraw.Bounds.Right - _thumbDraw.Radius,
                _timeLineDraw.Bounds.MidY);

            _thumbDraw.TextDraw.Paint.TextSize = _20px;

            _touch.Current.X = _timeLineDraw.Bounds.Right;
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
            TimelineSegment timelineSegment = _timelineSegments
                .FirstOrDefault(s => _touch.Current.X >= s.Bounds.Left && _touch.Current.X <= s.Bounds.Right);

            if (timelineSegment == null)
                return;

            int index = (int)((timelineSegment.Hours.Count() - 1) *
                (1 - ((timelineSegment.Bounds.Right - _touch.Current.X) / timelineSegment.Bounds.Width)));

            Time = timelineSegment.Hours[index];

            // Update time lable text
            timeLabel.Text = Time.ToString("HH:mm");
        }

        void DrawTimeline()
        {
            switch (_type)
            {
                case TimelineType.Day: DrawDayTimeline(); break;
                case TimelineType.Week: DrawWeekTimeline(); break;
            }
        }

        #endregion

        #region Week

        void GetWeekTimelineSegments()
        {
            // Init
            _timelineSegments = new List<TimelineSegment>();

            int intervals;

            DateTime start;

            // Get timeline hours
            foreach (var hours in Period.GroupBy(p => new { p.Day, p.Month }).Select(group => group))
            {
                start = hours.First();

                // Calculate day interval
                intervals = (int)Math.Ceiling((double)Period.Where(p => p.Day == start.Day).Count() / 8);

                // Add timeline
                _timelineSegments.Add(new TimelineSegment
                {
                    Intervals = intervals,
                    Hours = hours.ToList(),
                });
            }

            // Calculate step
            int totalIntervals = _timelineSegments.Select(s => s.Intervals).Sum() + _timelineSegments.Count();

            _timeLineDraw.Step = (float)Math.Round(_timeLineDraw.Bounds.Right / totalIntervals);
        }

        void DrawWeekTimeline()
        {
            // Init point
            var point = new SKPoint();
            point.X = _timeLineDraw.Step;
            point.Y = _timeLineDraw.Bounds.MidY;

            // Draw segments
            foreach (var timelineSegment in _timelineSegments)
            {
                // Left bound
                timelineSegment.Bounds.Left = point.X;

                // Draw day line
                if (timelineSegment != _timelineSegments.First())
                {
                    DrawDayLine(point);
                    point.X += _timeLineDraw.Step;
                }

                // Draw hours
                for (int i = 0; i < timelineSegment.Intervals; i++)
                {
                    DrawHourCircle(point);
                    point.X += _timeLineDraw.Step;
                }

                // Right bound
                timelineSegment.Bounds.Right = point.X;
            }
        }

        #endregion

        #region Day

        void GetDayTimelineSegments()
        {
            // Init
            _timelineSegments = new List<TimelineSegment>();

            var timelineSegment = new TimelineSegment();

            timelineSegment.Bounds.Left = _timeLineDraw.Bounds.Left;
            timelineSegment.Bounds.Right = _timeLineDraw.Bounds.Right;

            // Get hours
            timelineSegment.Hours = Period;

            // Calculate intervals
            int totalIntervals = (int)Math.Round((Period.Last() - Period.First()).TotalHours);

            // Calculate step
            _timeLineDraw.Step = (float)Math.Round(_timeLineDraw.Bounds.Right / totalIntervals);

            // Add timeline segment
            _timelineSegments.Add(timelineSegment);
        }

        void DrawDayTimeline()
        {
            // Init point
            var point = new SKPoint();
            point.X = _timeLineDraw.Step;
            point.Y = _timeLineDraw.Bounds.MidY;

            // Draw hours
            foreach (DateTime hour in _timelineSegments.First().Hours)
            {
                if (hour.Minute != 0)
                    continue;

                if (hour.TimeOfDay.Ticks == 0)
                    DrawDayLine(point);
                else
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
            _touch.Current = ToPixel(args.Location.X, args.Location.Y);

            if (_touch.Current.X < _timeLineDraw.Bounds.Left
                || _touch.Current.X > _timeLineDraw.Bounds.Right)
                return;

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
                // Update time label text
                timeLabel.Text = Period.Last().ToString("HH:mm");

                // Get timeline type
                if ((Period.Last() - Period.First()).TotalDays > 1)
                    _type = TimelineType.Week;

                // Init canvases
                timelineCanvas.InvalidateSurface();
            }
        }

        #endregion
    }

    enum TimelineType
    {
        Day,
        Week
    }

    class TimelineSegment
    {
        public int Intervals;

        public List<DateTime> Hours;

        public SKRect Bounds;
    }

    struct TimeLineDraw
    {
        public float Step;

        public SKRect Bounds;

        public SKPaint HourCirclePaint;

        public SKPaint DayLinePaint;

        public float DayLineHeight;

        public float HourCircleRadius;
    }

    struct ThumbDraw
    {
        public SKRect Bounds;

        public SKPoint Point;

        public SKPaint Paint;

        public float Radius;

        public TextDraw TextDraw;
    }

    struct Touch
    {
        public long Id;

        public SKPoint Current;

        public SKPoint Previous;

        public SKMatrix Matrix;
    }
}
