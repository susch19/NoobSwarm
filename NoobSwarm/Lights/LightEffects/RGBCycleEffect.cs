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
    public class RGBCycleEffect : LightEffect
    {
        private Bitmap? ledBitmap;
        private Rectangle bmpRect;
        private IReadOnlyList<LedKeyPoint>? ledKeyPoints;

        private List<LedKey>? ledKeys;
        public RGBCycleEffect()
        {
            ledKeys = null;
        }

        public RGBCycleEffect(List<LedKey> keys)
        {
            ledKeys = keys;
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            ledBitmap = new Bitmap(360, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bmpRect = new Rectangle(0, 0, ledBitmap.Width, ledBitmap.Height);

            var hsvGradient = new HSVGradientBrush(new[] { Color.Red, Color.Green, Color.Blue });
            hsvGradient.Draw(ledBitmap);
            ledBitmap.Save("Cycle.bmp");
            this.ledKeyPoints = ledKeyPoints;
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            if (ledKeyPoints is not null && ledBitmap is not null)
            {
                var col = ledBitmap.GetPixel((counter / 5) % 360, 0);
                if (ledKeys is null)
                    foreach (var item in ledKeyPoints)
                    {
                        currentColors[item.LedKey] = col;
                    }
                else
                    foreach (var item in ledKeyPoints)
                    {
                        if (ledKeys.Contains(item.LedKey))
                            currentColors[item.LedKey] = col;
                    }
            }
        }
    }
}
