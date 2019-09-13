using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavesGraphs.Helpers;
using WavesGraphs.Models.Settings;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WavesGraphs.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RangeSlider : ContentPage
    {
        public RangeSlider()
        {
            InitializeComponent();

            rangeSlider.PropertyChanged += RangeSlider_PropertyChanged;

            // Initialize values
            var values = new List<object>();

            var dateTime = new DateTime();

            for (int i = 0; i < 96; i++)
            {
                dateTime = dateTime.AddMinutes(15);

                values.Add(dateTime);
            }

            rangeSlider.Values = values;

            // Initialize thumbs
            var random = new Random();

            rangeSlider.Thumbs = new RangeSliderThumbs
            {
                Left = new RangeSliderThumbOptions
                {
                    BackgroundColor = "#FFFFFF",
                    TextColor = "#494948",
                    IconColor = "#494948",
                    Icon = IconsHelper.Eco,
                    Value = values[random.Next(0, 47)]
                },

                Right = new RangeSliderThumbOptions
                {
                    BackgroundColor = "#FFFFFF",
                    TextColor = "#494948",
                    IconColor = "#494948",
                    Icon = IconsHelper.Heart,
                    Value = values[random.Next(47, 95)]
                }
            };

            // Initialize segments
            rangeSlider.Segments = new RangeSliderSegments
            {
                ValueSegmentOptions = new RangeSliderSegmentOptions
                {
                    BackgorundColor = "#494948"
                },
                RemainingValueSegmentOptions = new RangeSliderSegmentOptions
                {
                    BackgorundColor = "#D8D8D8"
                },
            };
        }

        private void RangeSlider_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Start
            if (e.PropertyName == nameof(rangeSlider.Start))
            {
                start.Text = rangeSlider.Start?.ToString();
            }

            // End
            if (e.PropertyName == nameof(rangeSlider.End))
            {
                end.Text = rangeSlider.End?.ToString();
            }
        }
    }
}