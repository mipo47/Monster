using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Monster
{
	public class BitStorage : IBitInput, IBitOutput
	{
		int length;
		BitArray bits;

		public BitArray Bits
		{
			set
			{
				if (value.Length == length)
					bits = value;
				else
					throw new Exception("Invalid bit count");
			}
		}

		public bool this[int i]
		{
			get 
			{
                //if (bits == null)
                //    bits = new BitArray(length);

				return bits[i]; 
			}
			set
			{
                //if (bits == null)
                //    bits = new BitArray(length);

				bits[i] = value;
			}
		}

		public int Length
		{
			get { return length; }
		}

		public BitStorage()
		{
		}

		public BitStorage(int length)
		{
			this.length = length;
            bits = new BitArray(length);
		}

		/// <summary>
		/// Load data from bit array
		/// </summary>
		/// <param name="values">array with data</param>
		public void Load(bool[] values)
		{
			if (length == 0 || values.Length == length)
				bits = new BitArray(values);
			else
				throw new Exception("Invalid bit count");
		}

		/// <summary>
		/// Data loading from string. String format example "001011101"
		/// </summary>
		/// <param name="values">string with bits encoded</param>
		public void Load(string values)
		{
			if (length == 0)
				length = values.Length;

			if (bits == null)
				bits = new BitArray(length);

			if (values.Length != bits.Length)
				throw new Exception("Invalid bit count");	

			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == '0')
					bits.Set(i, false);
				else if (values[i] == '1')
					bits.Set(i, true);
				else
					throw new Exception("Unknown bit format");
			}
		}

		/// <summary>
		/// Initializes bit array and sets all value equal to value
		/// </summary>
		/// <param name="value">default bit value for array</param>
		public void Load(bool value)
		{
			if (length == 0)
				return;
				
			if (bits == null)
				bits = new BitArray(length);

			bits.SetAll(value);
		}

		/// <summary>
		/// Load data from IBitInput
		/// </summary>
		/// <param name="values">array with data</param>
		public void Load(IBitInput input)
		{
			if (length == 0)
				length = input.Length;

			if (input.Length != length)
				throw new Exception("Invalid input bit count");

			bits = (BitArray)input.GetBitArray().Clone();
		}

		/// <summary>
		/// Encodes bits into string. Format example "01101110"
		/// </summary>
		/// <returns>bit string</returns>
		public string GetView()
		{
			if (length == 0)
				return string.Empty;

			StringBuilder view = new StringBuilder();

			foreach (bool bit in bits)
				view.Append(bit ? '1' : '0');

			return view.ToString();
		}

		/// <summary>
		/// Returns data as BitArray object
		/// </summary>
		/// <returns>Bit array</returns>
		public BitArray GetBitArray()
		{
			return bits;
		}

		public IBitInput Copy(int startBit, int length)
		{
			if (startBit >= Length)
				return new BitStorage();

			if (startBit + length > Length)
				length = Length - startBit;

			BitStorage storage = new BitStorage(length);
		    storage.bits = new BitArray(length);

			for (int i = 0; i < length; i++)
				storage.bits[i] = bits[i + startBit];

			return storage;
		}

		public void SetRandomValues()
		{
			if (length == 0)
				return;

			if (bits == null)
				bits = new BitArray(length);

			for (int i = 0; i < length; i++)
				bits[i] = Common.GetRandomBit();
		}
	}
}
