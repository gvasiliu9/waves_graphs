using System;
using System.Collections.Generic;
using System.Globalization;
using WavesGraphs.Models.History;
using Xamarin.Forms;

namespace WavesGraphs.Pages
{
    public partial class HistoryPage : ContentPage
    {
        #region Fields

        string[] _dateTimes =
            {
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
        "2019-08-21T00:00:00Z",
        "2019-08-21T23:59:59Z",
        "2019-08-22T00:00:00Z"
            };

        List<DateTime> _timelinePeriod;

        #endregion

        public HistoryPage()
        {
            InitializeComponent();

            InitTimelineSlider();

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
        }

        void InitTimelineSlider()
        {
            string dateFormat = "yyyy-MM-dd'T'HH:mm:ss'Z'";

            _timelinePeriod = new List<DateTime>();

            for (int i = 0; i < _dateTimes.Length; i++)
                _timelinePeriod.Add(DateTime.ParseExact(_dateTimes[i], dateFormat, CultureInfo.InvariantCulture));

            timelineSlider.Period = _timelinePeriod;

            timelineSlider.PropertyChanged += TimelineSlider_PropertyChanged;
        }

        private void TimelineSlider_PropertyChanged
            (object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Percentage
            if (e.PropertyName == nameof(timelineSlider.Value))
            {
                //percentage.Text = $"Precentage = {timelineSlider.Value}";
            }

            // Time
            if (e.PropertyName == nameof(timelineSlider.Time))
            {
                //time.Text = $"Time = {timelineSlider.Time.ToString()}";
            }
        }
    }
}
