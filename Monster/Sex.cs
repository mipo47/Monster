using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster.Interfaces;

namespace Monster
{
	public class Sex : ISex
	{
        public static int DEFAULT_MUTATION = 16;
        public static int DEFAULT_SEX_MUTATION = 2;

		#region ISex Members

        public Genome MakeChild(Genome active, Genome passive, int mutation = 0)
		{
			if (active.BrainStructure != passive.BrainStructure)
				return null;

            if (mutation == 0)
                mutation = DEFAULT_SEX_MUTATION;

			Genome genome = new Genome(active.BrainStructure);
			for (int i = 0; i < active.Length; i++)
			{
				if (Common.GetRandomBit())
					genome[i] = active[i];
				else
					genome[i] = passive[i];
			}

			// Mutation
			int bitCount = 0;
			//mutation = genome.Length / mutation;
			if (mutation > 0)
				bitCount = Common.Random.Next(0, mutation);

			for (int i = 0; i < bitCount; i++)
			{
				int bitToChange = Common.Random.Next(genome.Length);
				genome[bitToChange] ^= true;
			}

			return genome;
		}

		#endregion
	}
}
