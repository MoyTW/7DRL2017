using System;

namespace MechArena.Mech
{
	public class Attachment
	{
		private int slotsUsed;

		public Attachment (int slotsRequired)
		{
            if (slotsRequired < 1)
            {
                throw new ArgumentException("Attachments cannot have 0 slots required!");
            }
			this.slotsUsed = slotsRequired;
		}

		public int SlotsUsed { get { return this.slotsUsed; } }
	}
}

