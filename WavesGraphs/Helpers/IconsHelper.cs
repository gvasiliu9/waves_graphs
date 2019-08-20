using System;
namespace WavesGraphs.Helpers
{
    public class IconsHelper
    {
        public const string Ventilator = "\uea02";
        public const string H20Event = "\uea03";
        public const string BoostEvent = "\uea04";
        public const string VocEvent = "\uea05";
        public const string Co2Event = "\uea06";
        public const string TemperatureEvent = "\uea07";

        public static string GetIconForDashboardGraph(string iconName)
        {
            iconName = iconName.ToLowerInvariant();

            switch (iconName)
            {
                case "co2": return Co2Event;
                case "h2o": return H20Event;
                case "voc": return VocEvent;
                case "boost": return BoostEvent;
            }

            return "";
        }
    }
}
