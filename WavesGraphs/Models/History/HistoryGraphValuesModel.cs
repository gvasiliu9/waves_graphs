using System;
using System.Collections.Generic;
using WavesGraphs.Models.Shared;

namespace WavesGraphs.Models.History
{
    public class HistoryGraphValuesModel
    {
        public string Title { get; set; }

        public string Icon { get; set; }

        public string MeasurementUnit { get; set; }

        public string Description { get; set; }

        public List<GraphValueModel> Values { get; set; }

        public List<ScaleIntervalModel> Scale { get; set; }
    }
}
