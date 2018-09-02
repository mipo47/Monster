using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface IArea
	{
		double TotalEnergy
		{
			get;
			set;
		}

        List<IMonster> Monsters
        {
            get;
        }
	}
}
