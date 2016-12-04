using System;

namespace MechArena
{
	public class MechAttachment
	{
		private int slotsUsed;

		public MechAttachment (int slotsRequired)
		{
			this.slotsUsed = slotsRequired;
		}

		public int SlotsUsed { get { return this.slotsUsed; } }
	}
}

