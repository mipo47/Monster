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
using System.Windows.Shapes;
using Monster;
using System.Windows.Threading;
using MonsterGUI.Controls;
using CellWorld;
using System.IO;
using Microsoft.Win32;

namespace MonsterGUI
{
	/// <summary>
	/// Interaction logic for MonsterInfo.xaml
	/// </summary>
	public partial class MonsterInfo : Window
	{
		DispatcherTimer timer;
		IMonster monster;
		UserControlArea controlArea;

		public MonsterInfo(UserControlArea controlArea, IMonster monster)
		{
			this.monster = monster;
			this.controlArea = controlArea;
			InitializeComponent();

			Title = monster.Name;
			
			timer = new DispatcherTimer();
			timer.Tick += new EventHandler(timer_Tick);
			timer.Interval = TimeSpan.FromMilliseconds(200);
			timer.Start();
		}

		void timer_Tick(object sender, EventArgs e)
		{
			TextBoxEnergy.Text = monster.Energy.ToString("f1");
			TextBoxName.Text = monster.Name;
			TextBoxAge.Text = monster.Age.ToString();
			TextBoxMemory.Text = monster.Brain.Genome.BrainStructure.MemoryBitCount + " ; " + monster.Brain.Genome.BrainStructure.TypeID;

			string parents = (monster as SimpleMonster.Monster).Parents;
			if (parents.Length > 20)
				parents = parents.Substring(parents.Length - 20);
			TextBoxParents.Text = parents;

			CellAreaInfo areaInfo = (CellAreaInfo) monster.AreaInfo;
			Left = -controlArea.Left + (areaInfo.X + 3) * controlArea.Scale + 16;
			Top = -controlArea.Top + (areaInfo.Y + 2) * controlArea.Scale + 32;

			if (Application.Current.MainWindow != null)
			{
				Left += Application.Current.MainWindow.Left;
				Top += Application.Current.MainWindow.Top;
			}
			else
				Close();

			if (monster.Energy <= 0f)
				Close();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			timer.Stop();
		}

		private void buttonSave_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.FileName = monster.Name;
			saveDialog.Title = "Save your monster";
			if (saveDialog.ShowDialog(this) == true)
			{
				monster.Brain.Genome.GetXml().Save(saveDialog.FileName);
			}
		}
	}
}
