using System;
using SkiaSharp;
using WavesGraphs.Controls.Models.Shared;

namespace WavesGraphs.Controls.Models.HistoryGraph
{
    public struct TitleDraw
    {
        public SKRect Bounds;

        public TextDraw IconTextDraw;

        public TextDraw GraphNameTextDraw;

        public TextDraw DescriptionTextDraw;

        public TextDraw LevelTextDraw;

        public TextDraw ValueTextDraw;
    }

    public struct GraphDraw
    {
        public SKRect Bounds;

        public SKPaint Paint;
    }
}
