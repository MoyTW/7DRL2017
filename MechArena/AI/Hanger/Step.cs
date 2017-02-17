using MechArena;
using System;

namespace MechArena.AI.Hanger
{
	public abstract class Step : SingleClause
	{
		public abstract void ApplyStep(Entity entity);
	}
}

