using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WavesGraphs.Models.Dashboard;
using WavesGraphs.Models.Shared;
using Rg.Plugins.Popup.Extensions;
using WavesGraphs.Pages;
using Xamarin.Forms;
using WavesGraphs.Popups;
using WavesGraphs.Modals;
using static WavesGraphs.Models.Dashboard.Enums;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace WavesGraphs.Controls
{
    public partial class RoomView : ContentView
    {
        #region Fields

        Rectangle _modalBounds;

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

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty ActivateManualModeProperty = BindableProperty
            .Create(nameof(ActivateManualMode), typeof(bool), typeof(RoomView), default(bool));

        public bool ActivateManualMode
        {
            get => (bool)GetValue(ActivateManualModeProperty);

            set => SetValue(ActivateManualModeProperty, value);
        }

        #endregion

        #region Commands

        public static readonly BindableProperty ShowPopupForCommandProperty = BindableProperty
            .Create(nameof(ShowPopupForCommand), typeof(ICommand), typeof(RoomView));

        public ICommand ShowPopupForCommand
        {
            get => (ICommand)GetValue(ShowPopupForCommandProperty);
            set => SetValue(ShowPopupForCommandProperty, value);
        }

        public static readonly BindableProperty ShowManualModeModalCommandProperty = BindableProperty
            .Create(nameof(ShowManualModeModalCommand), typeof(ICommand), typeof(RoomView));

        public ICommand ShowManualModeModalCommand
        {
            get => (ICommand)GetValue(ShowManualModeModalCommandProperty);
            set => SetValue(ShowManualModeModalCommandProperty, value);
        }

        public static readonly BindableProperty SetProfileCommandProperty = BindableProperty
            .Create(nameof(SetProfileCommand), typeof(ICommand), typeof(RoomView));

        public ICommand SetProfileCommand
        {
            get => (ICommand)GetValue(SetProfileCommandProperty);
            set => SetValue(SetProfileCommandProperty, value);
        }

        public static readonly BindableProperty BoostCommandProperty = BindableProperty
            .Create(nameof(BoostCommand), typeof(ICommand), typeof(RoomView));

        public ICommand BoostCommand
        {
            get => (ICommand)GetValue(BoostCommandProperty);
            set => SetValue(BoostCommandProperty, value);
        }

        #endregion

        public RoomView()
        {
            InitializeComponent();

            UpdateGraphValues();

            InitializeCommands();

            AddPagePaddingTop();

            message.Text = "Your device has been linked for less than 24h, so you're not able to see all the data";
        }

        #region Methods

        #region Modals

        async void ShowProfileModal()
        {
            var setProfileModal = new SetProfileModal();

            // Set profile command
            setProfileModal.SetProfileCommand = new Command(async (profile) =>
            {
                // Set profile command
                if (SetProfileCommand != null && SetProfileCommand.CanExecute(profile))
                    SetProfileCommand.Execute(profile);


            });

            // Close command
            //setProfileModal.CloseCommand = new Command(async () => await HideModal());

            // Get current profile
        }

        async void ShowManualModeModal()
        {
            await ShowModal(manualModeModal);

            // Close command
            manualModeModal.CloseCommand = new Command(async () =>
            {
                await HideModal(manualModeModal);
            });
        }

        async void ShowHistoryModal()
        {

        }

        #endregion

        #region Shared

        void InitializeCommands()
        {
            #region Current status

            // Expand current status
            currentStatus.ExpandCommand = new Command(async () =>
            {
                await dashboardGraph.FadeTo(0, 500);

                dashboardGraph.IsVisible = false;

                await currentStatus.Expand();
            });

            // Close current status
            currentStatus.CloseCommand = new Command(async () =>
            {
                await currentStatus.Close();

                dashboardGraph.IsVisible = true;

                await dashboardGraph.FadeTo(1, 500);
            });

            #endregion

            #region Modals

            // Profile
            roomTitle.ShowProfileModalCommand = new Command(ShowProfileModal);

            // History
            currentStatus.ShowHistoryModalCommand = new Command(async () =>
            {
                // Show history modal
            });

            #endregion

            #region Popups

            // Manual mode
            currentStatus.ShowManualModeModalCommand = new Command(async () =>
            {
                ActivateManualMode = false;

                if (ShowManualModeModalCommand != null && ShowManualModeModalCommand.CanExecute(this))
                    ShowManualModeModalCommand.Execute(this);
            });

            #endregion
        }

        async Task ShowModal(ContentView modal)
        {
            modal.IsVisible = true;

            // Get bounds
            Rectangle modalBounds = modal.Bounds;
            Rectangle layoutBounds = layout.Bounds;
            Rectangle roomTitleBounds = roomTitle.Bounds;

            // Calculate new bounds
            double margin = roomTitleBounds.Height * 0.75;

            modalBounds.Y = layout.Bounds.Height;

            await modal.LayoutTo(modalBounds, 5);

            modalBounds.Y = roomTitleBounds.Height + margin;
            modalBounds.Height -= (roomTitleBounds.Height + margin);

            // Animate
            var animations = new List<Task>();

            animations.Add(modal.LayoutTo(modalBounds));
            animations.Add(modal.FadeTo(1));

            await Task.WhenAll(animations);
        }

        async Task HideModal(ContentView modal)
        {
            Rectangle layoutBounds = layout.Bounds;

            await modal.FadeTo(0);

            modal.IsVisible = false;
        }

        void AddPagePaddingTop()
        {
            double paddingTop = 0;

            if (Device.RuntimePlatform == Device.Android)
                paddingTop = App.StatusBarHeight;
            else
                paddingTop = 10;

            Padding = new Thickness(0, paddingTop, 0, 0);
        }

        #endregion

        #endregion

        #region _24HGraph

        private void InitGraphValues()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            _graphValues = new GraphValues();
            _graphValues.Airflow = new List<GraphValueModel>();

            for (int i = 0; i < _timeAsString.Length; i++)
            {
                _graphValues.Airflow.Add(new GraphValueModel
                {
                    DateTime = DateTime.ParseExact(_timeAsString[i], dateFormat, CultureInfo.InvariantCulture),
                    Value = _airflow[i] ?? 0,
                });
            }

            // Check max airflow value, to determine graph scale
            int maxAirflow = _graphValues.Airflow.Select(v => v.Value).Max();

            if (maxAirflow <= 100)
                _graphValues.Scale = 100;
            else if (maxAirflow > 100 && maxAirflow <= 150)
                _graphValues.Scale = 150;
            else
                _graphValues.Scale = 200;

            // Events
            _graphValues.Events = new List<GraphEventModel>();

            var random = new Random();

            _graphValues.Events.AddRange(
                new List<GraphEventModel>
                {
                    new GraphEventModel
                    {
                        Icon = "CO2",
                        Time = _graphValues.Airflow[random.Next(0, 41)].DateTime,
                    },
                    new GraphEventModel
                    {
                        Icon = "CO2",
                        Time = _graphValues.Airflow[random.Next(0, 41)].DateTime,
                    },
                }
            );
        }

        private void UpdateGraphValues()
        {
            InitGraphValues();

            dashboardGraph.Values = _graphValues;
        }

        void OnGraphTapped(object sender, EventArgs args)
            => UpdateGraphValues();

        #endregion

        #region Events

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Active manual mode
            if (propertyName == ActivateManualModeProperty.PropertyName)
            {
                if (ActivateManualMode)
                    ShowManualModeModal();
            }
        }

        #endregion
    }
}