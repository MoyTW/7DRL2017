using System;
using System.Collections.Generic;

namespace MechArena.Mech
{
	public enum BodyPartLocations
	{
		HEAD=0,
		TORSO,
		LEFT_ARM,
		RIGHT_ARM,
		LEFT_LEG,
		RIGHT_LEG
	}

	public class BodyPart : GameObject
	{
		#region Variables & Properties

		private BodyPartLocations location;
		private int slotSpace;
		private List<Attachment> attachments;
		private int OpenSlots { get { return this.slotSpace - this.SlotsUsed; } }

		public BodyPartLocations Location { get { return this.location; } }
		// For some reason, it doesn't include LINQ with MonoDevelop!? Uh. When I get internet back I'll look.
		public int SlotsUsed
		{
			get
			{
				int slotsUsed = 0;
				for (int i = 0; i < this.attachments.Count; i++)
				{
					slotsUsed += this.attachments [i].SlotsUsed;
				}
				return slotsUsed;
			}
		}

		#endregion

		public BodyPart (BodyPartLocations location, int slotSpace)
		{
			this.location = location;
			this.slotSpace = slotSpace;
			this.attachments = new List<Attachment> ();
		}

		public bool CanAttach(Attachment attachment)
		{
			return this.OpenSlots < attachment.SlotsUsed;
		}

		public bool TryAttach(Attachment attachment)
		{
            if (attachment == null)
                throw new ArgumentException("Cannot attach null attachment!");
            if (this.attachments.Contains(attachment))
                throw new ArgumentException("Cannout attach attached item " + attachment.ToString() + "!");

            if (this.CanAttach(attachment))
            {
                this.attachments.Add(attachment);
                return true;
            }
            else
            {
                return false;
            }
		}

		public bool TryDetach(Attachment attachment)
		{
            if (attachment == null)
                throw new ArgumentException("Cannot detach null attachment!");
            if (!this.attachments.Contains(attachment))
                throw new ArgumentException("Cannot detach " + attachment.ToString() + " as it is not attached!");

            this.attachments.Remove(attachment);
            return true;
		}

		private bool TryTakeDamage()
		{
			throw new NotImplementedException ();
		}
	}
}
