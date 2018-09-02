using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CellWorld
{
	public enum ContentType
	{
		Empty 	= 0,
		Food 	= 1,
		Monster = 2,
		Wall 	= 3
	}

	public delegate void ContentTypeChangedDelegate(Cell cell, ContentType oldContentType);

	public class Cell
	{
		ContentType contentType;

		public event ContentTypeChangedDelegate ContentTypeChanged;

		public ContentType ContentType
		{
			get { return contentType; }
			set
			{
				if (contentType != value)
				{
					ContentType oldContentType = contentType;
					contentType = value;

					if (ContentTypeChanged != null)
						ContentTypeChanged(this, oldContentType);
				}
			}
		}

		public object Content
		{
			get;
			set;
		}

		public CellAreaInfo AreaInfo
		{
			get;
			set;
		}
	}
}
