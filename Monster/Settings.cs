using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public static class Settings
	{
		public static readonly int LogicOperationBitCount = 2;
		public static readonly int NeuronStateBitCount = 2;

		public static LogicOperation GetLogicOperation(IBitInput operationBits)
		{
			if (operationBits.Length != LogicOperationBitCount)
				throw new Exception("Invalid bit count");

			int operationNr = (operationBits[0] ? 2 : 0)
				+ (operationBits[1] ? 1 : 0);

			return (LogicOperation)operationNr;
		}
	}
}
