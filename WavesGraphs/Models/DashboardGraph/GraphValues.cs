using System;
using System.Collections.Generic;

namespace WavesGraphs.Models
{
    /// <summary>
    /// Store sensors values
    /// </summary>
    public class GraphValues
    {
        public int Scale { get; set; }

        public List<GraphAirflowModel> Airflow;

        public List<GraphEventModel> Events;
    }
}
