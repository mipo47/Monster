using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;
using DigitalNeuralNetwork;
using System.Threading.Tasks;

namespace Tester
{
	public class Brain : DigitalBrain
	{
		public int Correct;
		public BitStorage Output = new BitStorage(1);
	}

	public class Primary
	{
		static Random random = new Random((int)DateTime.Now.Ticks);
		List<Brain> brains = new List<Brain>();

		List<ushort> primes = new List<ushort>();
		List<ushort> nonPrimes = new List<ushort>();
		Dictionary<BitStorage, bool> numbers = new Dictionary<BitStorage, bool>();

		Sex sex = new Sex();
		ushort max = 1;
		int maxCorrect = 0;

		public Primary()
		{
			Console.WriteLine("Loading numbers");
			for (ushort i = 0; i < 200; i++)
				AddNumber();
			Console.WriteLine("Loaded");

			for (int i = 0; i < 3; i++)
				AddBrain();

			Console.WriteLine("Genome lenght: " + brains[0].Genome.Length);
		}

		public void AddNumber()
		{
			maxCorrect++;
			max += 2;
			bool isPrime = IsPrime(max);

			BitStorage input = new BitStorage(16);
			for (int bit = 0; bit < 16; bit++)
				input[bit] = ((max >> bit) & 1) > 0;
			numbers.Add(input, isPrime);

			Console.WriteLine("Adding " + max + " - " + isPrime);
			if (isPrime)
				primes.Add(max);
			else
				nonPrimes.Add(max);
		}

		public void AddBrain()
		{
			BrainStructure bs = new BrainStructure()
			{
				InputCount = 16,
				MemoryBitCount = 0,
				LevelSizes = new int[] { 32, 16, 1 }
			};

			Genome genome = new Genome(bs);
			genome.SetRandomValues();

			Brain brain = new Brain();
			brain.Load(genome);

			brains.Add(brain);
		}

		int Rand(int max)
		{
			int value = (int) Math.Sqrt(random.Next(max*max));
			if (value == max)
				value--;
			return value;
		}

		public void NextStep(bool print)
		{
			Parallel.For(0, brains.Count, b =>
			{
				Brain brain = brains[b];
				brain.Correct = 0;
				BitStorage output = brain.Output;

				foreach (var pair in numbers)
				{
					BitStorage input = pair.Key;
					bool isPrime = pair.Value;

					brain.Process(input, output);
					bool result = output[0];

					if (result == isPrime)
						brain.Correct++;
				}
			});

			brains = brains.OrderBy(b => b.Correct).ToList();

			Brain bestBrain = brains[brains.Count - 1];
			Brain secondBrain = brains[brains.Count - 2];
			Brain worstBrain = brains[0];
			brains.Remove(worstBrain);

			if (print)
			{
				Console.WriteLine("best : " + bestBrain.Correct + ", second: " + secondBrain.Correct + ", worst: " + worstBrain.Correct + ", max: " + maxCorrect);
			}
			
			if (bestBrain.Correct > (maxCorrect * 85) / 100)
				AddNumber();

			Genome genome = sex.MakeChild(bestBrain.Genome, secondBrain.Genome, random.Next(1, 100));
			Brain newBrain = new Brain();
			newBrain.Load(genome);
			brains.Add(newBrain);
		}

		public bool IsPrime(ushort number)
		{
			ushort max = (ushort)Math.Sqrt(number + 1);
			if (number % 2 == 0)
				return false;

			for (ushort i = 3; i <= max; i += 2)
				if (number % i == 0)
					return false;

			return true;
		}
	}
}
