using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface IAction : IBitOutput
	{
		void Do(IMonster monster);
	}
}
