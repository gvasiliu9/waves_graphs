using System;
using SkiaSharp;

namespace WavesGraphs.Controls.Models.Shared
{
    /// <summary>
    /// Layout text draw helper
    /// </summary>
    public struct TextDraw
    {
        public SKPaint Paint;

        public Margin Margin;

        public SKRect Bounds;
    }

    /// <summary>
    /// Element margins
    /// </summary>
    public struct Margin
    {
        public float Left;

        public float Top;

        public float Right;

        public float Bottom;
    }

    /// <summary>
    /// Info helper for canvas
    /// </summary>
    public struct CanvasInfo
    {
        public SKCanvas Canvas;

        public SKSurface Surface;

        public SKImageInfo ImageInfo;
    }

    /// <summary>
    /// Store size calculations, for responsive design
    /// </summary>
    public struct ElementSize
    {
        public float _2px;
        public float _3px;
        public float _5px;
        public float _8px;
        public float _10px;
        public float _13px;
        public float _15px;
        public float _20px;
        public float _25px;
        public float _30px;
        public float _35px;
        public float _40px;
        public float _50px;
    }
}
