using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface IProcessor
	{
		int InputCount { get; }
		int OutputCount { get; }

		void Process(IBitInput input, IBitOutput output);
		void Load(int inputCount, int outputCount, IBitInput logic);
	}
}
