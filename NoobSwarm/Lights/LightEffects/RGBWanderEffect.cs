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
    public class RGBWanderEffect : LightEffect
    {
        private Bitmap ledBitmap;
        private Rectangle bmpRect;
        private IReadOnlyList<LedKeyPoint> ledKeyPoints;

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            ledBitmap = new Bitmap((ledKeyPoints.Max(x => x.X) + 1) * 3, (ledKeyPoints.Max(x => x.Y) + 1), System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bmpRect = new Rectangle(0, 0, ledBitmap.Width, ledBitmap.Height);

            var hsvGradient = new HSVGradientBrush(new[] { Color.Red, Color.FromArgb(100, 0, 255, 0), Color.Blue });
            hsvGradient.Draw(ledBitmap);
            this.ledKeyPoints = ledKeyPoints;
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds)
        {
            foreach (var item in ledKeyPoints)
            {
                var xPos = (((item.X + counter) % ledBitmap.Width) + ledBitmap.Width) % ledBitmap.Width;
                currentColors[item.LedKey] = ledBitmap.GetPixel(xPos, item.Y);
            }
        }
    }
}
