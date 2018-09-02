using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using CellWorld;
using System.Windows.Threading;
using Monster;
using Microsoft.Win32;
using System.Xml.Linq;

namespace MonsterGUI
{
	public partial class WindowArea : Window
	{
		DispatcherTimer timer;

		public WindowArea()
		{
			InitializeComponent();

			CellArea cellArea = new CellArea(75, 45, 4000);
			MainArea.Area = cellArea;

			timer = new DispatcherTimer();
			timer.Tick += new EventHandler(timer_Tick);
			timer.Interval = TimeSpan.FromMilliseconds(100);

            sliderMutation.Value = Sex.DEFAULT_MUTATION;
		}

		void timer_Tick(object sender, EventArgs e)
		{
			//int stepCount = MainArea.Area.Monsters.Count;
			//if (stepCount == 0)
			//    stepCount = 1;

			if (checkBoxEvolution.IsChecked == true)
				MainArea.MultiStep(200);
			else
				MainArea.NextStep();

			CellArea area = MainArea.Area;
			double usedEnergy = 0;
			double mEnergy = 0;
			double fEnergy = 0;
			
			for (int x = 0; x < area.Width; x++)
				for (int y = 0; y < area.Height; y++)
				{
					Cell cell = area.GetCellAt(x, y);
					if (cell.ContentType == ContentType.Food)
					{
						usedEnergy += CellArea.FOOD_ENERGY;
						fEnergy += CellArea.FOOD_ENERGY;
					}
					else if (cell.ContentType == ContentType.Monster)
					{
						IMonster monster = (IMonster)cell.Content;
						usedEnergy += monster.Energy;
						mEnergy += monster.Energy;
					}
				}

			if (area.Monsters.Count > 0)
			{
				double avgEnergy = mEnergy / area.Monsters.Count;
				double best = area.Monsters.Max(m => m.Energy);
				IMonster bestMonster = area.Monsters.First(m => m.Energy == best);
				int bestMemory = bestMonster.Brain.Genome.BrainStructure.TypeID;

				double perf = 100.0 * mEnergy / area.TotalEnergy;

				int oldest = area.Monsters.Max(m => m.Age);
				IMonster oldestMonster = area.Monsters.First(m => m.Age == oldest);
				int oldestMemory = oldestMonster.Brain.Genome.BrainStructure.TypeID;
				Title = string.Format("{0:f0} : {1:f1} ; {2:f2} / {3:f1} - {4} |{5}|{6} ; {7:f0}", area.TotalEnergy, perf, avgEnergy, best, oldest, oldestMemory, bestMemory, sliderEvolution.Value);

				if (checkBoxEvolution.IsChecked == true)
				{
					double aimPerf = sliderEvolution.Value;
					if (perf > aimPerf || 4000.0 - MainArea.TotalEnergy >= aimPerf - perf)
						MainArea.TotalEnergy -= perf - aimPerf;
				}
			}
		}

		private void buttonStart_Click(object sender, RoutedEventArgs e)
		{
			if (buttonStart.Content.ToString() == "Start")
			{
				buttonStart.Content = "Stop";
				timer.Start();
			}
			else
			{
				buttonStart.Content = "Start";
				timer.Stop();
			}
		}

		private void buttonLoad_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Title = "Open saved monster";
			if (openDialog.ShowDialog(this) == true)
			{
				XDocument info = XDocument.Load(openDialog.FileName);
				Genome genome = new Genome(info);
				for (int i = 0; i < 10; i++)
					MainArea.Area.AddNewMonster(genome).Name = System.IO.Path.GetFileNameWithoutExtension(openDialog.FileName);
				MainArea.Refresh();
			}
		}

        private void checkBoxEvolution_Checked(object sender, RoutedEventArgs e)
        {
            if (checkBoxEvolution.IsChecked == true)
                timer.Interval = TimeSpan.FromMilliseconds(1);
            else
                timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void sliderMutation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                Sex.DEFAULT_MUTATION = (int) sliderMutation.Value;
                sliderMutation.ToolTip = "Mutation amount = " + Sex.DEFAULT_MUTATION;
            }
        }
	}
}
