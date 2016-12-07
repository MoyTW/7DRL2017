using System;

namespace MechArena
{
	public enum StateChangeType
	{
		PART_ATTACH = 0,
		PART_DETATCH,
		TAKE_DAMAGE
	}

	public class StateChange 
	{
		private StateChangeType changeType;

		public StateChangeType ChangeType { get { return this.changeType; } }
	}
}

