using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;
using System.Threading.Tasks;

namespace CellWorld
{
    public class CellArea : IArea
    {
		public static readonly double MONSTER_INIT_ENERGY = 10.0;
		public static readonly double FOOD_ENERGY = 5.0;
		public static readonly double MONSTER_MOVE_ENERGY = 0.1;
		public static readonly double WALL_PERCENT = 25.0;

		public static readonly double MONSTER_MASTURBATION_SEX_ENERGY = 40.0;
		public static readonly double MONSTER_ACTIVE_SEX_ENERGY = 20.0;
		public static readonly double MONSTER_PASSIVE_SEX_ENERGY = 20.0;
		
		Cell[,] cells;
		StepInfo stepInfo;

		ViewSensor viewSensor = new ViewSensor();
		MoveAction moveAction = new MoveAction();
		CollisionSensor collisionSensor = new CollisionSensor();

		public int Width { get; private set; }
		public int Height { get; private set; }

		public double UsedEnergy { get; set; }

		#region IArea Members

		public double TotalEnergy
		{
			get;
			set;
		}

		public List<IMonster> Monsters
		{
			get;
			private set;
		}

		#endregion

		public CellArea(int width, int height)
			: this(width, height, width * height) { }

        public CellArea(int width, int height, double totalEnergy)
        {
            TotalEnergy = totalEnergy;
            Width = width;
            Height = height;

            Monsters = new List<IMonster>();
            cells = new Cell[width,height];

            for (int w = 0; w < width; w++)
                for (int h = 0; h < height; h++)
                {
                    double percent = 100.0*Common.Random.NextDouble();
                    if (percent <= WALL_PERCENT)
                        cells[w, h] = new Cell {ContentType = ContentType.Wall};
                    else
                        cells[w, h] = new Cell {ContentType = ContentType.Empty};

                    cells[w, h].AreaInfo = new CellAreaInfo {X = w, Y = h, Area = this};
                    cells[w, h].ContentTypeChanged += CellArea_ContentTypeChanged;
                }

            // create wall border
			return;
            for (int w = 0; w < width; w++)
            {
                cells[w, 0].ContentType = ContentType.Wall;
                cells[w, height - 1].ContentType = ContentType.Wall;
            }

            for (int h = 0; h < height; h++)
            {
                cells[0, h].ContentType = ContentType.Wall;
                cells[width - 1, h].ContentType = ContentType.Wall;
            }
        }

        public Cell GetCellAt(int x, int y)
		{
            while (x < 0) x += Width;
            while (y < 0) y += Height;
			x %= Width;
			y %= Height;
			return cells[x, y];
		}

		public Cell GetCellAt(CellAreaInfo areaInfo)
		{
			return GetCellAt(areaInfo.X, areaInfo.Y);
		}

		public StepInfo NextStep()
		{
			stepInfo = new StepInfo();

            //for (int activeMonster = 0; activeMonster < Monsters.Count; activeMonster++)
            Parallel.For(0, Monsters.Count,  activeMonster =>
            {
                IMonster monster = Monsters[activeMonster];
                monster.NextStep(this);
                monster.Energy -= MONSTER_MOVE_ENERGY;
                UsedEnergy -= MONSTER_MOVE_ENERGY;
            });

            for (int i = 0; i < Monsters.Count; i++)
            {
                IMonster monster = Monsters[i];

                // Monster death
                if (monster.Energy < MONSTER_MOVE_ENERGY)// || Common.Random.NextDouble() < 0.000003 * monster.Age)
                {
                    UsedEnergy -= monster.Energy;
                    monster.Energy = 0.0;
                    Monsters.Remove(monster);
                    Cell cell = GetCellAt(monster.AreaInfo as CellAreaInfo);
                    cell.ContentType = ContentType.Empty;
                    i--;
                }
                // Child monster birth from single parent
                else if (monster.Energy > MONSTER_MASTURBATION_SEX_ENERGY && Common.Random.Next(50) == 48)
                {
                    CreateChildMonster(monster);
                }

                // Add meat to area
                if (UsedEnergy <= TotalEnergy - FOOD_ENERGY - MONSTER_INIT_ENERGY)
                {
                    int x = Common.Random.Next(Width);
                    int y = Common.Random.Next(Height);
                    Cell cell = cells[x, y];
                    if (cell.ContentType == ContentType.Empty)
                    {
                        cell.ContentType = ContentType.Food;
                        UsedEnergy += FOOD_ENERGY;
                    }
                }
            }

            // Add random monster to area
            if (UsedEnergy <= TotalEnergy - MONSTER_INIT_ENERGY && Common.Random.Next(100 + 10) == 21)
                GenerateMonster();

			return stepInfo;
		}

		public IMonster AddNewMonster(Genome genome)
		{
			int x, y;
			Cell cell = null;
			do
			{
				x = Common.Random.Next(Width);
				y = Common.Random.Next(Height);
				cell = cells[x, y];
			}
			while (cell.ContentType == ContentType.Monster || cell.ContentType == ContentType.Wall);

			if (cell.ContentType == ContentType.Food)
				UsedEnergy -= FOOD_ENERGY;

			UsedEnergy += MONSTER_INIT_ENERGY;

			IAreaInfo areaInfo = new CellAreaInfo { Area = this, X = x, Y = y };

			IMonster monster = new SimpleMonster.Monster(genome, areaInfo);
			monster.Energy = MONSTER_INIT_ENERGY;
			monster.Sensors.Add(viewSensor);
			monster.Sensors.Add(collisionSensor);
			monster.Actions.Add(new MoveAction());
			Monsters.Add(monster);

			cell.Content = monster;
			cell.ContentType = ContentType.Monster;

			return monster;
		}

		void CellArea_ContentTypeChanged(Cell cell, ContentType oldContentType)
		{
			if (stepInfo == null)
				return;

			stepInfo.AddCell(cell);
		}

        IMonster CreateChildMonster(IMonster parentMonster)
        {
            BrainStructure brainStructure = parentMonster.Brain.Genome.BrainStructure;
            Genome genome = new Genome(brainStructure);
            genome.Load(parentMonster.Brain.Genome);

            // Mutation
			int bitCount = Common.Random.Next(1, Sex.DEFAULT_MUTATION);
			for (int i = 0; i < bitCount; i++)
			{
				int bitToChange = Common.Random.Next(genome.Length);
				genome[bitToChange] ^= true;
			}

			parentMonster.Energy -= MONSTER_INIT_ENERGY;
			UsedEnergy -= MONSTER_INIT_ENERGY;

			IMonster monster = AddNewMonster(genome);
			(monster as SimpleMonster.Monster).Parents = (parentMonster as SimpleMonster.Monster).Parents + "M";
			return monster;
        }

		IMonster GenerateMonster()
		{
			int typeID = Common.Random.Next(0, 7);
			BrainStructure brainStructure = GetBrainStructure(typeID);

			Genome genome = new Genome(brainStructure);
			genome.SetRandomValues();

			return AddNewMonster(genome);
		}


		static BrainStructure GetBrainStructure(int typeID)
		{
			BrainStructure bs = new BrainStructure()
			{
				InputCount = 17, // sensors bits
				TypeID = typeID
			};

			switch (typeID)
			{
				case 0:
					bs.MemoryBitCount = 1;
					bs.LevelSizes = new int[] { 3 + bs.MemoryBitCount };
					break;
				case 1:
					bs.MemoryBitCount = 2;
					bs.LevelSizes = new int[] { 3 + bs.MemoryBitCount };
					break;
				case 2:
					bs.MemoryBitCount = 4;
					bs.LevelSizes = new int[] { 3 + bs.MemoryBitCount };
					break;
				case 3:
					bs.MemoryBitCount = 8;
					bs.LevelSizes = new int[] { 3 + bs.MemoryBitCount };
					break;
				case 4:
					bs.MemoryBitCount = 16;
					bs.LevelSizes = new int[] { 3 + bs.MemoryBitCount };
					break;
				case 5:
					bs.MemoryBitCount = 24;
					bs.LevelSizes = new int[] { 3 + bs.MemoryBitCount };
					break;
				case 6:
					bs.MemoryBitCount = 16;
					bs.LevelSizes = new int[] { 17 + bs.MemoryBitCount, 3 + bs.MemoryBitCount };
					break;
			}
			return bs;
		}
	}
}
