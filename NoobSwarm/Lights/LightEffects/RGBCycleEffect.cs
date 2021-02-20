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
    public class RGBCycleEffect : PerKeyLightEffect
    {
        private Bitmap? ledBitmap;
        private Rectangle bmpRect;
        private IReadOnlyList<LedKeyPoint>? ledKeyPoints;

        public RGBCycleEffect()
        {
            LedKeys = null;
        }

        public RGBCycleEffect(List<LedKey> keys)
        {
            LedKeys = keys;
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            ledBitmap = new Bitmap(360, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bmpRect = new Rectangle(0, 0, ledBitmap.Width, ledBitmap.Height);

            var hsvGradient = new HSVGradientBrush(new[] { Color.Red, Color.Green, Color.Blue });
            hsvGradient.Draw(ledBitmap);
            //ledBitmap.Save("Cycle.bmp");
            this.ledKeyPoints = ledKeyPoints;
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            if (ledKeyPoints is not null && ledBitmap is not null)
            {
                var col = ledBitmap.GetPixel(((int)(counter * Speed) / stepInrease) % 360, 0);
                if (LedKeys is null)
                    foreach (var item in ledKeyPoints)
                    {
                        if (currentColors.ContainsKey(item.LedKey))
                            currentColors[item.LedKey] = col;
                    }
                else
                    foreach (var item in ledKeyPoints)
                    {
                        if (LedKeys.Contains(item.LedKey) && currentColors.ContainsKey(item.LedKey))
                                currentColors[item.LedKey] = col;
                    }
            }
        }
    }
}
