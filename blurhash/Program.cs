using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ImageMagick;

namespace blurhash
{
    class Program
    {
        static void Main(string[] args)
        {
            int componentX = System.Convert.ToInt32(args[0]);
            int componentY = System.Convert.ToInt32(args[1]);
            string pathToFile = args[2];
            

            MagickImage im = new MagickImage(pathToFile);
            IPixelCollection pc = im.GetPixels();
            byte[] pixels = pc.ToByteArray(0, 0, im.Width, im.Height, PixelMapping.RGBA);

            Console.WriteLine(encode(pixels, im.Width, im.Height, componentX, componentY));
        }

        private static int bytesPerPixel = 4;

        private delegate float basisFunction(float i, float j);

        private static List<float> multiplyBasisFunction (byte[] pixels, int width, int height, basisFunction basisFunction)
        {
            float r = 0;
            float g = 0;
            float b = 0;
            var bytesPerRow = width * bytesPerPixel;

            for (int x = 0; x<width; x++) {
                for (int y = 0; y<height; y++) {
                    var basis = basisFunction(x, y);

                    r += basis* Util.sRGBToLinear(pixels[bytesPerPixel * x + 0 + y * bytesPerRow]);
                    g += basis* Util.sRGBToLinear(pixels[bytesPerPixel * x + 1 + y * bytesPerRow]);
                    b += basis* Util.sRGBToLinear(pixels[bytesPerPixel * x + 2 + y * bytesPerRow]);
                }
            }

            float scale = 1 / (float)(width * height);

            return new List<float>{ r* scale, g *scale, b * scale};
        }

        private static int encodeDC(List<float> value)
        {
            int roundedR = Util.linearTosRGB(value[0]);
            int roundedG = Util.linearTosRGB(value[1]);
            int roundedB = Util.linearTosRGB(value[2]);
            return (roundedR << 16) + (roundedG << 8) + roundedB;
        }

        private static int encodeAC(List<float> value, float maximumValue)
        {
            var quantR = (int)Math.Max(0, Math.Min(18, Math.Floor(Util.signPow(value[0] / maximumValue, 0.5F) * 9 + 9.5F)));
            var quantG = (int)Math.Max(0, Math.Min(18, Math.Floor(Util.signPow(value[1] / maximumValue, 0.5F) * 9 + 9.5F)));
            var quantB = (int)Math.Max(0, Math.Min(18, Math.Floor(Util.signPow(value[2] / maximumValue, 0.5F) * 9 + 9.5F)));
            return quantR * 19 * 19 + quantG * 19 + quantB;
        }


        private static string encode(byte[] pixels, int width, int height, int componentX, int componentY)
        {
            if (componentX < 1 || componentX > 9 || componentY < 1 || componentY > 9)
            {
                throw new Exception("BlurHash must have between 1 and 9 components");
            }
            if (width * height * 4 != pixels.Count())
            {
                throw new Exception("Width and height must match the pixels array");
            }
            var factors = new List<List<float>>();
            for (int y = 0; y < componentY; y++)
            {
                for (int x = 0; x < componentX; x++)
                {
                    var normalisation = x == 0 && y == 0 ? 1 : 2;
                    var factor = multiplyBasisFunction(pixels, width, height, (i, j) => normalisation *
                        (float)Math.Cos((Math.PI * x * i) / width) *
                        (float)Math.Cos((Math.PI * y * j) / height));
                    factors.Add(factor);
                }
            }
            var dc = factors[0];
            var ac = factors.Skip(1).Take(factors.Count() - 1);
            var hash = "";
            double sizeFlag = componentX - 1 + (componentY - 1) * 9;
            hash += Base83.encode(sizeFlag, 1);
            float maximumValue;

            if (ac.Count() > 0)
            {

                double actualMaximumValue = 0;
                foreach (List<float> val in ac)
                {
                    foreach (float n in val)
                    {
                        actualMaximumValue = Math.Max(n, actualMaximumValue);
                    }
                }

                float quantisedMaximumValue = (float)Math.Floor(Math.Max(0, Math.Min(82, Math.Floor(actualMaximumValue * 166 - 0.5F))));
                maximumValue = (quantisedMaximumValue + 1) / 166;
                hash += Base83.encode(quantisedMaximumValue, 1);
            }
            else
            {
                maximumValue = 1;
                hash += Base83.encode(0, 1);
            }
            hash += Base83.encode(encodeDC(dc), 4);

            foreach(var factor in ac)
            {
                hash += Base83.encode(encodeAC(factor, maximumValue), 2);
            }

            return hash;
        }

    }
}
