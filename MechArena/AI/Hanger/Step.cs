using Executor;
using System;

namespace Executor.AI.Hanger
{
	public abstract class Step : SingleClause
	{
		public abstract void ApplyStep(Entity entity);
	}
}

