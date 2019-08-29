using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WavesGraphs.Controls.Models.Shared;
using WavesGraphs.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WavesGraphs.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FanBoostBar : ContentView
    {
        #region Properties

        #region Bindable

        public static readonly BindableProperty MaxBoostProperty = BindableProperty
            .Create(nameof(MaxBoost), typeof(double), typeof(FanBoostBar), default(double));

        public double MaxBoost
        {
            get
            {
                return (double)GetValue(MaxBoostProperty);
            }
            set
            {
                SetValue(MaxBoostProperty, value);
            }
        }

        public static readonly BindableProperty MinBoostProperty = BindableProperty
            .Create(nameof(MinBoost), typeof(double), typeof(FanBoostBar), default(double));

        public double MinBoost
        {
            get
            {
                return (double)GetValue(MinBoostProperty);
            }
            set
            {
                SetValue(MinBoostProperty, value);
            }
        }

        public static readonly BindableProperty ValueProperty = BindableProperty
            .Create(nameof(Value), typeof(double), typeof(FanBoostBar), default(double));

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly BindableProperty CurrentValueProperty = BindableProperty
            .Create(nameof(CurrentValue), typeof(double), typeof(FanBoostBar), default(double));

        public double CurrentValue
        {
            get
            {
                return (double)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        public new static readonly BindableProperty ScaleProperty = BindableProperty
            .Create(nameof(Scale), typeof(int), typeof(FanBoostBar), default(int));

        public new int Scale
        {
            get
            {
                return (int)GetValue(ScaleProperty);
            }
            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        public static readonly BindableProperty ScaleLinesProperty = BindableProperty
            .Create(nameof(ScaleLines), typeof(int), typeof(FanBoostBar), default(int));

        public int ScaleLines
        {
            get
            {
                return (int)GetValue(ScaleLinesProperty);
            }
            set
            {
                SetValue(ScaleLinesProperty, value);
            }
        }

        public static readonly BindableProperty DragIconSourceProperty = BindableProperty
            .Create(nameof(DragIconSource), typeof(string), typeof(FanBoostBar), default(string));

        public string DragIconSource
        {
            get
            {
                return (string)GetValue(DragIconSourceProperty);
            }
            set
            {
                SetValue(DragIconSourceProperty, value);
            }
        }

        // Colors
        public static readonly BindableProperty PrimaryColorProperty = BindableProperty
            .Create(nameof(PrimaryColor), typeof(Color), typeof(FanBoostBar), default(Color));

        public Color PrimaryColor
        {
            get
            {
                return (Color)GetValue(PrimaryColorProperty);
            }

            set
            {
                SetValue(PrimaryColorProperty, value);
            }
        }

        #endregion

        // Shared fields
        private double _sliderPercentage;

        private float _y;

        // Bar & Bar container canvas fields
        private CanvasInfo _barContainerCanvasInfo;

        private int _barContainerPadding;

        private CanvasInfo _barCanvasInfo;

        private SKPaint _barContainerPaint;

        private SKPaint _barPaint;

        private SKPaint _dragLinePaint;

        private float _dragLineOffsetX;

        private float _dragLineOffsetY;

        private float _dragLineSpacing;

        // Font size
        private int _largeFontSize;

        private int _mediumFontSize;

        private int _smallFontSize;

        // Colors
        private string _lightColor = "#F6F6F6";

        private string _grayColor = "#D8D8D8";

        private string _darkColor = "#4B4B4A";

        #endregion

        public FanBoostBar()
        {
            InitializeComponent();

            // Bar container canvas
            _barContainerCanvasInfo = new CanvasInfo();

            _barContainerPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColor.Parse(_lightColor),
                IsAntialias = true,
            };

            // Bar canvas
            _barCanvasInfo = new CanvasInfo();

            _dragLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse("#FFFFFF"),
                StrokeWidth = 5,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true,
            };
        }

        #region Methods

        #region Events

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // MaxBoost
            if (propertyName == MaxBoostProperty.PropertyName)
            {
                slider.Maximum = MaxBoost;
            }
            // MinBoost

            if (propertyName == MinBoostProperty.PropertyName)
            {
                slider.Minimum = MinBoost;
            }

            // Primary color
            if (propertyName == PrimaryColorProperty.PropertyName)
            {
                _barPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColor.Parse(XamarinHelper.GetHexString(PrimaryColor)),
                    IsAntialias = true,
                };

                indicatorValueContainer.BackgroundColor = PrimaryColor;

                UpdateBoostRelaxColors();

            }

            // Value
            if (propertyName == CurrentValueProperty.PropertyName)
            {
                slider.Value = CurrentValue;

                UpdateCurrentIndicator();
                UpdateBoostRelaxColors();
            }

            // Scale
            if (propertyName == ScaleProperty.PropertyName)
            {
                //
            }

            // Scale lines
            if (propertyName == ScaleLinesProperty.PropertyName)
            {
                // Render scale lines
                if (ScaleLines > 0)
                {
                    for (int i = 0; i < ScaleLines; i++)
                    {
                        scale.Children.Add(new BoxView
                        {
                            HeightRequest = 2,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            BackgroundColor = Color.FromHex(_lightColor)
                        });
                    }
                }
            }

            // Is enabled
            if (propertyName == IsEnabledProperty.PropertyName)
            {
                if (!IsEnabled)
                    UpdateTextValuesColor();
            }
        }

        private void BarContainerCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Init
            _barContainerCanvasInfo.ImageInfo = e.Info;
            _barContainerCanvasInfo.Surface = e.Surface;
            _barContainerCanvasInfo.Canvas = e.Surface.Canvas;

            // Clear
            _barContainerCanvasInfo.Canvas.Clear(SKColors.Transparent);

            // Draw bar container
            _barContainerCanvasInfo.Canvas
                .DrawRoundRect(0, 0, _barContainerCanvasInfo.ImageInfo.Width
                , _barContainerCanvasInfo.ImageInfo.Height,
                _barContainerCanvasInfo.ImageInfo.Width, _barContainerCanvasInfo.ImageInfo.Width
                , _barContainerPaint);

            // Calc bar continaer padding
            _barContainerPadding = (int)(0.1 * _barContainerCanvasInfo.ImageInfo.Width);
        }

        private void BarCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Init
            _barCanvasInfo.ImageInfo = e.Info;
            _barCanvasInfo.Surface = e.Surface;
            _barCanvasInfo.Canvas = e.Surface.Canvas;

            // Calc Y axis percentage
            _y = (float)(_barCanvasInfo.ImageInfo.Height - (_sliderPercentage / 100) * _barCanvasInfo.ImageInfo.Height);

            // Clear
            _barCanvasInfo.Canvas.Clear(SKColors.Transparent);

            // Draw bar
            _barCanvasInfo.Canvas
                .DrawRoundRect(0 + _barContainerPadding, _y + _barContainerPadding
                , _barCanvasInfo.ImageInfo.Width - (_barContainerPadding * 2), Math.Abs(_barCanvasInfo.ImageInfo.Height - (_y + (_barContainerPadding * 2))),
                _barCanvasInfo.ImageInfo.Width, _barCanvasInfo.ImageInfo.Width
                , _barPaint);

            // Calc indicator margin top
            indicator.Margin = new Thickness
                (0, Math.Abs(indicatorContainer.Height - (indicatorContainer.Height * (_sliderPercentage / 100))));

            // Drag thumb lines
            if ((((_barCanvasInfo.ImageInfo.Height - _y) / _barCanvasInfo.ImageInfo.Height) * 100) > 6.899)
            {
                _dragLineOffsetX = (float)(0.32 * _barCanvasInfo.ImageInfo.Width);
                _dragLineOffsetY = (float)(0.35 * _barCanvasInfo.ImageInfo.Width);
                _dragLineSpacing = (float)(0.15 * _barCanvasInfo.ImageInfo.Width);

                _barCanvasInfo.Canvas
                    .DrawLine(_dragLineOffsetX, _y + _dragLineOffsetY, _barCanvasInfo.ImageInfo.Width - _dragLineOffsetX, _y + _dragLineOffsetY, _dragLinePaint);

                _barCanvasInfo.Canvas
                    .DrawLine(_dragLineOffsetX, _y + _dragLineOffsetY + _dragLineSpacing
                    , _barCanvasInfo.ImageInfo.Width - _dragLineOffsetX, _y + _dragLineOffsetY + _dragLineSpacing, _dragLinePaint);

                _dragLineSpacing += _dragLineSpacing;

                _barCanvasInfo.Canvas
                    .DrawLine(_dragLineOffsetX, _y + _dragLineOffsetY + _dragLineSpacing
                    , _barCanvasInfo.ImageInfo.Width - _dragLineOffsetX, _y + _dragLineOffsetY + _dragLineSpacing, _dragLinePaint);
            }
        }

        #endregion

        #endregion

        private void BarCanvasView_SizeChanged(object sender, EventArgs e)
        {
            // Calc width & height
            double width = barCanvasView.Width;
            double height = barCanvasView.Height;

            // Calc slider height
            slider.WidthRequest = height;
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            // Is Boost or Relax
            UpdateBoostRelaxColors();

            // Save current value
            Value = e.NewValue;

            // Calc slider percentage
            _sliderPercentage = (Value / MaxBoost) * 100;

            // Show curent slider value
            indicatorValue.Text = $"{Math.Round(slider.Value)}%";

            // Calc bar size
            barCanvasView.InvalidateSurface();
        }

        private Color SetTextColor(double value, double currentValue)
        {
            return value > currentValue ? Color.FromHex(_grayColor) : Color.FromHex(_darkColor);
        }

        private void CurrentCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            // Init
            SKImageInfo imageInfo = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = e.Surface.Canvas;

            // Dashed line paint
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColor.Parse(_grayColor),
                StrokeWidth = (float)(0.01 * imageInfo.Width),
                StrokeCap = SKStrokeCap.Round,
                PathEffect = SKPathEffect.CreateDash(new float[] { 0, (float)(0.03 * imageInfo.Width) }, 0)
            };

            // Dashed line path
            SKPath path = new SKPath();
            path.MoveTo(0, (imageInfo.Height / 2) + ((float)(0.01 * imageInfo.Width) / 2));
            path.LineTo(imageInfo.Width, (imageInfo.Height / 2) + ((float)(0.01 * imageInfo.Width) / 2));

            // Draw dashed line
            canvas.DrawPath(path, paint);
        }

        private void TextValues_SizeChanged(object sender, EventArgs e)
        {
            if (textValues.Children.Count > 0)
                return;

            // Init
            var maxValueFormatedString = new FormattedString();
            var minValueFormatedString = new FormattedString();

            var icon = "\uea02" + "   ";
            var iconFontFamily = "Icons.ttf#Icons";

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    iconFontFamily = "Icons.ttf#Icons";
                    break;
                case Device.iOS:
                    iconFontFamily = "Icons";
                    break;
            }

            _largeFontSize = (int)(0.2 * textValues.Width);
            _mediumFontSize = (int)(0.15 * textValues.Width);
            _smallFontSize = (int)(0.1 * textValues.Width);

            // Max value
            //
            // Icon
            var maxIconSpan = new Span
            {
                FontSize = _largeFontSize,
                Text = icon,
                FontFamily = iconFontFamily
            };

            // Text
            var maxValueSpan = new Span
            {
                FontSize = _mediumFontSize,
                Text = $"{MaxBoost}%"
            };

            // Format
            maxValueFormatedString.Spans.Add(maxIconSpan);
            maxValueFormatedString.Spans.Add(maxValueSpan);

            textValues.Children.Add(new FanBoostBarLabel
            {
                Value = MaxBoost,
                TextColor = SetTextColor(MaxBoost, Value),
                FormattedText = maxValueFormatedString,
            });

            // Scale
            double fontSize;

            if (Scale > 0)
            {
                // Calc step
                double step = MaxBoost / Scale;

                // Add scale values
                for (double i = MaxBoost - step; i >= step; i -= step)
                {
                    // Calc font size
                    fontSize = (i > MaxBoost / 2)
                        ? _largeFontSize : _mediumFontSize;

                    // Icon
                    var valueIconSpan = new Span
                    {
                        Text = icon,
                        FontFamily = iconFontFamily,
                        FontSize = fontSize
                    };

                    // Text
                    var scaleSpan = new Span
                    {
                        Text = $"{(int)i}%",
                        FontSize = _mediumFontSize
                    };

                    // Format
                    var scaleValueFormatedString = new FormattedString();

                    scaleValueFormatedString.Spans.Add(valueIconSpan);
                    scaleValueFormatedString.Spans.Add(scaleSpan);

                    textValues.Children.Add(new FanBoostBarLabel
                    {
                        Value = i,
                        TextColor = SetTextColor(i, Value),
                        FormattedText = scaleValueFormatedString,
                    });
                }
            }

            // Min value
            //
            // Icon
            var minIconSpan = new Span
            {
                FontSize = _mediumFontSize,
                Text = icon,
                FontFamily = iconFontFamily
            };

            // Text
            var minValueSpan = new Span
            {
                Text = $"{MinBoost}%",
                FontSize = _mediumFontSize
            };

            // Format
            minValueFormatedString.Spans.Add(minIconSpan);
            minValueFormatedString.Spans.Add(minValueSpan);

            textValues.Children.Add(new FanBoostBarLabel
            {
                Value = MinBoost,
                TextColor = SetTextColor(MinBoost, Value),
                FormattedText = minValueFormatedString
            });
        }

        private void Layout_SizeChanged(object sender, EventArgs e)
        {
            if (currentValueLayout.IsVisible)
                return;

            UpdateCurrentIndicator();

            currentValueLayout.IsVisible = true;
        }

        private void UpdateCurrentIndicator()
        {
            // Calc position Y
            var offset = (layout.Height * (_sliderPercentage / 100)) + (currentValueLayout.Height / 2);

            currentValueLayout.TranslateTo(0, layout.Height - offset, 300);

            // Calc font size
            double fontSize = currentValueLayout.Height * 0.19;

            currentLabel.FontSize = fontSize;

            boostLabelText.FontSize = fontSize;
            boostLabelIcon.FontSize = fontSize;

            relaxLabelText.FontSize = fontSize;
            relaxLabelIcon.FontSize = fontSize;
        }

        private void UpdateBoostRelaxColors()
        {
            if (Value > CurrentValue)
            {
                boostLabelText.TextColor = PrimaryColor;
                boostLabelIcon.TextColor = PrimaryColor;

                relaxLabelIcon.TextColor = Color.FromHex(_grayColor);
                relaxLabelText.TextColor = Color.FromHex(_grayColor);
            }
            else
            {
                boostLabelText.TextColor = Color.FromHex(_grayColor);
                boostLabelIcon.TextColor = Color.FromHex(_grayColor);

                relaxLabelIcon.TextColor = PrimaryColor;
                relaxLabelText.TextColor = PrimaryColor;
            }
        }

        private void UpdateTextValuesColor()
        {
            if (textValues.Children.Count == 0)
                return;

            foreach (View child in textValues.Children)
            {
                var textValue = child as FanBoostBarLabel;

                if (textValue == null)
                    continue;

                if (textValue.Value <= Value)
                    textValue.TextColor = Color.FromHex(_darkColor);
                else
                    textValue.TextColor = Color.FromHex(_grayColor);
            }
        }
    }
}