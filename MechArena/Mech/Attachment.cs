using System;

namespace MechArena.Mech
{
	public class Attachment
	{
		private int slotsUsed;

		public Attachment (int slotsRequired)
		{
			this.slotsUsed = slotsRequired;
		}

		public int SlotsUsed { get { return this.slotsUsed; } }
	}
}

