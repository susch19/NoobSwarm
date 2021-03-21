
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
    public class HSVColorWanderEffect : LightEffect
    {
        public Direction Direction { get; set; }
        public IReadOnlyList<Color> GradientColors { get; init; }

        private Bitmap? ledBitmap;
        private Rectangle bmpRect;
        int xMulti;
        int yMulti;


        public HSVColorWanderEffect()
        {
            GradientColors = new List<Color> { Color.Red, Color.FromArgb(0, 0xff, 0), Color.Blue };
        }
  
        public HSVColorWanderEffect(IReadOnlyList<Color> gradientColors) : this()
        {
            GradientColors = gradientColors;
        }


        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            try
            {
                ledBitmap = new Bitmap((ledKeyPoints.Max(x => x.X) + 1) * 3, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            }
            catch (Exception e)
            {

                throw;
            }

            bmpRect = new Rectangle(0, 0, ledBitmap.Width, ledBitmap.Height);

            var hsvGradient = new HSVGradientBrush(GradientColors);
            hsvGradient.Draw(ledBitmap);
            //ledBitmap.Save($"Wander_{string.Join('_', GradientColors)}.bmp");

            base.Init(ledKeyPoints);
        }

        public override bool InitNextFrame(int counter, long elapsedMilliseconds, short stepInrease, IReadOnlyList<(LedKey key, KeyChangeState state)> pressed)
        {
            if (LedKeyPoints is null || ledBitmap is null)
                return false;

            switch (Direction)
            {
                default:
                case Direction.Left:
                    xMulti = 1; yMulti = 0;
                    break;
                case Direction.Right:
                    xMulti = -1; yMulti = 0;
                    break;
                case Direction.Up:
                    xMulti = 0; yMulti = 1;
                    break;
                case Direction.Down:
                    xMulti = 0; yMulti = -1;
                    break;
            }
            return true;
        }

        public override Color? NextFrame(LedKey key, Color currentColor, int counter, long elapsedMilliseconds, short stepInrease)
        {
            var item = LedKeyPoints?.FirstOrNull(x => x.LedKey == key);
            if (item is null)
                return null;

            if (xMulti != 0)
            {
                var xPos = (((item.Value.X + (counter * xMulti)) % ledBitmap!.Width) + ledBitmap.Width) % ledBitmap.Width;
                return GetColorWithBrightness(ledBitmap.GetPixel(xPos, 0));

            }
            else if (yMulti != 0)
            {
                var yPos = (((item.Value.Y + (counter * yMulti)) % ledBitmap!.Width) + ledBitmap.Width) % ledBitmap.Width;
                return GetColorWithBrightness(ledBitmap.GetPixel(yPos, 0));
            }
            return null;
        }
    }
}
