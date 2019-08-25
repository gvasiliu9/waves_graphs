using System;
using SkiaSharp;
using WavesGraphs.Controls.Models.Shared;
using WavesGraphs.Models.Shared;

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
        public float Step;

        public SKRect Bounds;

        public SKPaint LinePaint;

        public SKPaint BackgroundPaint;
    }

    public struct GraphCircleDraw
    {
        public SKPaint CirclePaint;

        public SKPaint LinePaint;

        public float Radius;
    }

    public struct CurrentValueIndicatorDraw
    {
        public SKRect Bounds;

        public SKPaint ValueRectPaint;

        public SKPaint DashedLinePaint;

        public TextDraw ValueTextDraw;
    }
}
