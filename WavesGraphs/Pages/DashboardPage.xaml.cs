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
using WavesGraphs.Helpers;
using WavesGraphs.Models;
using WavesGraphs.Models.Dashboard;
using WavesGraphs.Models.History;
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

        #region Dashboard Graph

        private DashboardGraphValuesModel _dashboardGraphValues;

        private string[] _dashboardTimeAsString = {
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

        private int?[] _dashboardAirflow =
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

        #region History Graph

        #region Week

        string[] _weekStringDateTimes =
        {
                "2019-08-14T16:00:00Z",
                "2019-08-14T17:00:00Z",
                "2019-08-14T18:00:00Z",
                "2019-08-14T19:00:00Z",
                "2019-08-14T20:00:00Z",
                "2019-08-14T21:00:00Z",
                "2019-08-14T22:00:00Z",
                "2019-08-14T23:00:00Z",
                "2019-08-15T00:00:00Z",
                "2019-08-15T01:00:00Z",
                "2019-08-15T02:00:00Z",
                "2019-08-15T03:00:00Z",
                "2019-08-15T04:00:00Z",
                "2019-08-15T05:00:00Z",
                "2019-08-15T06:00:00Z",
                "2019-08-15T07:00:00Z",
                "2019-08-15T08:00:00Z",
                "2019-08-15T09:00:00Z",
                "2019-08-15T10:00:00Z",
                "2019-08-15T11:00:00Z",
                "2019-08-15T12:00:00Z",
                "2019-08-15T13:00:00Z",
                "2019-08-15T14:00:00Z",
                "2019-08-15T15:00:00Z",
                "2019-08-15T16:00:00Z",
                "2019-08-15T17:00:00Z",
                "2019-08-15T18:00:00Z",
                "2019-08-15T19:00:00Z",
                "2019-08-15T20:00:00Z",
                "2019-08-15T21:00:00Z",
                "2019-08-15T22:00:00Z",
                "2019-08-15T23:00:00Z",
                "2019-08-16T00:00:00Z",
                "2019-08-16T01:00:00Z",
                "2019-08-16T02:00:00Z",
                "2019-08-16T03:00:00Z",
                "2019-08-16T04:00:00Z",
                "2019-08-16T05:00:00Z",
                "2019-08-16T06:00:00Z",
                "2019-08-16T07:00:00Z",
                "2019-08-16T08:00:00Z",
                "2019-08-16T09:00:00Z",
                "2019-08-16T10:00:00Z",
                "2019-08-16T11:00:00Z",
                "2019-08-16T12:00:00Z",
                "2019-08-16T13:00:00Z",
                "2019-08-16T14:00:00Z",
                "2019-08-16T15:00:00Z",
                "2019-08-16T16:00:00Z",
                "2019-08-16T17:00:00Z",
                "2019-08-16T18:00:00Z",
                "2019-08-16T19:00:00Z",
                "2019-08-16T20:00:00Z",
                "2019-08-16T21:00:00Z",
                "2019-08-16T22:00:00Z",
                "2019-08-16T23:00:00Z",
                "2019-08-17T00:00:00Z",
                "2019-08-17T01:00:00Z",
                "2019-08-17T02:00:00Z",
                "2019-08-17T03:00:00Z",
                "2019-08-17T04:00:00Z",
                "2019-08-17T05:00:00Z",
                "2019-08-17T06:00:00Z",
                "2019-08-17T07:00:00Z",
                "2019-08-17T08:00:00Z",
                "2019-08-17T09:00:00Z",
                "2019-08-17T10:00:00Z",
                "2019-08-17T11:00:00Z",
                "2019-08-17T12:00:00Z",
                "2019-08-17T13:00:00Z",
                "2019-08-17T14:00:00Z",
                "2019-08-17T15:00:00Z",
                "2019-08-17T16:00:00Z",
                "2019-08-17T17:00:00Z",
                "2019-08-17T18:00:00Z",
                "2019-08-17T19:00:00Z",
                "2019-08-17T20:00:00Z",
                "2019-08-17T21:00:00Z",
                "2019-08-17T22:00:00Z",
                "2019-08-17T23:00:00Z",
                "2019-08-18T00:00:00Z",
                "2019-08-18T23:59:59Z",
                "2019-08-19T00:00:00Z",
                "2019-08-19T23:59:59Z",
                "2019-08-20T00:00:00Z",
                "2019-08-20T23:59:59Z",
                "2019-08-21T00:00:00Z"
            };

        List<DateTime> _weekPeriod;

        int?[] _weekTemperature =
        {
                 0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                10,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                20,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                25,
                30,
                30,
                35,
                35,
                0,
                0,
                10
        };

        #endregion

        #region Day

        #endregion

        #endregion

        #endregion

        public DashboardPage()
        {
            InitializeComponent();

            InitializeCommands();

            InitializeDashboardGraphValues();

            InitializeHistoryGraphs();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        #region Methods

        void InitializeDashboardGraphValues()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            _dashboardGraphValues = new DashboardGraphValuesModel();
            _dashboardGraphValues.Airflow = new List<GraphValueModel>();

            for (int i = 0; i < _dashboardTimeAsString.Length; i++)
            {
                _dashboardGraphValues.Airflow.Add(new GraphValueModel
                {
                    DateTime = DateTime.ParseExact(_dashboardTimeAsString[i], dateFormat, CultureInfo.InvariantCulture),
                    Value = _dashboardAirflow[i] ?? 0,
                });
            }

            // Check max airflow value, to determine graph scale
            int maxAirflow = _dashboardGraphValues.Airflow.Select(v => v.Value).Max();

            if (maxAirflow <= 100)
                _dashboardGraphValues.Scale = 100;
            else if (maxAirflow > 100 && maxAirflow <= 150)
                _dashboardGraphValues.Scale = 150;
            else
                _dashboardGraphValues.Scale = 200;

            // Events
            _dashboardGraphValues.Events = new List<GraphEventModel>();

            var random = new Random();

            _dashboardGraphValues.Events.AddRange(
                new List<GraphEventModel>
                {
                    new GraphEventModel
                    {
                        Icon = "CO2",
                        Time = _dashboardGraphValues.Airflow[random.Next(0, 41)].DateTime,
                    },
                    new GraphEventModel
                    {
                        Icon = "CO2",
                        Time = _dashboardGraphValues.Airflow[random.Next(0, 41)].DateTime,
                    },
                }
            );

            // Set values
            roomView.DashboardGraphValues = _dashboardGraphValues;
        }

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

        void InitializeHistoryGraphs()
        {
            var weekGraphValues = new List<HistoryGraphValuesModel>();
            int min;

            GetWeekPeriod();

            // Temperature
            var temperatureGraphValues = new HistoryGraphValuesModel
            {
                Title = "Temperature",
                Icon = IconsHelper.Co2Event,
                Description = "Indoor",
                MeasurementUnit = "°C",
                Scale = new List<ScaleIntervalModel>
                    {
                    new ScaleIntervalModel
                    {
                        From = 0,
                        To = 15,
                        Name = "Low",
                        Color = "#D7304B"
                    },
                    new ScaleIntervalModel
                    {
                        From = 15,
                        To = 20,
                        Name = "Medium",
                        Color = "#FAB269"
                    },
                    new ScaleIntervalModel
                    {
                        From = 20,
                        To = 25,
                        Name = "Good",
                        Color = "#3295C7"
                    },
                    new ScaleIntervalModel
                    {
                        From = 25,
                        To = 30,
                        Name = "Medium",
                        Color = "#FAB269"
                    },
                    new ScaleIntervalModel
                    {
                        From = 30,
                        To = 35,
                        Name = "High",
                        Color = "#D7304B"
                    }
                }
            };

            temperatureGraphValues.Values = new List<GraphValueModel>();

            min = temperatureGraphValues.Scale.First().From;

            for (int i = 0; i < _weekTemperature.Length; i++)
            {
                temperatureGraphValues.Values.Add(new GraphValueModel
                {
                    DateTime = _weekPeriod[i],
                    Value = _weekTemperature[i] ?? min
                });
            }

            weekGraphValues.Add(temperatureGraphValues);

            // Co2

            // H2o

            // Set values
            roomView.WeekGraphValues = weekGraphValues;
        }

        void GetWeekPeriod()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            _weekPeriod = new List<DateTime>();

            for (int i = 0; i < _weekStringDateTimes.Length; i++)
                _weekPeriod.Add(DateTime.ParseExact(_weekStringDateTimes[i], dateFormat, CultureInfo.InvariantCulture));
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
