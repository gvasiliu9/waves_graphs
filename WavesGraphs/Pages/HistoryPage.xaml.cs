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

        int?[] _temperature =
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
