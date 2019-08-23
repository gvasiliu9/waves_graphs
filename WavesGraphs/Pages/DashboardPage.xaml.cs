using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using WavesGraphs.Models;
using Xamarin.Forms;

namespace WavesGraphs.Pages
{
    [DesignTimeVisible(false)]
    public partial class DashboardPage : ContentPage
    {
        #region Fields

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

        public DashboardPage()
        {
            InitializeComponent();

            UpdateGraphValues();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await dashboardGraph.FadeTo(1, 2500);
        }

        private void InitGraphValues()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            _graphValues = new GraphValues();
            _graphValues.Airflow = new List<GraphAirflowModel>();

            for (int i = 0; i < _timeAsString.Length; i++)
            {
                _graphValues.Airflow.Add(new GraphAirflowModel
                {
                    DateTime = DateTime.ParseExact(_timeAsString[i], dateFormat, CultureInfo.InvariantCulture),
                    Airflow = _airflow[i] ?? 0,
                });
            }

            // Check max airflow value, to determine graph scale
            int maxAirflow = _graphValues.Airflow.Select(v => v.Airflow).Max();

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
    }
}
