using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public partial class IconButton : ContentView
    {
        #region Bindable Properties
        public static readonly BindableProperty TextProperty = BindableProperty
            .Create(nameof(Text), typeof(string), typeof(IconButton), default(string));

        public string Text
        {
            get => (string)GetValue(TextProperty);

            set => SetValue(TextProperty, value);
        }


        public static readonly BindableProperty TextColorProperty = BindableProperty
            .Create(nameof(TextColor), typeof(Color), typeof(IconButton), default(Color));

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);

            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty
            .Create(nameof(FontSize), typeof(NamedSize), typeof(IconButton), default(NamedSize));

        public NamedSize FontSize
        {
            get => (NamedSize)GetValue(FontSizeProperty);

            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty ConstantFontSizeProperty = BindableProperty
            .Create(nameof(ConstantFontSize), typeof(double), typeof(IconButton), default(double));

        public double ConstantFontSize
        {
            get => (double)GetValue(ConstantFontSizeProperty);

            set => SetValue(ConstantFontSizeProperty, value);
        }

        #endregion

        #region Commands

        public static readonly BindableProperty TapCommandProperty = BindableProperty
            .Create(nameof(TapCommand), typeof(ICommand), typeof(IconButton));

        public ICommand TapCommand
        {
            get => (ICommand)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
        }

        #endregion

        public IconButton()
        {
            InitializeComponent();
        }


        #region Events

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Text
            if (propertyName == TextProperty.PropertyName)
            {
                label.Text = Text;
            }

            // Text color
            if (propertyName == TextColorProperty.PropertyName)
            {
                label.TextColor = TextColor;
            }

            // Named font size
            if (propertyName == FontSizeProperty.PropertyName)
            {
                label.FontSize = Device.GetNamedSize(FontSize, label);
            }

            // Constant font size
            if (propertyName == ConstantFontSizeProperty.PropertyName)
            {
                label.FontSize = ConstantFontSize;
            }
        }

        async void Label_Tapped(object sender, EventArgs args)
        {

            if (TapCommand != null && TapCommand.CanExecute(null))
            {
                await this.FadeTo(0.5);
                TapCommand.Execute(null);
            }

            await this.FadeTo(1);
        }

        #endregion
    }
}
