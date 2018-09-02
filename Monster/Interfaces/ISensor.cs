﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public interface ISensor : IBitInput
	{
		void Prepare(IMonster monster);
	}
}
