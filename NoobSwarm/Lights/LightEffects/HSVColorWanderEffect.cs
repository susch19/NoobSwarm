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
    public class HSVColorWanderEffect : PerKeyLightEffect
    {
        public Direction Direction { get; set; }
        public IReadOnlyList<Color> GradientColors { get; init; }

        private Bitmap? ledBitmap;
        private Rectangle bmpRect;


        public HSVColorWanderEffect()
        {
            LedKeys = null;
            GradientColors = new List<Color> { Color.Red, Color.FromArgb(0, 0xff, 0), Color.Blue };
        }

        public HSVColorWanderEffect(List<LedKey> keys) : this()
        {
            LedKeys = keys;
        }
        public HSVColorWanderEffect(IReadOnlyList<Color> gradientColors) : this()
        {
            GradientColors = gradientColors;
        }
        public HSVColorWanderEffect(List<LedKey> keys, IReadOnlyList<Color> gradientColors)
        {
            LedKeys = keys;
            GradientColors = gradientColors;
        }

        public override void Init(IReadOnlyList<LedKeyPoint> ledKeyPoints)
        {
            ledBitmap = new Bitmap((ledKeyPoints.Max(x => x.X) + 1) * 3, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bmpRect = new Rectangle(0, 0, ledBitmap.Width, ledBitmap.Height);

            var hsvGradient = new HSVGradientBrush(GradientColors);
            hsvGradient.Draw(ledBitmap);
            //ledBitmap.Save($"Wander_{string.Join('_', GradientColors)}.bmp");

            base.Init(ledKeyPoints);
        }

        public override void Next(Dictionary<LedKey, Color> currentColors, int counter, long elapsedMilliseconds, ushort stepInrease, IReadOnlyList<LedKey> pressed)
        {
            if (LedKeyPoints is not null && ledBitmap is not null)
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

                if (LedKeys is null)
                {
                    foreach (var item in LedKeyPoints)
                    {
                        if (currentColors.ContainsKey(item.LedKey))
                            SetKeyColor(currentColors, (int)(counter * Speed), xMulti, yMulti, item);
                    }
                }
                else
                {
                    foreach (var item in LedKeyPoints)
                    {
                        if (!LedKeys.Contains(item.LedKey))
                            continue;

                        if (currentColors.ContainsKey(item.LedKey))
                            SetKeyColor(currentColors, (int)(counter * Speed), xMulti, yMulti, item);
                    }
                }
            }
        }

        private void SetKeyColor(Dictionary<LedKey, Color> currentColors, int counter, int xMulti, int yMulti, LedKeyPoint item)
        {

            if (xMulti != 0)
            {
                var xPos = (((item.X + (counter * xMulti)) % ledBitmap!.Width) + ledBitmap.Width) % ledBitmap.Width;
                var col = ledBitmap.GetPixel(xPos, 0);
                currentColors[item.LedKey] = Color.FromArgb(col.A, (byte)(col.R * BrightnessPercent), (byte)(col.G * BrightnessPercent), (byte)(col.B * BrightnessPercent));
            }
            else if (yMulti != 0)
            {
                var yPos = (((item.Y + (counter * yMulti)) % ledBitmap!.Width) + ledBitmap.Width) % ledBitmap.Width;
                var col = ledBitmap.GetPixel(yPos, 0);
                currentColors[item.LedKey] = Color.FromArgb(col.A, (byte)(col.R * BrightnessPercent), (byte)(col.G * BrightnessPercent), (byte)(col.B * BrightnessPercent));
            }
        }
    }
}
