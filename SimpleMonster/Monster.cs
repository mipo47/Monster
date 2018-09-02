using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;
using DigitalNeuralNetwork;

namespace SimpleMonster
{
    public class Monster : IMonster
    {
		public string Parents = "";

		#region IMonster properties
		public double Energy
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public int Age
		{
			get;
			protected set;
		}

        public IBrain Brain
        {
            get;
            private set;
        }

        public IAreaInfo AreaInfo
        {
            get;
            private set;
        }

        public List<ISensor> Sensors
        {
            get;
            private set;
        }

        public List<IAction> Actions
        {
            get;
            private set;
		}
		#endregion

		public Monster(Genome genome, IAreaInfo areaInfo)
        {
            Brain = new DigitalBrain();
            Brain.Load(genome);
            //((DigitalBrain)Brain).Compile();

            AreaInfo = areaInfo;

            Sensors = new List<ISensor>();
            Actions = new List<IAction>();

			Name = Common.GenerateName(4, 6);
			Age = 0;
        }

        public void NextStep(object actionLock)
		{
			BitStorage brainInput = new BitStorage(Brain.InputCount);
			int bitPosition = 0;

			// Prepare brain input using sensors
			foreach (ISensor sensor in Sensors)
			{
				sensor.Prepare(this);
				for (int i = 0; i < sensor.Length; i++)
					brainInput[i + bitPosition] = sensor[i];

				bitPosition += sensor.Length;
			}

			BitStorage brainOutput = new BitStorage(Brain.OutputCount);
			Brain.Process(brainInput, brainOutput);

			// Make actions using brain output
			bitPosition = 0;
            lock (actionLock)
            {
                foreach (IAction action in Actions)
                {
                    action.Load(brainOutput.Copy(bitPosition, action.Length));
                    action.Do(this);
                    bitPosition += action.Length;
                }
            }

			Age++;
		}
    }
}
