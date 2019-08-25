using System;
using SkiaSharp;

namespace WavesGraphs.Controls.Models.History.Scale
{
    public struct ScaleLinesDraw
    {
        public SKRect Bounds;

        public SKPaint DottedLinePaint;

        public SKPaint SeparatorLinePaint;
    }

    public struct ScaleColorsDraw
    {
        public SKRect Bounds;

        public SKPaint Paint;
    }

    public struct ScaleIntervalDraw
    {
        public SKPoint Start;

        public SKPoint End;
    }
}
