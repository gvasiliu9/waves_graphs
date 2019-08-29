using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace WavesGraphs.Controls
{
    public class FanBoostBarLabel : Label
    {
        public static readonly BindableProperty ValueProperty = BindableProperty
            .Create(nameof(Value), typeof(double), typeof(FanBoostBarLabel), default(double));

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            // Fan boost bar value
            if (propertyName == ValueProperty.PropertyName)
            {
                //
            }
        }
    }
}
