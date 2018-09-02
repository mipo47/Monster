using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;
using System.Threading.Tasks;

namespace DigitalNeuralNetwork
{
	class DigitalLevel : ILevel
	{
		List<INeuron> neurons;

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

		public void Process(IBitInput input, IBitOutput output)
		{
			if (input.Length != InputCount)
				throw new Exception("Invalid input bit count");

			if (output.Length != OutputCount)
				throw new Exception("Invalid output bit count");

            int outputCount = OutputCount;
            for (int bitPos = 0; bitPos < outputCount; bitPos++)
            {
                INeuron neuron = neurons[bitPos];
                output[bitPos] = neuron.Process(input);
            }

            //Parallel.For(0, outputCount, bitPos =>
            //{
            //    INeuron neuron = neurons[bitPos];
            //    output[bitPos] = neuron.Process(input);
            //});
		}

		#endregion

		#region ILevel Members

		public List<INeuron> Neurons
		{
			get { return neurons; }
		}

		public void Load(int inputCount, int outputCount, IBitInput logic)
		{
			InputCount = inputCount;
			OutputCount = outputCount;

			neurons = new List<INeuron>(outputCount);
			int neuronLogicLength = logic.Length / outputCount;

			for (int i = 0; i < outputCount; i++)
			{
				DigitalNeuron neuron = new DigitalNeuron();
				IBitInput neuronLogic = logic.Copy(i * neuronLogicLength, neuronLogicLength);
				neuron.Load(inputCount, neuronLogic);
				neurons.Add(neuron);
			}
		}

		#endregion
	}
}
