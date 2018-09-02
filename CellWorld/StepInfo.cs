using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;

namespace CellWorld
{
	public class StepInfo
	{
		public List<Cell> ChangedCells
		{
			get;
			private set;
		}

		public void AddCell(Cell cell)
		{
			if (!ChangedCells.Contains(cell))
				ChangedCells.Add(cell);
		}

		public StepInfo()
		{
			ChangedCells = new List<Cell>();
		}
	}
}
