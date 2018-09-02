using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface IBrain : IProcessor
	{
		List<ILevel> Levels
		{
			get;
		}

		Genome Genome
		{
			get;
		}

		BitStorage Memory
		{
			get;
		}

		void Load(Genome genome);
	}
}
