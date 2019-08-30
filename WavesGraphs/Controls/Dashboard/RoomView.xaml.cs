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
using WavesGraphs.Models.History;

namespace WavesGraphs.Controls
{
    public partial class RoomView : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty ActivateManualModeProperty = BindableProperty
            .Create(nameof(ActivateManualMode), typeof(bool), typeof(RoomView), default(bool));

        public bool ActivateManualMode
        {
            get => (bool)GetValue(ActivateManualModeProperty);

            set => SetValue(ActivateManualModeProperty, value);
        }

        public static readonly BindableProperty DashboardGraphValuesProperty = BindableProperty
            .Create(nameof(DashboardGraphValues), typeof(DashboardGraphValuesModel), typeof(RoomView), default(DashboardGraphValuesModel));

        public DashboardGraphValuesModel DashboardGraphValues
        {
            get => (DashboardGraphValuesModel)GetValue(DashboardGraphValuesProperty);

            set => SetValue(DashboardGraphValuesProperty, value);
        }

        public static readonly BindableProperty DayGraphValuesProperty = BindableProperty
            .Create(nameof(DayGraphValues), typeof(List<HistoryGraphValuesModel>), typeof(RoomView), default(List<HistoryGraphValuesModel>));

        public List<HistoryGraphValuesModel> DayGraphValues
        {
            get => (List<HistoryGraphValuesModel>)GetValue(DayGraphValuesProperty);

            set => SetValue(DayGraphValuesProperty, value);
        }

        public static readonly BindableProperty WeekGraphValuesProperty = BindableProperty
            .Create(nameof(WeekGraphValues), typeof(List<HistoryGraphValuesModel>), typeof(RoomView), default(List<HistoryGraphValuesModel>));

        public List<HistoryGraphValuesModel> WeekGraphValues
        {
            get => (List<HistoryGraphValuesModel>)GetValue(WeekGraphValuesProperty);

            set => SetValue(WeekGraphValuesProperty, value);
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
            await ShowModal(historyModal);
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
            currentStatus.ShowHistoryModalCommand = new Command(ShowHistoryModal);

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

            // Dashboard graph values
            if (propertyName == DashboardGraphValuesProperty.PropertyName)
            {
                dashboardGraph.Values = DashboardGraphValues;
            }

            // Day graph values
            if (propertyName == DayGraphValuesProperty.PropertyName)
            {
                historyModal.DayGraphValues = DayGraphValues;
            }

            // Day graph values
            if (propertyName == WeekGraphValuesProperty.PropertyName)
            {
                historyModal.WeekGraphValues = WeekGraphValues;
            }
        }

        #endregion
    }
}