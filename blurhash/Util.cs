using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blurhash
{
    class Util
    {
        public static float sRGBToLinear(byte value)
        {
            float v = value / 255F;
            if (v <= 0.04045F) {
                return (v / 12.92F);
            } else {
            return (float)Math.Pow((v + 0.055F) / 1.055F, 2.4F);
            }
        }

        public static int linearTosRGB(float value)
        {
            float v = Math.Max(0F, Math.Min(1F, value));
            if (v <= 0.0031308F) {
                return (int)(v* 12.92F * 255 + 0.5F);
            } else {
                return (int)((1.055F * Math.Pow(v, 1 / 2.4F) - 0.055F) * 255 + 0.5F);
            }
        }

        public static int sign(float n)
        {
            return (n < 0 ? -1 : 1);
        }

        public static float signPow(float val, float exp)
        {
            return sign(val) * (float)Math.Pow(Math.Abs(val), exp);
        }
    }
}
