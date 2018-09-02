using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public static class Common
	{
		static string letters = "QWERTYUIOPASDFGHJKLZXCVBNM1234567890";
		static short bitsLeft = 0;
		static int bits;

        public static readonly Random Random = new Random((int)DateTime.Now.Ticks); //(int)DateTime.Now.Ticks

		public static bool GetRandomBit()
		{
			if (bitsLeft == 0)
			{
				bits = Random.Next(0x10000000);
				bitsLeft = 28;
			}

			bool result = ((bits >> bitsLeft) & 1) > 0;
			bitsLeft--;
			return result;
		}

		public static string GenerateName(int minLength, int maxLength)
		{
			int length = minLength + Random.Next(1 + maxLength - minLength);
			StringBuilder name = new StringBuilder(length);

			for (int i = 0; i < length; i++)
			{
				char letter = letters[Random.Next(letters.Length)];
				name.Append(letter);
			}

			return name.ToString();
		}
	}
}
