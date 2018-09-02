using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;
using System.Linq.Expressions;
using System.Reflection;

namespace DigitalNeuralNetwork
{
	class DigitalNeuron : INeuron
	{
		LogicOperation[] logicOperations;

		public bool InitValue
		{
			get;
			protected set;
		}

		public bool Negative
		{
			get;
			protected set;
		}

        public LogicOperation[] LogicOperations
        {
            get { return logicOperations; }
        }

		#region IProcessor Members

		public int InputCount
		{
			get;
			private set;
		}

		public int OutputCount
		{
			get;
			private set;
		}

		public void Load(int inputCount, int outputCount, IBitInput logic)
		{
			Load(inputCount, logic);
		}

		public void Process(IBitInput input, IBitOutput output)
		{
			output[0] = Process(input);
		}

		#endregion

		#region INeuron Members

		public void Load(int inputCount, IBitInput logic)
		{
			InputCount = inputCount;
			OutputCount = 1;

			logicOperations = new LogicOperation[inputCount];

			// Load neuron state
			InitValue = logic[0];
			Negative = logic[1];

			for (int inputNr = 0; inputNr < inputCount; inputNr++)
			{
				IBitInput operationBits = logic.Copy(Settings.NeuronStateBitCount + inputNr * Settings.LogicOperationBitCount, Settings.LogicOperationBitCount);
				logicOperations[inputNr] = Settings.GetLogicOperation(operationBits);
			}
		}

		public bool Process(IBitInput input)
		{
			if (input.Length != InputCount)
				throw new Exception("Invalid input bit count");

			bool result = InitValue;
            int inputCount = InputCount;
			for (int i = 0; i < inputCount; i++)
			{
				switch (logicOperations[i])
				{
					case LogicOperation.None:
						continue;

					case LogicOperation.Or:
						result |= input[i];
						break;

					case LogicOperation.And:
						result &= input[i];
						break;

					case LogicOperation.Xor:
						result ^= input[i];
						break;
				}
			}

			result ^= Negative;

			return result;
		}

		#endregion
	}
}
