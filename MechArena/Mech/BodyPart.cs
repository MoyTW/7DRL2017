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

	public class MechBodyPart : GameObject, Stateful
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

		public MechBodyPart (BodyPartLocations location, int slotSpace)
		{
			this.location = location;
			this.slotSpace = slotSpace;
			this.attachments = new List<Attachment> ();
		}

		public bool CanAttach(Attachment attachment)
		{
			return this.OpenSlots < attachment.SlotsUsed;
		}

		private bool TryAttachPart(StateChange change)
		{
			throw new NotImplementedException ();
		}

		private bool TryDetachPart(StateChange change)
		{
			throw new NotImplementedException ();
		}

		private bool TryTakeDamage(StateChange change)
		{
			throw new NotImplementedException ();
		}

		public virtual bool TryApplyStateChange(StateChange change)
		{
			switch (change.ChangeType)
			{
			case StateChangeType.PART_ATTACH:
				return this.TryAttachPart (change);
			case StateChangeType.PART_DETATCH:
				return this.TryDetachPart (change);
			case StateChangeType.TAKE_DAMAGE:
				return this.TryTakeDamage (change);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
