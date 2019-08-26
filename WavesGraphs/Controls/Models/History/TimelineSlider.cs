using System;
using System.Collections.Generic;
using SkiaSharp;
using WavesGraphs.Controls.Models.Shared;

namespace WavesGraphs.Controls.Models.History.TimelineSlider
{
    public enum TimelineType
    {
        Day,
        Week
    }

    public class TimelineSegment
    {
        public List<DateTime> Hours;

        public SKRect Bounds;
    }

    public struct TimeLineDraw
    {
        public float Step;

        public SKRect Bounds;

        public SKPaint HourCirclePaint;

        public SKPaint DayLinePaint;

        public float DayLineHeight;

        public float HourCircleRadius;
    }

    public struct ThumbDraw
    {
        public SKRect Bounds;

        public SKPoint Point;

        public SKPaint Paint;

        public float Radius;

        public TextDraw TextDraw;
    }
}
