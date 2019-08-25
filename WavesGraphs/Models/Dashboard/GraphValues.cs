using System;
using System.Collections.Generic;
using WavesGraphs.Models.Shared;

namespace WavesGraphs.Models.Dashboard
{
    /// <summary>
    /// Store sensors values
    /// </summary>
    public class GraphValues
    {
        public int Scale { get; set; }

        public List<GraphValueModel> Airflow;

        public List<GraphEventModel> Events;
    }
}
