using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blurhash
{
    class Base83
    {
        private static string digitCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#$%*+,-.:;=?@[]^_{|}~";

        public static int decode(string input) {
            var value = 0;
            for (int i = 0; i < input.Length; i++) {
                var c = input[i];
                var digit = digitCharacters.IndexOf(c);
                value = value* 83 + digit;
            }
            return value;
        }

        public static string encode(double number, int length)
        {
            var result = "";
            for (int i = 1; i <= length; i++)
            {
                var digit = ((int)number / Math.Pow(83, length - i)) % 83;
                result += digitCharacters[(int)digit];
            }
            return result;
        }

    }
}
