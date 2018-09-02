using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface INeuron : IProcessor
	{
		void Load(int inputCount, IBitInput logic);
		bool Process(IBitInput input);
	}
}
