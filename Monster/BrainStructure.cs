using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monster
{
	public class BrainStructure
	{
		static readonly int[] EMPTY_ARRAY = new int[0];

		public BrainStructure()
		{
			LevelSizes = EMPTY_ARRAY;
		}

		public int TypeID
		{
			get;
			set;
		}

		public int InputCount
		{
			get;
			set;
		}

		public int OutputCount
		{
			get
			{
				if (LevelSizes.Length == 0)
					return 0;
				else
					return LevelSizes.Last() - MemoryBitCount;
			}
		}

		public int MemoryBitCount
		{
			get;
			set;
		}

		public int[] LevelSizes
		{
			get;
			set;
		}

		public int GenomeLength
		{
			get
			{
				int genomeLength = MemoryBitCount;
				int previousLevelSize = InputCount + MemoryBitCount;

				foreach (int levelSize in LevelSizes)
				{
					genomeLength += GetLevelLogicLength(previousLevelSize, levelSize);
					previousLevelSize = levelSize;
				}

				return genomeLength;
			}
		}

		public static bool operator==(BrainStructure bs1, BrainStructure bs2)
		{
			if (bs1.InputCount != bs2.InputCount
				|| bs1.OutputCount != bs2.OutputCount
				|| bs1.MemoryBitCount != bs2.MemoryBitCount
				|| bs1.LevelSizes.Length != bs2.LevelSizes.Length)
				return false;

			for (int i = 0; i < bs1.LevelSizes.Length; i++)
				if (bs1.LevelSizes[i] != bs2.LevelSizes[i])
					return false;

			return true;
		}

		public static bool operator!=(BrainStructure bs1, BrainStructure bs2)
		{
			return !(bs1 == bs2);
		}

		public static int GetLevelLogicLength(int inputCount, int outputCount)
		{
			return outputCount * (Settings.NeuronStateBitCount + inputCount * Settings.LogicOperationBitCount);
		}
	}
}
