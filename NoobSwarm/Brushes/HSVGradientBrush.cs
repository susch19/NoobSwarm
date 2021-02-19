using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobSwarm.Brushes
{
    public class HSVGradientBrush
    {
        private HSV[] hsvColors;

        public HSVGradientBrush(Color[] colors)
        {
            if (colors.Length < 2)
                throw new ArgumentException();
            hsvColors = colors.Select(x => new HSV(x)).ToArray();
        }

        public void Draw(Bitmap bmp)
        {
            unsafe
            {
                var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var ptr = (byte*)bmpData.Scan0;

                var partSize = (bmp.Width + (bmp.Width % hsvColors.Length)) / hsvColors.Length;


                for (int i = 0; i < hsvColors.Length; i++)
                {
                    var localIP = (i + 1) % hsvColors.Length;

                    bool hueIncrease = hsvColors[i].Hue < hsvColors[localIP].Hue;
                    var toShiftHue = 0f;
                    if (!hueIncrease)
                    {
                        hueIncrease = 360 - hsvColors[i].Hue + hsvColors[localIP].Hue < 180;
                        if (hueIncrease)
                            toShiftHue = hsvColors[localIP].Hue + 360f - hsvColors[i].Hue;
                        else
                        {
                            toShiftHue = hsvColors[i].Hue - hsvColors[localIP].Hue;
                        }
                    }
                    else
                    {
                        toShiftHue = hsvColors[localIP].Hue - hsvColors[i].Hue;
                    }


                    bool satIncrease = false;
                    float toShiftSat = 0f;
                    if (hsvColors[i].Saturation != hsvColors[localIP].Saturation)
                    {
                        satIncrease = hsvColors[i].Saturation < hsvColors[localIP].Saturation;
                        toShiftSat = satIncrease ? hsvColors[localIP].Saturation - hsvColors[i].Saturation : hsvColors[i].Saturation - hsvColors[localIP].Saturation;
                    }

                    bool valIncrease = false;
                    float toShiftVal = 0f;
                    if (hsvColors[i].Value != hsvColors[localIP].Value)
                    {
                        valIncrease = hsvColors[i].Value < hsvColors[localIP].Value;
                        toShiftVal = valIncrease ? hsvColors[localIP].Value - hsvColors[i].Value : hsvColors[i].Value - hsvColors[localIP].Value;
                    }


                    var sizeChangePerPixelHue = toShiftHue / partSize;
                    var sizeChangePerPixelSat = toShiftSat / partSize;
                    var sizeChangePerPixelVal = toShiftVal / partSize;

                    for (int pX = partSize * i; pX < partSize * (i + 1); pX++)
                    {
                        var c = HSV.ColorFromHSV(
                            hsvColors[i].Hue + (sizeChangePerPixelHue * (pX % partSize) * (hueIncrease ? 1 : -1))
                            , hsvColors[i].Saturation + (sizeChangePerPixelSat * (pX % partSize) * (satIncrease ? 1 : -1))
                            , hsvColors[i].Value + (sizeChangePerPixelVal * (pX % partSize) * (valIncrease ? 1 : -1)));
                        for (int pY = 0; pY < bmp.Height; pY++)
                        {
                            var baseIndex = ((pY * bmp.Width) + pX) * 4;
                            ptr[baseIndex] = c.B;
                            ptr[baseIndex + 1] = c.G;
                            ptr[baseIndex + 2] = c.R;
                            ptr[baseIndex + 3] = 255;
                        }
                    }
                }
                bmp.UnlockBits(bmpData);
            }
        }
    }

    public struct HSV
    {
        public float Hue { get; set; }
        public float Saturation { get; set; }
        public float Value { get; set; }

        public HSV(Color c)
        {
            int max = Math.Max(c.R, Math.Max(c.G, c.B));
            int min = Math.Min(c.R, Math.Min(c.G, c.B));

            Hue = c.GetHue();
            Saturation = (max == 0) ? 0 : 1f - (1f * min / max);
            Value = max / 255f;
        }

        public HSV(float hue, float saturation, float value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            hue = (hue + 360) % 360;
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }


    }
}
