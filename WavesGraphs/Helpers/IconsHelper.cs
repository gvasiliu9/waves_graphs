using System;
namespace WavesGraphs.Helpers
{
    public class IconsHelper
    {
        public const string Check = "\uea02";
        public const string Close = "\uea03";
        public const string Historic = "\uea06";
        public const string Hamburger = "\uea08";
        public const string Bathroom = "\uea09";
        public const string Heart = "\uea0a";
        public const string Co2Event = "\uea0b";
        public const string VocEvent = "\uea0c";
        public const string BoostEvent = "\uea0d";
        public const string H20Event = "\uea0e";
        public const string Ventilator = "\uea0f";
        public const string Intense = "\uea10";
        public const string Eco = "\uea11";
        public const string Manual = "\uea13";
        public const string Up = "\uea14";
        public const string Info = "\uea15";

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
