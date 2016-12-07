using System;

namespace MechArena
{
	public interface Stateful
	{
		bool TryApplyStateChange (StateChange change);
	}
}

