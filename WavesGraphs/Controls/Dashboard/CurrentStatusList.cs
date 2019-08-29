using System;
using System.Windows.Input;
using Xamarin.Forms;
using static WavesGraphs.Models.Dashboard.Enums;

namespace WavesGraphs.Controls
{
    public class CurrentStatusList : StackLayout
    {
        #region Commands

        public static readonly BindableProperty ShowPopupForCommandProperty = BindableProperty
            .Create(nameof(ShowPopupForCommand), typeof(ICommand), typeof(CurrentStatusList));

        public ICommand ShowPopupForCommand
        {
            get => (ICommand)GetValue(ShowPopupForCommandProperty);
            set => SetValue(ShowPopupForCommandProperty, value);
        }

        public static readonly BindableProperty ShowHistoryCommandProperty = BindableProperty
            .Create(nameof(ShowHistoryModalCommand), typeof(ICommand), typeof(CurrentStatusList));

        public ICommand ShowHistoryModalCommand
        {
            get => (ICommand)GetValue(ShowHistoryCommandProperty);
            set => SetValue(ShowHistoryCommandProperty, value);
        }

        #endregion

        #region Events

        public void ShowPopupFor(SensorType sensorType)
        {
            // Show popup for selected sensor
            if (ShowPopupForCommand != null && ShowPopupForCommand.CanExecute(sensorType))
                ShowPopupForCommand.Execute(sensorType);
        }

        public void ShowHistory()
        {
            // Show history command
            if (ShowHistoryModalCommand != null && ShowHistoryModalCommand.CanExecute(null))
                ShowHistoryModalCommand.Execute(null);
        }

        #endregion
    }
}
