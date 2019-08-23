using System;
using SkiaSharp;
using WavesGraphs.Controls.Models.Shared;

namespace WavesGraphs.Controls.Models.DashboardGraph
{
    /// <summary>
    /// Hours line draw helper
    /// </summary>
    public struct HoursDraw
    {
        public float Step;

        public float CircleRadius;

        public SKRect Bounds;

        public SKPaint Paint;

        public SKPaint MidnightIndicatorPaint;

        public int TotalHours;
    }

    /// <summary>
    /// Graph line draw helper
    /// </summary>
    public struct GraphDraw
    {
        public float Step;

        public SKRect Bounds;

        public SKPaint Paint;

        public float CornerRadius;

        public SKColor FirstGradientColor;

        public SKColor SecondGradientColor;
    }

    /// <summary>
    /// Current value indicator draw helper
    /// </summary>
    public struct CurrentValueIndicatorDraw
    {
        public SKPaint Paint;

        public float Radius;
    }

    /// <summary>
    /// Max value line draw helper
    /// </summary>
    public struct TitleDraw
    {
        public SKRect Bounds;

        public SKPaint LinePaint;

        public float CenterY;
    }

    /// <summary>
    /// Max value dra helper
    /// </summary>
    public struct MaxValueDraw
    {
        public float Height;

        public SKRect Bounds;

        public SKPaint Paint;

        public float Radius;
    }

    /// <summary>
    /// Sensor label draw helper
    /// </summary>
    public class SensorLabelDraw
    {
        public SKRect Bounds;

        public SKColor ActiveColor;

        public SKColor NormalColor;

        public bool IsActive { get; set; }

        public DateTime DateTime { get; set; }

        public int Airflow { get; set; }

        public SKPoint Point;

        public SKPaint LinePaint { get; set; }

        public TextDraw IconTextDraw;

        public TextDraw HourTextDraw;

        public string Icon { get; set; }
    }

    /// <summary>
    /// Store text constant to display in control
    /// </summary>
    public struct TextConstants
    {
        public string Hours;

        public string Title;

        public string Icon;
    }

    /// <summary>
    /// Store size calculations, for responsive design
    /// </summary>
    public struct Size
    {
        public float _3px;
        public float _5px;
        public float _10px;
        public float _15px;
        public float _25px;
        public float _30px;
        public float _35px;
        public float _40px;
        public float _50px;
    }
}
