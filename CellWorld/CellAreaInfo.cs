using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;

namespace CellWorld
{
	public class CellAreaInfo : IAreaInfo
	{
		public static bool operator ==(CellAreaInfo cai1, CellAreaInfo cai2)
		{
			return cai1.X == cai2.X && cai1.Y == cai2.Y;
		}

		public static bool operator !=(CellAreaInfo cai1, CellAreaInfo cai2)
		{
			return !(cai1 == cai2);
		}

		public override bool Equals(object obj)
		{
			return (CellAreaInfo)obj == this;
		}

		public override int GetHashCode()
		{
			return X & (Y << 16);
		}

        public int X
        {
            get;
            set;
        }

		public int Y
		{
			get;
			set;
		}

        public void MoveTo(int x, int y)
        {
            CellArea cellArea = Area as CellArea;
            while (x < 0) x += cellArea.Width;
            while (y < 0) y += cellArea.Height;
            x %= cellArea.Width;
            y %= cellArea.Height;

            if (x == X && y == Y)
                return;
            
            Cell destinationCell = cellArea.GetCellAt(x, y);
            if (destinationCell.ContentType != ContentType.Empty)
                return;

            Cell currentCell = cellArea.GetCellAt(this);

			destinationCell.Content = currentCell.Content;
			destinationCell.ContentType = currentCell.ContentType;
			currentCell.Content = null;
			currentCell.ContentType = ContentType.Empty;
        
            X = x;
            Y = y;
        }

		public CellAreaInfo Clone()
		{
			return new CellAreaInfo
			{
				X = X,
				Y = Y,
				Area = Area
			};
		}

        public override string ToString()
        {
            return "(" + X + ", " + Y + ')';
        }

		#region IAreaInfo Members

		public IArea Area
		{
			get;
			set;
		}

		#endregion
	}
}
