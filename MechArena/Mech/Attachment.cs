using System;
using System.Collections.Generic;

namespace MechArena.Mech
{
    public class Attachment
	{
        private string label;
		private int slotsUsed;
        private int sizeMax;
        private int damageTaken;
        private AttachmentEffect effect;

        public string Label { get { return this.label; } }
        public int SlotsUsed { get { return this.slotsUsed; } }

        public int SizeMax { get { return this.sizeMax; } }
        public int SizeCurrent { get { return this.sizeMax - this.damageTaken; } }
        public float PercentDamaged { get { return this.SizeCurrent / this.SizeMax; } }
        public bool Destroyed { get { return this.SizeCurrent <= 0; } }

        public Attachment (string label, int slotsRequired, int sizeMax, AttachmentEffect effect)
		{
            if (slotsRequired < 1)
                throw new ArgumentException("Attachments cannot have 0 slots required!");

            this.label = label;
			this.slotsUsed = slotsRequired;
            this.sizeMax = sizeMax;
            this.damageTaken = 0;
            this.effect = effect;
		}

        public bool HasEffect(AttachmentEffectType effectType)
        {
            return this.effect.EffectType == effectType;
        }

        public AttachmentEffect InspectEffect(AttachmentEffectType effectType)
        {
            if (this.HasEffect(effectType))
                return this.effect;
            else
                throw new ArgumentException("Can't inspect effect of different type!");
        }

        public void TakeDamage()
        {
            if (this.Destroyed)
                throw new ArgumentException("Cannot take damage to a destroyed attachment!");
            this.damageTaken++;
        }
	}
}

