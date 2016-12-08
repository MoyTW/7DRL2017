using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena.Mech
{
    public enum AttachmentEffectType
    {
        SENSOR_PLUS = 0,
        POWER_PLANT,
        ARM_ACTUATOR,
        LEG_ACTUATOR,
        UTILITY_MOUNT,
        HOLSTER_MOUNT,
        WEAPON_MOUNT,
        ARMOR,
        BOOSTER,
        ACCELERATOR
    }

    public class AttachmentEffect
    {
        private AttachmentEffectType effectType;

        public AttachmentEffectType EffectType { get { return this.effectType; } }

        public AttachmentEffect(AttachmentEffectType effectType)
        {
            this.effectType = effectType;
        }

        /* Damage Stuff For Later
        private float damagePercent;

        public void SetDamaged(float damagePercent)
        {
            this.damagePercent = damagePercent;
            this.WasDamaged(damagePercent);
        }

        protected virtual void WasDamaged(float damagePercent)
        {
            throw new NotImplementedException();
        }
        */
    }
}
