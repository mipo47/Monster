using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DigitalNeuralNetwork;
using Monster;
using CellWorld;
using System.Threading.Tasks;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
            Program program = new Program();
            program.Performance();
            //program.TestCompilation();

            //Primary primary = new Primary();
            //DateTime lastPrint = DateTime.Now;
            //for (int i = 0; i < 10000000; i++)
            //{
            //    bool print = false;
            //    if ((DateTime.Now - lastPrint).TotalSeconds > 1.0)
            //    {
            //        print = true;
            //        lastPrint = DateTime.Now;
            //    }
            //    primary.NextStep(print);	
            //}

			Console.ReadKey(false);
		}

        void TestCompilation()
        {
            var bs = new BrainStructure()
            {
                InputCount = 100,
                MemoryBitCount = 16,
                LevelSizes = new int[] { 50, 24 }
            };
            Genome genome = new Genome(bs);
            genome.SetRandomValues();

            var brain = new DigitalBrain();
            brain.Load(genome);

            var output = new BitStorage(bs.OutputCount);
            var input = new BitStorage(bs.InputCount);
            input.SetRandomValues();
            Console.WriteLine(input.GetView());

            int count = 10000;
            var start = DateTime.Now;
            Parallel.For(0, count, i =>
                                      {
                                          brain.Process(input, output);
                                      });

            Console.WriteLine(output.GetView() + " " + count / (DateTime.Now - start).TotalSeconds);

            brain.Compile();
            start = DateTime.Now;
            Parallel.For(0, count, i =>
            {
                brain.Process(input, output);
            });

            Console.WriteLine(output.GetView() + " " + count / (DateTime.Now - start).TotalSeconds);
        }

		void Performance()
		{
            var brains = new List<IBrain>();
            var bs = new BrainStructure()
            {
                InputCount = 100,
                MemoryBitCount = 16,
                LevelSizes = new int[] { 50, 24 }
            };
            for (int i = 0; i < 100; i++)
            {
                Genome genome = new Genome(bs);
                genome.SetRandomValues();

                var brain = new DigitalBrain();
                brain.Load(genome);
                brains.Add(brain);
            }

            Console.WriteLine("Compiling");
            int nr = 0;
		    var cStart = DateTime.Now;
            Parallel.ForEach(brains, brain =>
            {
                (brain as DigitalBrain).Compile();
                lock (brains)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Compiling " + ++nr);
                }
            });

		    Console.WriteLine("Compilations per second " + nr/(DateTime.Now - cStart).TotalSeconds);
            return;

            var output = new BitStorage(bs.OutputCount);
            int op = 0;
			DateTime start = DateTime.Now;
			for (int i = 0; i <= 10000; i++)
			{
                var input = new BitStorage(bs.InputCount);
                input.SetRandomValues();
                Parallel.ForEach(brains, brain =>
                {
                    brain.Process(input, output);
                });

                op += brains.Count;

				if (i % 10 == 0)
				{
					double seconds = (DateTime.Now - start).TotalSeconds;
					double performance = op;
					performance /= seconds;

					Console.SetCursorPosition(0, 0);
					Console.WriteLine(seconds.ToString("f2") + " => " + i + " ; " + performance.ToString("f2"));
				}
			}
            Console.WriteLine("Press any key");
			Console.ReadKey(true);
		}

		void Start()
		{
			int memorySize = 2;
			BrainStructure brainStructure = new BrainStructure()
			{  
				InputCount = 16,
				LevelSizes = new int[] { 3 + memorySize },
				MemoryBitCount = memorySize
			};

			Genome genome = new Genome(brainStructure);
			genome.SetRandomValues();
			Console.WriteLine("Genome: " + genome.GetView());

			DigitalBrain brain = new DigitalBrain();
			brain.Load(genome);

			BitStorage input = new BitStorage(brainStructure.InputCount);
			BitStorage output = new BitStorage(brainStructure.OutputCount);

			for (int i = 0; i < 5; i++)
			{
				input.SetRandomValues();
				Console.WriteLine("\r\nInput: " + input.GetView());
				for (int j = 0; j < 1; j++)
				{
					//int bitToChange = Common.Random.Next(genome.Length);
					//genome[bitToChange] ^= true;
					//Console.WriteLine("Genome: " + genome.GetView());

					//brain.Load(genome);

					for (int k = 0; k < 3; k++)
					{
						brain.Process(input, output);
						Console.WriteLine("Output: " + output.GetView() + " Memory: " + brain.Memory.GetView());
					}
				}
			}
		}

		void TestCellWorld()
		{
			CellArea cellArea = new CellArea(70, 35);
            bool repeat = true;
            int iteration = 0;
            while (repeat)
            {
                cellArea.NextStep();

				double energySum = cellArea.Monsters.Sum(m => m.Energy);
				if (energySum > 2000f)
					break;

                if (++iteration % 2000 == 0)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(iteration + " => " + energySum.ToString("f2") + "            ");
                    Console.Out.Flush();
                }
            }

            do
            {
                for (int i = 0; i < cellArea.Monsters.Count; i++)
                    cellArea.NextStep();

                PrintCellArea(cellArea);
                Console.WriteLine("Press spacebar to continue    ");
            }
            while (Console.ReadKey(false).Key == ConsoleKey.Spacebar);
		}

		void PrintCellArea(CellArea cellArea)
		{
			Console.SetCursorPosition(0, 0);
            Console.Write("   ");
            for (int w = 0; w < cellArea.Width; w++)
                Console.Write(w % 10);
            Console.WriteLine();

            var top = cellArea.Monsters.OrderBy(m => -m.Energy).ToList();

			for (int h = 0; h < cellArea.Height; h++)
			{
                Console.Write(h);
                Console.Write(' ');
                if (h < 10)
                    Console.Write(' ');

				for (int w = 0; w < cellArea.Width; w++)
				{
					Cell cell = cellArea.GetCellAt(w, h);
					switch (cell.ContentType)
					{
						case ContentType.Empty:
							Console.Write('.');
							break;
						case ContentType.Food:
							Console.Write('+');
							break;
						case ContentType.Monster:
                            int place = top.IndexOf(cell.Content as IMonster);
                            if (place >= 10)
                                Console.Write('M');
                            else
                                Console.Write(place);
							break;
						case ContentType.Wall:
							Console.Write('#');
							break;
					}
				}
				Console.WriteLine();
			}

            foreach (IMonster monster in top.Take(32))
            {
                Console.WriteLine(monster.AreaInfo + ": " + monster.Energy.ToString("f1") + "               "); 
            }
			Console.Out.Flush();
		}
	}
}
