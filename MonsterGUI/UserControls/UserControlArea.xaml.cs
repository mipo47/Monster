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
using CellWorld;
using System.ComponentModel;
using Monster;
using System.Windows.Media.Effects;

namespace MonsterGUI.Controls
{
	/// <summary>
	/// Interaction logic for UserControlArea.xaml
	/// </summary>
	public partial class UserControlArea : UserControl
	{
		static Uri[] meals = new Uri[] {
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\apple.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\corn.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\nuts.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\carrot.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\cheese.jpg")
		};

		static Uri[] monsters = new Uri[] {
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\hamster1.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\hamster2.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\hamster3.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\hamster4.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\hamster5.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\hamster6.jpg"),
			new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\hamster7.jpg"),
		};

		CellArea area;
		Dictionary<Cell, Rectangle> rectangles;
		bool drag = false;
		DateTime previousMove = DateTime.Now;
		Point position;

		public double Scale
		{
			get;
			protected set;
		}

		public double Left
		{
			get;
			protected set;
		}

		public double Top
		{
			get;
			protected set;
		}

		public CellArea Area
		{
			get
			{
				return area;
			}
			set
			{
				area = value;
				Refresh();
			}
		}

		public double TotalEnergy
		{
			get { return Area.TotalEnergy; }
			set { sliderEnergy.Value = value; }
		}

		public Canvas Canvas
		{
			get { return CanvasArea; }
		}

		public UserControlArea()
		{
			InitializeComponent();
			Scale = 17.0;
		}

		public UserControlArea(CellArea cellArea)
			: this()
		{
			Area = cellArea;
			sliderEnergy.Value = Area.TotalEnergy;
		}

		public void MultiStep(int stepCount)
		{
			StepInfo stepInfos = new StepInfo();

			for (int step = 0; step < stepCount; step++)
			{
				StepInfo stepInfo = area.NextStep();
				foreach (Cell cell in stepInfo.ChangedCells)
					stepInfos.AddCell(cell);
			}

			foreach (Cell cell in stepInfos.ChangedCells)
			{
				if (!rectangles.ContainsKey(cell))
					continue;

				Rectangle rectangle = rectangles[cell];
				if (rectangle != null)
					RefreshRectangle(rectangle, cell);
			}
		}

		public void NextStep()
		{
			StepInfo stepInfo = area.NextStep();

			foreach (Cell cell in stepInfo.ChangedCells)
			{
				if (!rectangles.ContainsKey(cell))
					continue;

				Rectangle rectangle = rectangles[cell];
				Cell realCell = area.GetCellAt(cell.AreaInfo);
				if (rectangle != null)
					RefreshRectangle(rectangle, realCell);
			}
		}

		public void Refresh()
		{
			if (rectangles != null)
			{
				foreach (var pair in rectangles)
					pair.Value.MouseUp -= rectangle_MouseUp;
			}

			rectangles = new Dictionary<Cell, Rectangle>();
			CanvasArea.Children.Clear();

			int endX = (int)(Left / Scale);
			if (--endX < 0)
				endX += area.Width;

			int endY = (int)(Top / Scale);
			if (--endY < 0)
				endY += area.Height;

			for (double xPos = Left; ; xPos += Scale)
			{
				int x = (int)(xPos / Scale);

				for (double yPos = Top; ; yPos += Scale)
				{
					Rectangle rectangle = new Rectangle
					{
						Width = Scale,
						Height = Scale,
					};

					int y = (int)(yPos / Scale);

					Cell cell = area.GetCellAt(x, y);
					rectangles[cell] = rectangle;
					RefreshRectangle(rectangle, cell);

					Canvas.SetLeft(rectangle, xPos - Left);
					Canvas.SetTop(rectangle, yPos - Top);

					CanvasArea.Children.Add(rectangle);

					if (endY % area.Height == y % area.Height)
						break;
				}

				if (endX % area.Width == x % area.Width)
					break;
			}
		}

		void RefreshRectangle(Rectangle rectangle, Cell cell)
		{
			BitmapImage bi;
			rectangle.MouseUp -= rectangle_MouseUp;
			rectangle.Stroke = null;

			switch (cell.ContentType)
			{
				case ContentType.Empty:
					rectangle.Fill = Brushes.White;
					break;

				case ContentType.Food:
					int mealNr = cell.AreaInfo.X + cell.AreaInfo.Y;
					mealNr %= meals.Length;
					bi = new BitmapImage(meals[mealNr]);
					rectangle.Fill = new ImageBrush(bi);
					break;

				case ContentType.Monster:
					IMonster monster = cell.Content as IMonster;
					int monsterNr = monster.Brain.Genome.BrainStructure.TypeID;
					bi = new BitmapImage(monsters[monsterNr]);
					rectangle.Fill = new ImageBrush(bi);
					rectangle.MouseUp += rectangle_MouseUp;
					if (monster.Age > 1000)
					{
						if (monster.Age > 10000)
							rectangle.Stroke = new SolidColorBrush(Colors.Red);
						else if (monster.Age > 5000)
							rectangle.Stroke = new SolidColorBrush(Colors.Blue);
						else if (monster.Age > 2500)
							rectangle.Stroke = new SolidColorBrush(Colors.Green);
						else
							rectangle.Stroke = new SolidColorBrush(Colors.YellowGreen);
					}
					break;

				case ContentType.Wall:
					bi = new BitmapImage(new Uri(@"G:\Projects\exa\Monster\MonsterGUI\Images\wall.jpg"));
					rectangle.Fill = new ImageBrush(bi);
					break;
			}
		}

		void rectangle_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Rectangle rectange = sender as Rectangle;
			Cell cell = rectangles.Single(pair => pair.Value == rectange).Key;
			IMonster monster = (IMonster) cell.Content;
			MonsterInfo monsterInfo = new MonsterInfo(this, monster);
			monsterInfo.Show();
		}

		private void CanvasArea_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (!DesignerProperties.GetIsInDesignMode(this))
				Refresh();
		}

		private void CanvasArea_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Scale *= (1 + 0.001 * e.Delta);
			if (Scale < 1)
				Scale = 1;

			Refresh();
		}

		private void MoveCanvasArea(Point newPosition)
		{
			Left -= newPosition.X - position.X;
			Top -= newPosition.Y - position.Y;
			position = newPosition;
			Refresh();
		}

		private void CanvasArea_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.RightButton == MouseButtonState.Pressed)
			{
				position = e.GetPosition(CanvasArea);
				drag = true;
			}
		}

		private void CanvasArea_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (drag && e.RightButton == MouseButtonState.Released)
			{
				Point newPosition = e.GetPosition(CanvasArea);
				MoveCanvasArea(newPosition);
				drag = false;
			}
		}

		private void CanvasArea_MouseMove(object sender, MouseEventArgs e)
		{
			if (drag && (DateTime.Now - previousMove).TotalMilliseconds > 50.0)
			{
				MoveCanvasArea(e.GetPosition(CanvasArea));
				previousMove = DateTime.Now;
			}
		}

		private void CanvasArea_MouseLeave(object sender, MouseEventArgs e)
		{
			drag = false;
		}

		private void sliderEnergy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (Area != null)
				Area.TotalEnergy = sliderEnergy.Value;
		}
	}
}
