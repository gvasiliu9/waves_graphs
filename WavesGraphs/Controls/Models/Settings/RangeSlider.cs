using SkiaSharp;
using WavesGraphs.Controls.Models.Shared;

namespace WavesGraphs.Controls.Models.Settings.RangeSlider
{
    public struct ThumbDraw
    {
        public SKRect Bounds;

        public SKPaint Paint;

        public TextDraw ValueTextDraw;

        public TextDraw IconTextDraw;

        public SKPoint Point;

        public float Radius;
    }
}