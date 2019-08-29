using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using static WavesGraphs.Models.Dashboard.Enums;

namespace WavesGraphs.Controls
{
    public partial class CurrentStatusItem : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty SensorTypeProperty = BindableProperty
            .Create(nameof(SensorType), typeof(SensorType), typeof(CurrentStatusItem), default(SensorType));

        public SensorType SensorType
        {
            get => (SensorType)GetValue(SensorTypeProperty);

            set => SetValue(SensorTypeProperty, value);
        }

        public static readonly BindableProperty IconProperty = BindableProperty
            .Create(nameof(Icon), typeof(string), typeof(CurrentStatusItem), default(string));

        public string Icon
        {
            get => (string)GetValue(IconProperty);

            set => SetValue(IconProperty, value);
        }

        public static readonly BindableProperty NameProperty = BindableProperty
            .Create(nameof(Name), typeof(string), typeof(CurrentStatusItem), default(string));

        public string Name
        {
            get => (string)GetValue(NameProperty);

            set => SetValue(NameProperty, value);
        }

        public static readonly BindableProperty DescriptionProperty = BindableProperty
            .Create(nameof(Description), typeof(string), typeof(CurrentStatusItem), default(string));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);

            set => SetValue(DescriptionProperty, value);
        }

        public static readonly BindableProperty LevelProperty = BindableProperty
            .Create(nameof(Level), typeof(string), typeof(CurrentStatusItem), default(string));

        public string Level
        {
            get => (string)GetValue(LevelProperty);

            set => SetValue(LevelProperty, value);
        }

        public static readonly BindableProperty ValueProperty = BindableProperty
            .Create(nameof(Value), typeof(string), typeof(CurrentStatusItem), default(string));

        public string Value
        {
            get => (string)GetValue(ValueProperty);

            set => SetValue(ValueProperty, value);
        }

        public static readonly BindableProperty ColorProperty = BindableProperty
            .Create(nameof(Color), typeof(string), typeof(CurrentStatusItem), default(string));

        public string Color
        {
            get => (string)GetValue(ColorProperty);

            set => SetValue(ColorProperty, value);
        }

        #endregion

        #region Commands


        #endregion

        public CurrentStatusItem()
        {
            InitializeComponent();

            InitializeCommands();
        }

        #region Methods

        void InitializeCommands()
        {
            // Icon tap
            icon.TapCommand = new Command(() =>
            {
                var parent = GetParent();

                parent.ShowPopupFor(SensorType);
            });

            // Name tap
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) =>
            {
                var parent = GetParent();

                parent.ShowHistory();
            };

            nameAndDescription.GestureRecognizers.Add(tapGestureRecognizer);
        }

        CurrentStatusList GetParent()
            => this.Parent as CurrentStatusList;

        #endregion

        #region Events

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Name
            if (propertyName == NameProperty.PropertyName)
            {
                name.Text = Name;
            }

            // Icon
            if (propertyName == IconProperty.PropertyName)
            {
                icon.Text = Icon;
            }

            // Description
            if (propertyName == DescriptionProperty.PropertyName)
            {
                description.Text = Description;
            }

            // Level
            if (propertyName == LevelProperty.PropertyName)
            {
                level.Text = Level;
            }

            // Value
            if (propertyName == ValueProperty.PropertyName)
            {
                value.Text = Value;
            }

            // Color
            if (propertyName == ColorProperty.PropertyName)
            {
                Color color = Xamarin.Forms.Color.FromHex(Color);

                icon.TextColor = color;
                level.TextColor = color;
                value.TextColor = color;
            }
        }

        #endregion
    }
}
