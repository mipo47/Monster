using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Monster
{
	public interface IBitOutput
	{
		bool this[int i]
		{
			set;
		}

		/// <summary>
		/// Bit count in data
		/// </summary>
		int Length
		{
			get;
		}

		/// <summary>
		/// Load data from bit array
		/// </summary>
		/// <param name="values">array with data</param>
		void Load(bool[] values);

		/// <summary>
		/// Data loading from string. String format example "001011101"
		/// </summary>
		/// <param name="values">string with bits encoded</param>
		void Load(string values);

		/// <summary>
		/// Initializes bit array and sets all value equal to value
		/// </summary>
		/// <param name="value">default bit value for array</param>
		void Load(bool value);

		/// <summary>
		/// Load data from IBitInput
		/// </summary>
		/// <param name="values">array with data</param>
		void Load(IBitInput input);
	}
}
