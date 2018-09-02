using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Monster;

namespace CellWorld
{
	class CollisionSensor : BitStorage, ISensor
	{
		public CollisionSensor() : base(1) { }

		public void Prepare(IMonster monster)
		{
			MoveAction moveAction = monster.Actions.SingleOrDefault(a => a.GetType() == typeof(MoveAction)) as MoveAction;
			if (moveAction != null)
				this[0] = moveAction.Collision;
			else
				this[0] = false;
		}
	}
}
