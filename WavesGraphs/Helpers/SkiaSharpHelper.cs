using System;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using WavesGraphs.Models.Shared;

namespace WavesGraphs.Helpers
{
    public static class SkiaSharpHelper
    {
        /// <summary>
        /// Load embeded font
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        public static SKTypeface LoadTtfFont(string fontName)
        {
            var assembly = typeof(SkiaSharpHelper).GetTypeInfo().Assembly;
            var file = $"WavesGraphs.Resources.Fonts.{fontName}.ttf";

            using (var resource = assembly.GetManifestResourceStream(file))
            {
                using (var memory = new MemoryStream())
                {
                    resource?.CopyTo(memory);

                    var bytes = memory?.ToArray();

                    using (var stream = new SKMemoryStream(bytes))
                    {
                        return SKTypeface.FromStream(stream);
                    }
                }
            }
        }

        public static float GetPercentage(float input, Range range)
        {
            float x = range.To - range.From;

            float result = (100 * (input - range.From)) / x;

            return result / 100;
        }

        public static SKPoint ToPixel(float x, float y, ref SKCanvasView canvasView)
        {
            return new SKPoint((float)Math.Round((canvasView.CanvasSize.Width * x / canvasView.Width))
                , (float)Math.Round((canvasView.CanvasSize.Height * y / canvasView.Height)));
        }
    }
}
