using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    class Component_MechSkeleton : Component
    {
        private static Dictionary<BodyPartLocation, int> MechTemplate = new Dictionary<BodyPartLocation, int>() {
            { BodyPartLocation.HEAD, 5 },
            { BodyPartLocation.TORSO, 15 },
            { BodyPartLocation.LEFT_ARM, 8 },
            { BodyPartLocation.RIGHT_ARM, 8 },
            { BodyPartLocation.LEFT_LEG, 10 },
            { BodyPartLocation.RIGHT_LEG, 10 }
        };
        private Dictionary<BodyPartLocation, Entity> bodyParts;

        public Component_MechSkeleton()
        {
            this.bodyParts = new Dictionary<BodyPartLocation, Entity>();

            foreach(var bp in Enum.GetValues(typeof(BodyPartLocation)).Cast<BodyPartLocation>())
            {
                this.bodyParts[bp] = EntityBuilder.BuildBodyPart(bp, MechTemplate[bp], MechTemplate[bp]);
            }
        }

        // We have NO DAMAGE TRANSFER! A mech with no arm is just harder to hit.
        private Entity FindBodyPartToDamage()
        {
            BodyPartLocation damagedLocation = GameRandom.RandomByWeight(MechTemplate, (a => a.Value)).Key;
            return this.bodyParts[damagedLocation];
        }

        // Since there is no damage transfer, will *always* complete the event!
        private void HandleTakeDamage(GameEvent_TakeDamage ev)
        {
            Entity damagedPart = this.FindBodyPartToDamage();
            if (damagedPart != null)
                damagedPart.HandleEvent(ev);

            ev.Completed = true;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_TakeDamage)
                this.HandleTakeDamage((GameEvent_TakeDamage)ev);

            return ev;
        }
    }
}
