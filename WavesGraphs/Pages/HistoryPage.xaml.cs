using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WavesGraphs.Models.History;
using WavesGraphs.Models.Shared;
using Xamarin.Forms;

namespace WavesGraphs.Pages
{
    public partial class HistoryPage : ContentPage
    {
        #region Fields

        // JSON Data
        string[] _dateTimes =
            {
                "2019-08-12T12:15:00Z",
                "2019-08-12T12:30:00Z",
                "2019-08-12T12:45:00Z",
                "2019-08-12T13:00:00Z",
                "2019-08-12T13:15:00Z",
                "2019-08-12T13:30:00Z",
                "2019-08-12T13:45:00Z",
                "2019-08-12T14:00:00Z",
                "2019-08-12T14:15:00Z",
                "2019-08-12T14:30:00Z",
                "2019-08-12T14:45:00Z",
                "2019-08-12T15:00:00Z",
                "2019-08-12T15:15:00Z",
                "2019-08-12T15:30:00Z",
                "2019-08-12T15:45:00Z",
                "2019-08-12T16:00:00Z",
                "2019-08-12T16:15:00Z",
                "2019-08-12T16:30:00Z",
                "2019-08-12T16:45:00Z",
                "2019-08-12T17:00:00Z",
                "2019-08-12T17:15:00Z",
                "2019-08-12T17:30:00Z",
                "2019-08-12T17:45:00Z",
                "2019-08-12T18:00:00Z",
                "2019-08-12T18:15:00Z",
                "2019-08-12T18:30:00Z",
                "2019-08-12T18:45:00Z",
                "2019-08-12T19:00:00Z",
                "2019-08-12T19:15:00Z",
                "2019-08-12T19:30:00Z",
                "2019-08-12T19:45:00Z",
                "2019-08-12T20:00:00Z",
                "2019-08-12T20:15:00Z",
                "2019-08-12T20:30:00Z",
                "2019-08-12T20:45:00Z",
                "2019-08-12T21:00:00Z",
                "2019-08-12T21:15:00Z",
                "2019-08-12T21:30:00Z",
                "2019-08-12T21:45:00Z",
                "2019-08-12T22:00:00Z",
                "2019-08-12T22:15:00Z",
                "2019-08-12T22:30:00Z",
                "2019-08-12T22:45:00Z",
                "2019-08-12T23:00:00Z",
                "2019-08-12T23:15:00Z",
                "2019-08-12T23:30:00Z",
                "2019-08-12T23:45:00Z",
                "2019-08-13T00:00:00Z",
                "2019-08-13T00:15:00Z",
                "2019-08-13T00:30:00Z",
                "2019-08-13T00:45:00Z",
                "2019-08-13T01:00:00Z",
                "2019-08-13T01:15:00Z",
                "2019-08-13T01:30:00Z",
                "2019-08-13T01:45:00Z",
                "2019-08-13T02:00:00Z",
                "2019-08-13T02:15:00Z",
                "2019-08-13T02:30:00Z",
                "2019-08-13T02:45:00Z",
                "2019-08-13T03:00:00Z",
                "2019-08-13T03:15:00Z",
                "2019-08-13T03:30:00Z",
                "2019-08-13T03:45:00Z",
                "2019-08-13T04:00:00Z",
                "2019-08-13T04:15:00Z",
                "2019-08-13T04:30:00Z",
                "2019-08-13T04:45:00Z",
                "2019-08-13T05:00:00Z",
                "2019-08-13T05:15:00Z",
                "2019-08-13T05:30:00Z",
                "2019-08-13T05:45:00Z",
                "2019-08-13T06:00:00Z",
                "2019-08-13T06:15:00Z",
                "2019-08-13T06:30:00Z",
                "2019-08-13T06:45:00Z",
                "2019-08-13T07:00:00Z",
                "2019-08-13T07:15:00Z",
                "2019-08-13T07:30:00Z",
                "2019-08-13T07:45:00Z",
                "2019-08-13T08:00:00Z",
                "2019-08-13T08:15:00Z",
                "2019-08-13T08:30:00Z",
                "2019-08-13T08:45:00Z",
                "2019-08-13T09:00:00Z",
                "2019-08-13T09:15:00Z",
                "2019-08-13T09:30:00Z",
                "2019-08-13T09:45:00Z",
                "2019-08-13T10:00:00Z",
                "2019-08-13T10:15:00Z",
                "2019-08-13T10:30:00Z",
                "2019-08-13T10:45:00Z",
                "2019-08-13T11:00:00Z",
                "2019-08-13T11:15:00Z",
                "2019-08-13T11:30:00Z",
                "2019-08-13T11:45:00Z",
                "2019-08-13T12:00:00Z"
            };

        int?[] _temperature =
        {
            0,
            0,
            0,
            5,
            5,
            5,
            5,
            10,
            10,
            10,
            10,
            15,
            15,
            15,
            15,
            20,
            20,
            20,
            20,
            25,
            25,
            25,
            25,
            30,
            30,
            30,
            30,
            35,
            35,
            35,
            35,
            0,
            0,
            0,
            0,
            5,
            5,
            5,
            5,
            10,
            10,
            10,
            10,
            15,
            15,
            15,
            15,
            20,
            20,
            20,
            20,
            25,
            25,
            25,
            25,
            30,
            30,
            30,
            30,
            35,
            35,
            35,
            35,
            0,
            0,
            0,
            0,
            5,
            5,
            5,
            5,
            10,
            10,
            10,
            10,
            15,
            15,
            15,
            15,
            20,
            20,
            20,
            20,
            25,
            25,
            25,
            25,
            30,
            30,
            30,
            30,
            35,
            35,
            35,
            35,
            0
        };

        // Control data
        List<DateTime> _timelinePeriod;

        #endregion

        public HistoryPage()
        {
            InitializeComponent();

            InitTimelineSlider();

            InitTemperatureGraph();
        }

        void InitTemperatureGraph()
        {
            // Scale
            temperatureGraph.ScaleIntervals = new List<ScaleIntervalModel>
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
            };

            // Values
            var graphValues = new List<GraphValueModel>();

            int min = temperatureGraph.ScaleIntervals.First().From;

            for (int i = 0; i < _temperature.Length; i++)
            {
                graphValues.Add(new GraphValueModel
                {
                    DateTime = _timelinePeriod[i],
                    Value = _temperature[i] ?? min
                });
            }

            temperatureGraph.Values = graphValues;
        }

        void InitTimelineSlider()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            _timelinePeriod = new List<DateTime>();

            for (int i = 0; i < _dateTimes.Length; i++)
                _timelinePeriod.Add(DateTime.ParseExact(_dateTimes[i], dateFormat, CultureInfo.InvariantCulture));

            timelineSlider.Period = _timelinePeriod;
        }
    }
}
