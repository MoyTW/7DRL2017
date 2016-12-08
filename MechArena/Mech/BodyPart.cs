using System;
using System.Linq;
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
		public int SlotsUsed { get { return this.attachments.Sum(i => i.SlotsUsed); } }
        public int SlotsRemaining { get { return this.slotSpace - this.SlotsUsed; } }

        public IList<Attachment> InspectAttachments()
        {
            return attachments.AsReadOnly();
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
			return this.OpenSlots > attachment.SlotsUsed;
		}

		public void TryAttach(Attachment attachment)
		{
            if (attachment == null)
                throw new ArgumentException("Cannot attach null attachment!");
            else if (this.attachments.Contains(attachment))
                throw new ArgumentException("Cannot attach attached item " + attachment.ToString() + "!");
            else if (!this.CanAttach(attachment))
                throw new ArgumentException("Cannot attach item " + attachments.ToString() + ", too few slots!");
            else
                this.attachments.Add(attachment);
		}

		public void Detach(Attachment attachment)
		{
            if (attachment == null)
                throw new ArgumentException("Cannot detach null attachment!");
            else if (!this.attachments.Contains(attachment))
                throw new ArgumentException("Cannot detach " + attachment.ToString() + " as it is not attached!");
            else
                this.attachments.Remove(attachment);
		}

		private bool TryTakeDamage()
		{
			throw new NotImplementedException ();
		}
	}
}
