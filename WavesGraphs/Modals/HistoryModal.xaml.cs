using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WavesGraphs.Controls;
using WavesGraphs.Models.History;
using Xamarin.Forms;

namespace WavesGraphs.Modals
{
    public partial class HistoryModal : ContentView
    {
        #region Bindable Properties

        public static readonly BindableProperty DayGraphValuesProperty = BindableProperty
            .Create(nameof(DayGraphValues), typeof(List<HistoryGraphValuesModel>), typeof(HistoryModal), default(List<HistoryGraphValuesModel>));

        public List<HistoryGraphValuesModel> DayGraphValues
        {
            get => (List<HistoryGraphValuesModel>)GetValue(DayGraphValuesProperty);

            set => SetValue(DayGraphValuesProperty, value);
        }

        public static readonly BindableProperty WeekGraphValuesProperty = BindableProperty
            .Create(nameof(WeekGraphValues), typeof(List<HistoryGraphValuesModel>), typeof(HistoryModal), default(List<HistoryGraphValuesModel>));

        public List<HistoryGraphValuesModel> WeekGraphValues
        {
            get => (List<HistoryGraphValuesModel>)GetValue(WeekGraphValuesProperty);

            set => SetValue(WeekGraphValuesProperty, value);
        }

        #endregion

        #region Commands

        public static readonly BindableProperty CloseCommandProperty = BindableProperty
            .Create(nameof(CloseCommand), typeof(ICommand), typeof(HistoryModal));

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public static readonly BindableProperty DayGraphCommandProperty = BindableProperty
            .Create(nameof(DayGraphCommand), typeof(ICommand), typeof(HistoryModal));

        public ICommand DayGraphCommand
        {
            get => (ICommand)GetValue(DayGraphCommandProperty);
            set => SetValue(DayGraphCommandProperty, value);
        }

        public static readonly BindableProperty WeekGraphCommandProperty = BindableProperty
            .Create(nameof(WeekGraphCommand), typeof(ICommand), typeof(HistoryModal));

        public ICommand WeekGraphCommand
        {
            get => (ICommand)GetValue(WeekGraphCommandProperty);
            set => SetValue(WeekGraphCommandProperty, value);
        }

        #endregion

        public HistoryModal()
        {
            InitializeComponent();

            InitializeCommands();
        }

        #region Methods

        void InitializeCommands()
        {
            // Close

            // Day graph

            // Week graph
        }

        #endregion

        #region Events

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Day graph values
            if (propertyName == DayGraphValuesProperty.PropertyName)
            {
                graphs.Children.Clear();
            }

            // Week graph values
            if (propertyName == WeekGraphValuesProperty.PropertyName)
            {
                // Clear graphs container
                graphs.Children.Clear();

                // Set timline slider period
                var period = WeekGraphValues.FirstOrDefault().Values.Select(v => v.DateTime).ToList();

                timelineSlider.Period = period;

                // Add graphs
                foreach (var weekGraphValue in WeekGraphValues)
                {
                    graphs.Children.Add(new HistoryGraph
                    {
                        Title = weekGraphValue.Title,
                        Icon = weekGraphValue.Icon,
                        Description = weekGraphValue.Description,
                        MeasurementUnit = weekGraphValue.MeasurementUnit,
                        ScaleIntervals = weekGraphValue.Scale,
                        Values = weekGraphValue.Values
                    });
                }
            }
        }

        #endregion
    }
}
