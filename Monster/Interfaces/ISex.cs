using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster.Interfaces
{
	public interface ISex
	{
		Genome MakeChild(Genome active, Genome passive, int mutation);
	}
}
