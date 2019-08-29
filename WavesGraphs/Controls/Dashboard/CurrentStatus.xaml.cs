using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Rg.Plugins.Popup.Extensions;
using WavesGraphs.Popups;
using Xamarin.Forms;
using static WavesGraphs.Models.Dashboard.Enums;

namespace WavesGraphs.Controls
{
    public partial class CurrentStatus : ContentView
    {
        #region Fields

        double _sensorsListHeight;

        #endregion

        #region Bindable Porperties

        #endregion

        #region Commands

        public static readonly BindableProperty CloseCommandProperty = BindableProperty
            .Create(nameof(CloseCommand), typeof(ICommand), typeof(CurrentStatus));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly BindableProperty ExpandCommandProperty = BindableProperty
            .Create(nameof(ExpandCommand), typeof(ICommand), typeof(CurrentStatus));

        public ICommand ExpandCommand
        {
            get => (ICommand)GetValue(ExpandCommandProperty);
            set => SetValue(ExpandCommandProperty, value);
        }

        public static readonly BindableProperty ShowManualModeModalCommandProperty = BindableProperty
            .Create(nameof(ShowManualModeModalCommand), typeof(ICommand), typeof(CurrentStatus));

        public ICommand ShowManualModeModalCommand
        {
            get => (ICommand)GetValue(ShowManualModeModalCommandProperty);
            set => SetValue(ShowManualModeModalCommandProperty, value);
        }

        public static readonly BindableProperty ShowHistoryModalProperty = BindableProperty
            .Create(nameof(ShowHistoryModalCommand), typeof(ICommand), typeof(CurrentStatus));

        public ICommand ShowHistoryModalCommand
        {
            get => (ICommand)GetValue(ShowHistoryModalProperty);
            set => SetValue(ShowHistoryModalProperty, value);
        }

        public static readonly BindableProperty ShowPopupForCommandProperty = BindableProperty
            .Create(nameof(ShowPopupForCommand), typeof(ICommand), typeof(CurrentStatusList));

        public ICommand ShowPopupForCommand
        {
            get => (ICommand)GetValue(ShowPopupForCommandProperty);
            set => SetValue(ShowPopupForCommandProperty, value);
        }

        #endregion

        public CurrentStatus()
        {
            InitializeComponent();

            InitCommands();

            sensorsList.PropertyChanged += sensorsList_PropertyChanged;
        }

        void InitCommands()
        {
            #region Header

            // Close
            closeIcon.TapCommand = new Command(async () =>
            {
                if (CloseCommand != null && CloseCommand.CanExecute(null))
                    CloseCommand.Execute(null);
            });

            // Expand
            expandIcon.TapCommand = new Command(async () =>
            {
                if (ExpandCommand != null && ExpandCommand.CanExecute(null))
                    ExpandCommand.Execute(null);
            });

            // Manual mode
            manualIcon.TapCommand = new Command(async () =>
            {
                if (ShowManualModeModalCommand != null && ShowManualModeModalCommand.CanExecute(null))
                    ShowManualModeModalCommand.Execute(null);
            });

            // History
            historyIcon.TapCommand = new Command(() =>
            {
                if (ShowHistoryModalCommand != null && ShowHistoryModalCommand.CanExecute(null))
                    ShowHistoryModalCommand.Execute(null);
            });

            #endregion

            #region Sensors List

            // Show sensor modal
            sensorsList.ShowPopupForCommand = new Command<SensorType>(async (sensorType) =>
            {
                if (ShowPopupForCommand != null && ShowPopupForCommand.CanExecute(sensorType))
                    ShowPopupForCommand.Execute(sensorType);
            });

            // Show history modal
            sensorsList.ShowHistoryModalCommand = new Command(() =>
            {
                if (ShowHistoryModalCommand != null && ShowHistoryModalCommand.CanExecute(null))
                    ShowHistoryModalCommand.Execute(null);
            });

            #endregion
        }

        #region Methods

        public async Task Expand()
        {
            // Animate height
            var animation = new Animation(v => sensorsList.HeightRequest = v, 0, _sensorsListHeight);

            animation.Commit(this, "ExpandAnimation",
                16,
                250,
                Easing.Linear, (v, c) => sensorsList.HeightRequest = _sensorsListHeight, () => false);

            // Hide icon
            await expandIcon.FadeTo(0);
            expandIcon.IsVisible = false;

            closeIcon.IsVisible = true;
            await closeIcon.FadeTo(1);
        }

        public async Task Close()
        {
            // Animate height
            var animation = new Animation(v => sensorsList.HeightRequest = v, _sensorsListHeight, 0);

            animation.Commit(this, "CloseAnimation",
                16,
                250,
                Easing.Linear, (v, c) => sensorsList.HeightRequest = 0, () => false);

            // Hide icon
            await closeIcon.FadeTo(0);
            closeIcon.IsVisible = false;

            expandIcon.IsVisible = true;
            await expandIcon.FadeTo(1);
        }

        #endregion

        #region Events

        private async void sensorsList_PropertyChanged
            (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Height
            if (e.PropertyName == HeightProperty.PropertyName)
            {
                sensorsList.PropertyChanged -= sensorsList_PropertyChanged;

                _sensorsListHeight = sensorsList.Height;

                sensorsList.HeightRequest = 0;

                await container.FadeTo(1, 1500);

                sensorsList.Opacity = 1;
            }
        }

        #endregion
    }
}
