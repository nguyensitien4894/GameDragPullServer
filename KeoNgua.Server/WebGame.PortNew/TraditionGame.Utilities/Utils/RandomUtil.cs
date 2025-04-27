using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace MsTraditionGame.Utilities.Utils
{
    public class RandomUtil
    {
        protected static readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        protected static byte[] randomNumber = new byte[4];

        public static int NextByte(int value)
        {
            if (value == 0)
                throw new DivideByZeroException();
            rngCsp.GetBytes(randomNumber);
            return Math.Abs(randomNumber[0] % value);
        }

        public static int NextInt(int value)
        {
            if (value == 0)
                throw new DivideByZeroException();
            rngCsp.GetBytes(randomNumber);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(randomNumber);
            return Math.Abs(BitConverter.ToInt32(randomNumber, 0) % value);
        }

        public static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}