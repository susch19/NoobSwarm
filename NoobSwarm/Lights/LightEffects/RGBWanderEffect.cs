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
        public Direction Direction { get; set; }

        private Bitmap? ledBitmap;
        private Rectangle bmpRect;
        private IReadOnlyList<LedKeyPoint>? ledKeyPoints;
        private List<LedKey>? ledKeys;


        public RGBWanderEffect()
        {
            ledKeys = null;
        }

        public RGBWanderEffect(List<LedKey> keys)
        {
            ledKeys = keys;
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            ledBitmap = new Bitmap((ledKeyPoints.Max(x => x.X) + 1) * 3, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bmpRect = new Rectangle(0, 0, ledBitmap.Width, ledBitmap.Height);

            var hsvGradient = new HSVGradientBrush(new[] { Color.Red, Color.Green, Color.Blue });
            hsvGradient.Draw(ledBitmap);
            //ledBitmap.Save("Test.bmp");
            this.ledKeyPoints = ledKeyPoints;
            Initialized = true;
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            if (ledKeyPoints is not null && ledBitmap is not null)
            {
                var xMulti = 1;
                var yMulti = 0;
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

                if (ledKeys is null)
                {
                    foreach (var item in ledKeyPoints)
                    {
                        SetKeyColor(currentColors, counter, xMulti, yMulti, item);
                    }
                }
                else
                {
                    foreach (var item in ledKeyPoints)
                    {
                        if (!ledKeys.Contains(item.LedKey))
                            continue;

                        SetKeyColor(currentColors, counter, xMulti, yMulti, item);
                    }
                }
            }
        }

        private void SetKeyColor(Dictionary<LedKey, Color> currentColors, int counter, int xMulti, int yMulti, LedKeyPoint item)
        {
            if (xMulti != 0)
            {
                var xPos = (((item.X + (counter * xMulti)) % ledBitmap!.Width) + ledBitmap.Width) % ledBitmap.Width;
                currentColors[item.LedKey] = ledBitmap.GetPixel(xPos, 0);
            }
            else if (yMulti != 0)
            {
                var yPos = (((item.Y + (counter * yMulti)) % ledBitmap!.Width) + ledBitmap.Width) % ledBitmap.Width;
                currentColors[item.LedKey] = ledBitmap.GetPixel(yPos, 0);
            }
        }
    }
}
