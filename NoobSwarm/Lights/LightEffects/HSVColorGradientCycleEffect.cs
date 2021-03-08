using NoobSwarm.Brushes;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vulcan.NET;

namespace NoobSwarm.Lights.LightEffects
{
    public class HSVColorGradientCycleEffect : PerKeyLightEffect
    {
        public IReadOnlyList<Color> GradientColors { get; init; }

        private Bitmap? ledBitmap;
        private Rectangle bmpRect;
        public HSVColorGradientCycleEffect()
        {
            LedKeys = null;
            GradientColors = new List<Color> { Color.Red, Color.FromArgb(0,0xff,0), Color.Blue };
        }

        public HSVColorGradientCycleEffect(List<LedKey> keys) : this()
        {
            LedKeys = keys;
        }
        public HSVColorGradientCycleEffect(IReadOnlyList<Color> gradientColors) : this()
        {
            GradientColors = gradientColors;
        }
        public HSVColorGradientCycleEffect(List<LedKey> keys, IReadOnlyList<Color> gradientColors)
        {
            LedKeys = keys;
            GradientColors = gradientColors;
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            ledBitmap = new Bitmap(360, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bmpRect = new Rectangle(0, 0, ledBitmap.Width, ledBitmap.Height);

            var hsvGradient = new HSVGradientBrush(GradientColors);
            hsvGradient.Draw(ledBitmap);
            //ledBitmap.Save($"Cycle_{string.Join('_', GradientColors)}.bmp");
            base.Init(ledKeyPoints);
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            if (LedKeyPoints is not null && ledBitmap is not null)
            {
                var col = ledBitmap.GetPixel(((int)(counter * Speed) / stepInrease) % 360, 0);
                col = Color.FromArgb(col.A, (byte)(col.R * BrightnessPercent), (byte)(col.G * BrightnessPercent), (byte)(col.B * BrightnessPercent));
                if (LedKeys is null)
                    foreach (var item in LedKeyPoints)
                    {
                        if (currentColors.ContainsKey(item.LedKey))
                            currentColors[item.LedKey] = col;
                    }
                else
                    foreach (var item in LedKeyPoints)
                    {
                        if (LedKeys.Contains(item.LedKey) && currentColors.ContainsKey(item.LedKey))
                                currentColors[item.LedKey] = col;
                    }
            }
        }
    }
}
