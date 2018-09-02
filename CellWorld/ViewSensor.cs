using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;

namespace CellWorld
{
	/// <summary>
	/// Monster eyes. View 8 cells around monster. 2 bits per cell. 16bit
	/// </summary>
	class ViewSensor : BitStorage, ISensor
	{
		public ViewSensor()
			: base(16)
		{
		}

		public void Prepare(IMonster monster)
		{
			CellAreaInfo areaInfo = monster.AreaInfo as CellAreaInfo;
			CellArea area = areaInfo.Area as CellArea;
			int x = areaInfo.X;
			int y = areaInfo.Y;

			int startBit = 0;		
			startBit = SetContentType(startBit, area.GetCellAt(x, y - 1).ContentType);
			startBit = SetContentType(startBit, area.GetCellAt(x - 1, y - 1).ContentType);
			startBit = SetContentType(startBit, area.GetCellAt(x + 1, y - 1).ContentType);
			startBit = SetContentType(startBit, area.GetCellAt(x - 1, y).ContentType);
			startBit = SetContentType(startBit, area.GetCellAt(x + 1, y).ContentType);
			startBit = SetContentType(startBit, area.GetCellAt(x - 1, y + 1).ContentType);
			startBit = SetContentType(startBit, area.GetCellAt(x + 1, y + 1).ContentType);
			startBit = SetContentType(startBit, area.GetCellAt(x, y + 1).ContentType);
		}

		int SetContentType(int startBit, ContentType contentType)
		{
			switch (contentType)
			{
				case ContentType.Empty:
					this[startBit] = false;
					this[startBit + 1] = false;
					break;
				case ContentType.Food:
					this[startBit] = true;
					this[startBit + 1] = false;
					break;
				case ContentType.Monster:
					this[startBit] = false;
					this[startBit + 1] = true;
					break;
				case ContentType.Wall:
					this[startBit] = true;
					this[startBit + 1] = true;
					break;
			}
			return startBit + 2;
		}
	}
}
