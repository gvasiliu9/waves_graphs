using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using WavesGraphs.Controls.Models.Settings.RangeSlider;
using WavesGraphs.Controls.Models.Shared;
using WavesGraphs.Helpers;
using WavesGraphs.Helpers.Constants;
using WavesGraphs.Models.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WavesGraphs.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RangeSlider : ContentView
    {
        #region Fields

        private ActiveThumb _activeThumb;

        private ActiveThumb _lastActiveThumb;

        private float _lastPercentage;

        private string _defaultColor = "#000000";

        private string _value;

        // Canvas
        private CanvasInfo _thumbsCanvasInfo;
        private CanvasInfo _segmentsCanvasInfo;
        private CanvasInfo _valuesCanvasInfo;

        // Tumbs
        private ThumbDraw _leftThumbDraw;
        private ThumbDraw _rightThumbDraw;

        // Touch
        private Touch _touch;

        // Segments
        private SKPaint _valueSegmentPaint;
        private SKPaint _remainingValueSegmentPaint;

        #endregion

        #region Bindable Properties

        // Start
        public static readonly BindableProperty StartProperty = BindableProperty
            .Create(nameof(Start),
                typeof(object),
                typeof(RangeSlider),
                default(object));

        public object Start
        {
            get
            {
                return (object)GetValue(StartProperty);
            }
            set
            {
                SetValue(StartProperty, value);
            }
        }

        // End
        public static readonly BindableProperty EndProperty = BindableProperty
            .Create(nameof(End),
                typeof(object),
                typeof(RangeSlider),
                default(object));

        public object End
        {
            get
            {
                return (object)GetValue(EndProperty);
            }
            set
            {
                SetValue(EndProperty, value);
            }
        }

        // Thumbs
        public static readonly BindableProperty ThumbsProperty = BindableProperty
            .Create(nameof(Thumbs),
                typeof(RangeSliderThumbs),
                typeof(RangeSlider),
                default(RangeSliderThumbs));

        public RangeSliderThumbs Thumbs
        {
            get
            {
                return (RangeSliderThumbs)GetValue(ThumbsProperty);
            }
            set
            {
                SetValue(ThumbsProperty, value);
            }
        }

        // Segments
        public static readonly BindableProperty SegmentsProperty = BindableProperty
            .Create(nameof(Segments),
                typeof(RangeSliderSegments),
                typeof(RangeSlider),
                default(RangeSliderSegments));

        public RangeSliderSegments Segments
        {
            get
            {
                return (RangeSliderSegments)GetValue(SegmentsProperty);
            }
            set
            {
                SetValue(SegmentsProperty, value);
            }
        }

        // Values
        public static readonly BindableProperty ValuesProperty = BindableProperty
            .Create(nameof(Values),
                typeof(List<object>),
                typeof(RangeSlider),
                default(List<object>));

        public List<object> Values
        {
            get
            {
                return (List<object>)GetValue(ValuesProperty);
            }
            set
            {
                SetValue(ValuesProperty, value);
            }
        }

        #endregion

        public RangeSlider()
        {
            InitializeComponent();

            InitializePaints();
        }

        #region Methods

        private void InitializePaints()
        {
            // Left thumb
            _leftThumbDraw.Paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            _leftThumbDraw.ValueTextDraw.Paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
            };

            _leftThumbDraw.IconTextDraw.Paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Typeface = SkiaSharpHelper.LoadTtfFont(Fonts.Icons)
            };

            // Right thumb
            _rightThumbDraw.Paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true
            };

            _rightThumbDraw.ValueTextDraw.Paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true
            };

            _rightThumbDraw.IconTextDraw.Paint = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Typeface = SkiaSharpHelper.LoadTtfFont(Fonts.Icons)
            };

            // Thumbs shadow
            _leftThumbDraw.Paint.ImageFilter = _rightThumbDraw.Paint.ImageFilter = SKImageFilter.CreateDropShadow(
                   0,
                   0,
                   5,
                   5,
                   SKColor.Parse("#EFEFEF"),
                   SKDropShadowImageFilterShadowMode.DrawShadowAndForeground
                );

            // Value segment
            _valueSegmentPaint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
            };

            // Remaining value segment
            _remainingValueSegmentPaint = new SKPaint
            {
                Color = SKColors.Gray,
                IsAntialias = true,
            };
        }

        private bool HasValues()
            => (Values != null && Values.Any());

        private float GetXPercentage(RangeSliderThumbOptions thumbOptions)
        {
            // Check values
            if (!HasValues())
                return -1;

            // Calculate value index
            int index = Values.FindIndex(v => v == thumbOptions.Value);

            _lastPercentage = (float)index / (Values.Count - 1);

            return _lastPercentage;
        }

        private void Calculate(SKPaintSurfaceEventArgs e)
        {
            // Segments
            _valueSegmentPaint.StrokeWidth = _remainingValueSegmentPaint.StrokeWidth = 5;

            // Text
            _leftThumbDraw.ValueTextDraw.Margin.Bottom = 8;
            _leftThumbDraw.ValueTextDraw.Margin.Top = 5;

            _rightThumbDraw.ValueTextDraw.Margin.Bottom = 8;
            _rightThumbDraw.ValueTextDraw.Margin.Top = 5;
        }

        private object GetThumbValue(SKPoint point)
        {
            // Calculate percentage
            float percentage = (point.X / _segmentsCanvasInfo.ImageInfo.Width);

            // Calculate value index
            int index = Math.Abs((int)(percentage * (Values.Count() - 1)));

            // Check value index
            if (index >= Values.Count())
                return null;

            // Return value
            return Values[index];
        }

        private void CheckThumbBounds(ref ThumbDraw thumbDraw)
        {
            if ((thumbDraw.Point.X - thumbDraw.Radius) < _thumbsCanvasInfo.ImageInfo.Rect.Left)
                thumbDraw.Point.X = _thumbsCanvasInfo.ImageInfo.Rect.Left + thumbDraw.Radius;
            else if ((thumbDraw.Point.X + thumbDraw.Radius) > _thumbsCanvasInfo.ImageInfo.Rect.Right)
                thumbDraw.Point.X = _thumbsCanvasInfo.ImageInfo.Rect.Right - thumbDraw.Radius;
        }

        private void DrawSegments()
        {
            // Left thumb value segment
            float y = _thumbsCanvasInfo.ImageInfo.Rect.MidY;

            var leftThumbSegmentStart = new SKPoint
            {
                Y = y,
                X = _leftThumbDraw.Radius
            };

            var leftThumbSegmentEnd = new SKPoint
            {
                Y = y,
                X = _leftThumbDraw.Point.X
            };

            DrawSegment(leftThumbSegmentStart, leftThumbSegmentEnd, _valueSegmentPaint);

            // Right thumb value segment
            var rightThumbSegmentStart = new SKPoint
            {
                Y = y,
                X = _rightThumbDraw.Point.X
            };

            var rightThumbSegmentEnd = new SKPoint
            {
                Y = y,
                X = _thumbsCanvasInfo.ImageInfo.Rect.Right - _rightThumbDraw.Radius
            };

            DrawSegment(rightThumbSegmentStart, rightThumbSegmentEnd, _valueSegmentPaint);

            // Remaining value segment
            var remaningValueSegmentStart = new SKPoint
            {
                Y = y,
                X = _leftThumbDraw.Point.X
            };

            var remaningValueSegmentEnd = new SKPoint
            {
                Y = y,
                X = _rightThumbDraw.Point.X
            };

            DrawSegment(remaningValueSegmentStart, remaningValueSegmentEnd, _remainingValueSegmentPaint);
        }

        private void DrawSegment(SKPoint start, SKPoint end, SKPaint paint)
            => _segmentsCanvasInfo.Canvas.DrawLine(start, end, paint);

        private void DrawThumb(ref ThumbDraw thumbDraw, RangeSliderThumbOptions thumbOptions)
        {
            // Colors
            thumbDraw.Paint.Color = SKColor.Parse((thumbOptions.BackgroundColor ?? _defaultColor));

            thumbDraw.ValueTextDraw.Paint.Color = SKColor.Parse((thumbOptions.TextColor ?? _defaultColor));

            float percentage = GetXPercentage(thumbOptions);

            if (percentage == -1f)
                return;

            // Calculate thumb bounds
            thumbDraw.Radius = _thumbsCanvasInfo.ImageInfo.Rect.MidY
                           - (_thumbsCanvasInfo.ImageInfo.Rect.MidY * 0.5f);

            thumbDraw.Point = new SKPoint
            {
                X = percentage * _thumbsCanvasInfo.ImageInfo.Width,
                Y = _thumbsCanvasInfo.ImageInfo.Rect.MidY
            };

            CheckThumbBounds(ref thumbDraw);

            _thumbsCanvasInfo.Canvas.DrawCircle(thumbDraw.Point, thumbDraw.Radius, thumbDraw.Paint);
        }

        private void DrawThumbIcon(ref ThumbDraw thumbDraw, RangeSliderThumbOptions thumbOptions)
        {
            // Set size
            thumbDraw.IconTextDraw.Paint.TextSize = thumbDraw.Radius * 0.75f;

            // Measure
            thumbDraw.IconTextDraw.Paint.MeasureText(thumbOptions.Icon,
                ref thumbDraw.IconTextDraw.Bounds);

            // Update paint
            thumbDraw.IconTextDraw.Paint.Color = SKColor.Parse(thumbOptions.IconColor ?? _defaultColor);

            // Draw
            var point = new SKPoint
            {
                X = thumbDraw.Point.X - Math.Abs(thumbDraw.IconTextDraw.Bounds.MidX),
                Y = thumbDraw.Point.Y + Math.Abs(thumbDraw.IconTextDraw.Bounds.MidY)
            };

            _thumbsCanvasInfo.Canvas.DrawText(thumbOptions.Icon,
                point,
                thumbDraw.IconTextDraw.Paint);
        }

        private bool ThumbValuesHaveConflict()
        {
            return ((_rightThumbDraw.Point.X - Math.Abs(_rightThumbDraw.ValueTextDraw.Bounds.MidX)) <=
                (_leftThumbDraw.Point.X + Math.Abs(_leftThumbDraw.ValueTextDraw.Bounds.MidX))
                &&
                (_rightThumbDraw.Point.X + Math.Abs(_rightThumbDraw.ValueTextDraw.Bounds.MidX)) >=
                (_leftThumbDraw.Point.X - Math.Abs(_leftThumbDraw.ValueTextDraw.Bounds.MidX)));
        }

        private float GetThumbValueXPosition(ThumbDraw thumbDraw, TextDraw textDraw)
        {
            if (thumbDraw.Point.X - textDraw.Bounds.MidX < _valuesCanvasInfo.ImageInfo.Rect.Left)
                return _valuesCanvasInfo.ImageInfo.Rect.Left;
            else if (thumbDraw.Point.X + textDraw.Bounds.MidX > _valuesCanvasInfo.ImageInfo.Rect.Right)
                return _valuesCanvasInfo.ImageInfo.Rect.Right - textDraw.Bounds.Width;
            else
                return thumbDraw.Point.X - textDraw.Bounds.MidX;
        }

        private void DrawValues()
        {
            // Get left thumb value
            _leftThumbDraw.ValueTextDraw.Paint.TextSize = _leftThumbDraw.Radius * 0.5f;

            _leftThumbDraw.ValueTextDraw.Paint.MeasureText(Thumbs.Left.Value?.ToString(), ref _leftThumbDraw.ValueTextDraw.Bounds);

            // Get right thumb value
            _rightThumbDraw.ValueTextDraw.Paint.TextSize = _rightThumbDraw.Radius * 0.5f;

            _rightThumbDraw.ValueTextDraw.Paint.MeasureText(Thumbs.Right.Value?.ToString(), ref _rightThumbDraw.ValueTextDraw.Bounds);

            // If thumbs values have conflict
            if (ThumbValuesHaveConflict())
            {
                // Right thumb value
                _valuesCanvasInfo.Canvas.DrawText(Thumbs.Right.Value?.ToString(),
                    new SKPoint
                    {
                        X = GetThumbValueXPosition(_rightThumbDraw, _rightThumbDraw.ValueTextDraw),
                        Y = _rightThumbDraw.Point.Y - _rightThumbDraw.Radius
                            - _rightThumbDraw.ValueTextDraw.Margin.Bottom,
                    },
                    _rightThumbDraw.ValueTextDraw.Paint);

                // Left thumb value
                _valuesCanvasInfo.Canvas.DrawText(Thumbs.Left.Value?.ToString(),
                    new SKPoint
                    {
                        X = GetThumbValueXPosition(_leftThumbDraw, _leftThumbDraw.ValueTextDraw),
                        Y = _leftThumbDraw.Point.Y + _leftThumbDraw.Radius
                            + _leftThumbDraw.ValueTextDraw.Paint.TextSize
                            + _leftThumbDraw.ValueTextDraw.Margin.Top,
                    },
                    _leftThumbDraw.ValueTextDraw.Paint);
            }
            else
            {
                // Right thumb value
                _valuesCanvasInfo.Canvas.DrawText(Thumbs.Right.Value?.ToString(),
                    new SKPoint
                    {
                        X = GetThumbValueXPosition(_rightThumbDraw, _rightThumbDraw.ValueTextDraw),
                        Y = _rightThumbDraw.Point.Y + _rightThumbDraw.Radius
                            + _rightThumbDraw.ValueTextDraw.Paint.TextSize
                            + _rightThumbDraw.ValueTextDraw.Margin.Top,
                    },
                    _rightThumbDraw.ValueTextDraw.Paint);

                // Left thumb value
                _valuesCanvasInfo.Canvas.DrawText(Thumbs.Left.Value?.ToString(),
                    new SKPoint
                    {
                        X = GetThumbValueXPosition(_leftThumbDraw, _leftThumbDraw.ValueTextDraw),
                        Y = _leftThumbDraw.Point.Y + _leftThumbDraw.Radius
                            + _leftThumbDraw.ValueTextDraw.Paint.TextSize
                            + _leftThumbDraw.ValueTextDraw.Margin.Top,
                    },
                    _leftThumbDraw.ValueTextDraw.Paint);
            }
        }

        #endregion

        #region Events

        private void OnTouch
            (object sender, TouchTracking.TouchActionEventArgs args)
        {
            // Convert touch point to pixel point
            _touch.Current = SkiaSharpHelper.ToPixel(args.Location.X, args.Location.Y, ref thumbsCanvas);

            // Check bounds
            if (_touch.Current.X < _thumbsCanvasInfo.ImageInfo.Rect.Left
                || _touch.Current.X > _thumbsCanvasInfo.ImageInfo.Rect.Right)
                return;

            // Check touch type
            switch (args.Type)
            {
                case TouchActionType.Pressed:

                    _touch.Matrix = SKMatrix.MakeIdentity();

                    // Adjust thumbs bounds
                    _leftThumbDraw.Bounds = new SKRect
                        (_leftThumbDraw.Point.X - _leftThumbDraw.Radius, _leftThumbDraw.Point.Y - _leftThumbDraw.Radius
                        , _leftThumbDraw.Point.X + _leftThumbDraw.Radius, _leftThumbDraw.Point.Y + _leftThumbDraw.Radius);

                    _rightThumbDraw.Bounds = new SKRect
                        (_rightThumbDraw.Point.X - _rightThumbDraw.Radius, _rightThumbDraw.Point.Y - _rightThumbDraw.Radius
                        , _rightThumbDraw.Point.X + _rightThumbDraw.Radius, _rightThumbDraw.Point.Y + _rightThumbDraw.Radius);

                    // Check if the inner circle was touched
                    if (_leftThumbDraw.Bounds.Contains(_touch.Current)
                        && _rightThumbDraw.Bounds.Contains(_touch.Current))
                    {
                        _touch.Id = args.Id;
                        _touch.Previous = _touch.Current;

                        _activeThumb = _lastActiveThumb;
                    }
                    else if (_leftThumbDraw.Bounds.Contains(_touch.Current))
                    {
                        _touch.Id = args.Id;
                        _touch.Previous = _touch.Current;

                        _activeThumb = ActiveThumb.Left;
                    }
                    else if (_rightThumbDraw.Bounds.Contains(_touch.Current))
                    {
                        _touch.Id = args.Id;
                        _touch.Previous = _touch.Current;

                        _activeThumb = ActiveThumb.Right;
                    }

                    break;

                case TouchActionType.Moved:
                    if (_touch.Id == args.Id)
                    {
                        // Adjust the matrix for the new position
                        _touch.Matrix.TransX += _touch.Current.X - _touch.Previous.X;
                        _touch.Matrix.TransY += _touch.Current.Y - _touch.Previous.Y;
                        _touch.Previous = _touch.Current;

                        var value = GetThumbValue(_touch.Current);

                        // Check active thumb
                        if (_activeThumb == ActiveThumb.Left)
                        {
                            Thumbs.Left.Value = value;

                            Start = value;
                        }
                        else if (_activeThumb == ActiveThumb.Right)
                        {
                            Thumbs.Right.Value = value;

                            End = value;
                        }

                        // Update value
                        thumbsCanvas.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    {
                        _touch.Id = -1;

                        _lastActiveThumb = _activeThumb;
                    }
                    break;
            }
        }

        private void SegmentsCanvas_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // Get info
            _segmentsCanvasInfo.Canvas = e.Surface.Canvas;
            _segmentsCanvasInfo.Surface = e.Surface;
            _segmentsCanvasInfo.ImageInfo = e.Info;

            // Clear canvas
            e.Surface.Canvas.Clear(SKColors.Transparent);

            DrawSegments();
        }

        private void ValuesCanvas_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // Get info
            _valuesCanvasInfo.Canvas = e.Surface.Canvas;
            _valuesCanvasInfo.Surface = e.Surface;
            _valuesCanvasInfo.ImageInfo = e.Info;

            // Clear canvas
            e.Surface.Canvas.Clear(SKColors.Transparent);

            DrawValues();
        }

        private void ThumbsCanvas_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // Get info
            _thumbsCanvasInfo.Canvas = e.Surface.Canvas;
            _thumbsCanvasInfo.Surface = e.Surface;
            _thumbsCanvasInfo.ImageInfo = e.Info;

            // Clear canvas
            e.Surface.Canvas.Clear(SKColors.Transparent);

            Calculate(e);

            // Draw thumbs
            if (_activeThumb == ActiveThumb.Left)
            {
                // Rights
                DrawThumb(ref _rightThumbDraw, Thumbs.Right);
                DrawThumbIcon(ref _rightThumbDraw, Thumbs.Right);

                // Left
                DrawThumb(ref _leftThumbDraw, Thumbs.Left);
                DrawThumbIcon(ref _leftThumbDraw, Thumbs.Left);
            }
            else if (_activeThumb == ActiveThumb.Right)
            {
                // Left
                DrawThumb(ref _leftThumbDraw, Thumbs.Left);
                DrawThumbIcon(ref _leftThumbDraw, Thumbs.Left);

                // Right
                DrawThumb(ref _rightThumbDraw, Thumbs.Right);
                DrawThumbIcon(ref _rightThumbDraw, Thumbs.Right);
            }

            // Draw segments
            segmentsCanvas.InvalidateSurface();

            // Draw 
            valuesCanvas.InvalidateSurface();
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Values
            if (propertyName == ValuesProperty.PropertyName)
            {
                thumbsCanvas.InvalidateSurface();
            }

            // Thumbs
            if (propertyName == ThumbsProperty.PropertyName)
            {
                thumbsCanvas.InvalidateSurface();
            }

            // Segments
            if (propertyName == SegmentsProperty.PropertyName)
            {
                _valueSegmentPaint.Color = SKColor.Parse(Segments.ValueSegmentOptions.BackgorundColor);

                _remainingValueSegmentPaint.Color = SKColor.Parse(Segments.RemainingValueSegmentOptions.BackgorundColor);

                segmentsCanvas.InvalidateSurface();
            }
        }

        #endregion

        #region Types

        private enum ActiveThumb
        {
            Left,
            Right
        }

        #endregion
    }

}