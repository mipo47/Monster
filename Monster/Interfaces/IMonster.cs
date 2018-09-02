using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface IMonster
	{
		double Energy
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		int Age
		{
			get;
		}

		IBrain Brain
		{
			get;
		}

		IAreaInfo AreaInfo
		{
			get;
		}

        List<ISensor> Sensors
        {
            get;
        }

        List<IAction> Actions
        {
            get;
        }

		void NextStep(object actionLock);
	}
}
