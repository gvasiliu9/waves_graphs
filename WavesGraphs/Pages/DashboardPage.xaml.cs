using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using SkiaSharp;
using WavesGraphs.Controls;
using WavesGraphs.Models;
using WavesGraphs.Models.Dashboard;
using WavesGraphs.Models.Shared;
using WavesGraphs.Popups;
using Xamarin.Forms;
using static WavesGraphs.Models.Dashboard.Enums;

namespace WavesGraphs.Pages
{
    [DesignTimeVisible(false)]
    public partial class DashboardPage : ContentPage
    {
        #region Fields

        #endregion

        public DashboardPage()
        {
            InitializeComponent();

            InitializeCommands();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        #region Methods

        void InitializeCommands()
        {
            // Show manual mode popup
            roomView.ShowManualModeModalCommand = new Command<RoomView>(async (roomView) =>
            {
                await Navigation.PushPopupAsync(new ManualModeActivationPopup(roomView));
            });

            // Show sensor popup
            roomView.ShowPopupForCommand = new Command<SensorType>(async (sensorType) =>
            {
                switch (sensorType)
                {
                    case SensorType.Temperatue:
                        break;
                    case SensorType.H20:
                        break;
                    case SensorType.Voc:
                        break;
                    case SensorType.Co2:
                        break;
                    case SensorType.Airflow:
                        break;
                }
            });
        }

        #endregion

        #region Commands

        async void ShowManualModePopup()
        {

        }

        async void SetProfile()
        {

        }

        async void Boost()
        {

        }

        #endregion
    }
}
