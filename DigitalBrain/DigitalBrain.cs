using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Monster;

namespace DigitalNeuralNetwork
{
	public class DigitalBrain : IBrain
	{
		List<ILevel> levels = new List<ILevel>();
		Genome genome;
		BrainStructure brainStructure;

        MethodInfo compiled;
        bool autoCompile;
        
        public DigitalBrain(bool autoCompile = false)
        {
            this.autoCompile = autoCompile;
        }

		#region IBrain Members

		public List<ILevel> Levels
		{
			get { return levels; }
		}

		public Genome Genome
		{
			get { return genome; }
		}

		public BitStorage Memory
		{
			get;
			private set;
		}

		public void Load(Genome genome)
		{
			this.brainStructure = genome.BrainStructure;
			this.genome = genome;
			Load(brainStructure.InputCount, brainStructure.OutputCount, genome);
		}

		#endregion

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
			levels.Clear();
			if (genome == null)
			{
				brainStructure = new BrainStructure()
				{
					InputCount = inputCount,
					LevelSizes = new int[] { outputCount }
				};

				genome = new Genome(brainStructure);
				genome.Load(logic);
			}

			// add memory input and output
			inputCount += brainStructure.MemoryBitCount;

			int genomeOffset = 0;
			for (int levelIndex = 0; levelIndex < brainStructure.LevelSizes.Length; levelIndex++)
			{
				DigitalLevel level = new DigitalLevel();

				outputCount = brainStructure.LevelSizes[levelIndex];
				int logicLength = BrainStructure.GetLevelLogicLength(inputCount, outputCount);

				logic = genome.Copy(genomeOffset, logicLength);
				genomeOffset += logicLength;

				level.Load(inputCount, outputCount, logic);
				levels.Add(level);
				inputCount = outputCount;
			}

			// Load initial memory status
			if (brainStructure.MemoryBitCount > 0)
			{
				Memory = new BitStorage();
				Memory.Load(genome.Copy(genomeOffset, brainStructure.MemoryBitCount));
			}

			InputCount = brainStructure.InputCount;
			OutputCount = brainStructure.OutputCount;

            if (autoCompile)
                new Thread(() =>
                {
                    Compile();
                }).Start();
		}

		public void Process(IBitInput input, IBitOutput output)
		{
			if (input.Length != InputCount)
				throw new Exception("Invalid input bit count");

			if (output.Length != OutputCount)
				throw new Exception("Invalid output bit count");

			// Add memory bits to input
			if (Memory != null)
			{
				var inputWithMemory = new BitStorage(input.Length + Memory.Length);

				for (int i = 0; i < input.Length; i++)
					inputWithMemory[i] = input[i];

				for (int i = 0; i < Memory.Length; i++)
					inputWithMemory[i + input.Length] = Memory[i];

				input = inputWithMemory;
			}

			BitStorage result = null;

            if (compiled != null)
            {
                result = new BitStorage(OutputCount + (Memory != null ? Memory.Length : 0));
                compiled.Invoke(null, new object[] { input, result });
            }
            else
            {
                foreach (ILevel level in Levels)
                {
                    result = new BitStorage(level.OutputCount);
                    level.Process(input, result);
                    input = result;
                }
            }

			if (result.Length == OutputCount)
				output.Load(result);
			else
			{
				output.Load(result.Copy(0, OutputCount));

				// Update memory
                for (int i = 0; i < Memory.Length; i++)
                    Memory[i] = result[OutputCount + i];
			}
		}

		#endregion

        public void Compile()
        {
            if (compiled == null)
                compiled = BrainCompiler.CompileProcessor(this);
        }
	}
}
