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
    public class HSVColorGradientCycleEffect : LightEffect
    {
        public IReadOnlyList<Color> GradientColors { get; init; }

        private Bitmap? ledBitmap;
        private Rectangle bmpRect;
        private Color? nextFrameColor;
        public HSVColorGradientCycleEffect()
        {
            //LedKeys = null;
            GradientColors = new List<Color> { Color.Red, Color.FromArgb(0,0xff,0), Color.Blue };
        }

        public HSVColorGradientCycleEffect(IReadOnlyList<Color> gradientColors) : this()
        {
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


        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (LedKeyPoints is not null && ledBitmap is not null)
            {
                if (stepInrease == 0)
                    return true;

                nextFrameColor = GetColorWithBrightness(ledBitmap.GetPixel(Math.Abs((counter % 360)), 0));
                return true;
            }
            return false;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            return nextFrameColor;
        }
    }
}
