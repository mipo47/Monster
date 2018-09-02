using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Monster
{
	public class Genome : BitStorage
	{
		public BrainStructure BrainStructure
		{
			get;
			private set;
		}

		public Genome(BrainStructure brainStructure)
			: base(brainStructure.GenomeLength)
		{
			BrainStructure = brainStructure;
		}

		public Genome(XDocument info)
			: base(int.Parse(info.Root.Attribute(XName.Get("Length")).Value))
		{
			// Loading brain structure
			XElement structure = info.Root.Element(XName.Get("BrainStructure"));
			BrainStructure = new BrainStructure()
			{
				InputCount = int.Parse(structure.Attribute(XName.Get("InputCount")).Value),
				TypeID = int.Parse(structure.Attribute(XName.Get("TypeID")).Value),
				MemoryBitCount = int.Parse(structure.Attribute(XName.Get("MemoryBitCount")).Value)
			};

			var levels = structure.Element(XName.Get("Levels")).Elements(XName.Get("Level"));
			BrainStructure.LevelSizes = new int[levels.Count()];
			int levelNr = 0;
			foreach (XElement level in levels)
				BrainStructure.LevelSizes[levelNr++] = int.Parse(level.Value);


			// Loading genome
			Load(info.Root.Element(XName.Get("Data")).Value);
		}

		public XDocument GetXml()
		{
			XElement levels = new XElement(XName.Get("Levels"));
			foreach (int level in BrainStructure.LevelSizes)
				levels.Add(new XElement(XName.Get("Level"), level));

			XDocument doc = new XDocument(
				new XElement(XName.Get("Genome"),
					new XAttribute(XName.Get("Length"), Length),
					new XElement(XName.Get("BrainStructure"),
						new XAttribute(XName.Get("TypeID"), BrainStructure.TypeID),
						new XAttribute(XName.Get("InputCount"), BrainStructure.InputCount),
						new XAttribute(XName.Get("OutputCount"), BrainStructure.OutputCount),
						new XAttribute(XName.Get("MemoryBitCount"), BrainStructure.MemoryBitCount),
						levels
					),
					new XElement("Data", this.GetView())
				)
			);
			return doc;
		}
	}
}
