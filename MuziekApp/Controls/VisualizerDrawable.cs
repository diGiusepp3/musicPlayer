using System;
using Microsoft.Maui.Graphics;

namespace MuziekApp.Controls
{
    public class VisualizerDrawable : IDrawable
    {
        readonly Random _random = new Random();
        readonly float[] _barHeights;
        const int BarCount = 20;

        public VisualizerDrawable()
        {
            _barHeights = new float[BarCount];
        }

        /// <summary>
        /// Roep dit periodiek aan om nieuwe bar-hoogtes te genereren.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < BarCount; i++)
                _barHeights[i] = (float)_random.NextDouble();
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var w = dirtyRect.Width;
            var h = dirtyRect.Height;
            var barWidth = w / (BarCount * 2f - 1f);

            canvas.SaveState();
            canvas.Alpha = 0.4f; // half-transparant
            canvas.FillColor = Colors.Purple;

            for (int i = 0; i < BarCount; i++)
            {
                var barH = _barHeights[i] * h;
                var x = i * barWidth * 2;
                var y = h - barH;
                canvas.FillRectangle(x, y, barWidth, barH);
            }

            canvas.RestoreState();
        }
    }
}