using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Monster
{
	public interface IBitInput
	{
		/// <summary>
		/// Data array
		/// </summary>
		bool this[int i]
		{
			get;
		}

		/// <summary>
		/// Bit count in data
		/// </summary>
		int Length
		{
			get;
		}

		/// <summary>
		/// Cuts bits from data and create new IBitInput
		/// </summary>
		/// <param name="startBit">Start bit position</param>
		/// <param name="length">Amount of bits to be copyed</param>
		/// <returns>Sub-data from bit array</returns>
		IBitInput Copy(int startBit, int length);

		/// <summary>
		/// Encodes bits into string. Format example "01101110"
		/// </summary>
		/// <returns>bit string</returns>
		string GetView();

		/// <summary>
		/// Returns data as BitArray object
		/// </summary>
		/// <returns>Bit array</returns>
		BitArray GetBitArray();
	}
}
