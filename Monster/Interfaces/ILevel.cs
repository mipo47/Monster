using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface ILevel : IProcessor
	{
		List<INeuron> Neurons
		{
			get;
		}
	}
}
