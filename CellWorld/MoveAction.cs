using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;

namespace CellWorld
{
	/// <summary>
	/// All monster actions completed by movement. 3 bits
	/// </summary>
	class MoveAction : BitStorage, IAction
	{
		static Sex sex = new Sex();

		public bool Collision { get; private set; }

		public MoveAction()
			: base(3)
		{
			Collision = false;
		}

		public void Do(IMonster monster)
		{
			CellAreaInfo areaInfo = monster.AreaInfo as CellAreaInfo;
			CellArea area = areaInfo.Area as CellArea;

			int x = areaInfo.X;
			int y = areaInfo.Y;

			int moveNr = (this[0] ? 1 : 0) + (this[1] ? 2 : 0) + (this[2] ? 4 : 0);
			switch (moveNr)
			{
				case 0: y--; break;
				case 1: x--; y--; break;
				case 2: x++; y--; break;
				case 3: x--; break;
				case 4: x++; break;
				case 5: x--; y++; break;
				case 6: x++; y++; break;
				case 7: y++; break;
			}

			Collision = false;
			Cell targetCell = area.GetCellAt(x, y);
			switch (targetCell.ContentType)
			{
				case ContentType.Empty:
					areaInfo.MoveTo(x, y);
					break;
				case ContentType.Wall:
					Collision = true;
					break;
				case ContentType.Food:
					monster.Energy += CellArea.FOOD_ENERGY;
					targetCell.ContentType = ContentType.Empty;
					break;
				case ContentType.Monster:
					if (monster.Energy < CellArea.MONSTER_ACTIVE_SEX_ENERGY)
						break;

					IMonster targetMonster = (IMonster)targetCell.Content;
					if (targetMonster.Energy < CellArea.MONSTER_PASSIVE_SEX_ENERGY)
						break;

					Genome genome = sex.MakeChild(monster.Brain.Genome, targetMonster.Brain.Genome);
					if (genome != null)
					{
						monster.Energy -= CellArea.MONSTER_INIT_ENERGY * 0.5;
						targetMonster.Energy -= CellArea.MONSTER_INIT_ENERGY * 0.5;
						area.UsedEnergy -= CellArea.MONSTER_INIT_ENERGY;
						SimpleMonster.Monster m = area.AddNewMonster(genome) as SimpleMonster.Monster;
						m.Parents = (monster as SimpleMonster.Monster).Parents + "S";
					}
					//else
					//{
					//    // Get energy from target monster
					//    targetMonster.Energy -= CellArea.FOOD_ENERGY;
					//    monster.Energy += CellArea.FOOD_ENERGY;
					//}
					
					break;
			}
		}
	}
}
