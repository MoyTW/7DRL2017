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

            foreach(var bp in MechTemplate.Keys)
            {
                this.bodyParts[bp] = EntityBuilder.BuildBodyPart(bp, MechTemplate[bp], MechTemplate[bp]);
            }
        }

        #region Event Handlers

        // We have NO DAMAGE TRANSFER! A mech with no arm is just harder to hit.
        private BodyPartLocation FindBodyPartLocationByWeight()
        {
            return GameRandom.RandomByWeight(MechTemplate, (a => a.Value)).Key;
        }

        // All Attack logic currently handled here; might want to put it into its own class for explicit-ness.
        private void HandleAttack(GameEvent_Attack ev)
        {
            if (ev.Target != this.Parent)
                return;

            int attackerBaseToHit = ev.Attacker.TryGetAttribute(EntityAttributeType.TO_HIT).Value;
            int weaponBaseDamage = ev.Weapon.TryGetAttribute(EntityAttributeType.DAMAGE).Value;

            // Resolve pilot skills here
            // Get pilot skill modifiers for attacker
            // Get pilot skill modifiers for defender

            int targetDodge = ev.Target.TryGetAttribute(EntityAttributeType.DODGE).Value;

            int roll = GameRandom.Next(1, 20);
            int toHit = attackerBaseToHit;
            int dodge = targetDodge;

            if (roll + toHit > 10 + dodge)
            {
                int damage = weaponBaseDamage; // Possible damage modifiers
                Console.WriteLine(String.Format("Attack by {0} hit {1} for {2} damage!", ev.Attacker, ev.Target,
                    damage));

                // Retarget on appropriate body part
                if (ev.SubTarget == BodyPartLocation.ANY)
                    ev.SubTarget = this.FindBodyPartLocationByWeight();

                Entity subTargetEntity = this.bodyParts[ev.SubTarget];

                // This is all damage handling
                if (subTargetEntity.TryGetDestroyed().Destroyed)
                {
                    Console.WriteLine(
                        String.Format("Attack by {0} missed - the {1} of the target was already destroyed!",
                        ev.Attacker, ev.SubTarget));
                }
                else
                {
                    subTargetEntity.HandleEvent(new GameEvent_TakeDamage(damage));

                    // Detach body part from mech if destroyed
                    if (0 >= subTargetEntity.TryGetAttribute(EntityAttributeType.STRUCTURE).Value)
                    {
                        Console.WriteLine(String.Format("Part {0} was destroyed!", ev.SubTarget));
                    }
                }
            }
            else
            {
                Console.WriteLine(String.Format("Attack by {0} missed {1}!", ev.Attacker, ev.Target));
            }

            ev.Completed = true;
        }

        // Since there is no damage transfer, will *always* complete the event!
        private void HandleTakeDamage(GameEvent_TakeDamage ev)
        {
            Entity damagedPart = this.bodyParts[this.FindBodyPartLocationByWeight()];
            if (damagedPart != null)
                damagedPart.HandleEvent(ev);

            ev.Completed = true;
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Attack)
                this.HandleAttack((GameEvent_Attack)ev);
            if (ev is GameEvent_TakeDamage)
                this.HandleTakeDamage((GameEvent_TakeDamage)ev);

            return ev;
        }

        #endregion

        #region Query Handlers

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            foreach(var part in this.bodyParts.Values)
            {
                if (part != null)
                    part.HandleQuery(q);
            }
        }

        private void HandleQuerySubEntities(GameQuery_SubEntities q)
        {
            foreach(var part in this.bodyParts.Values)
            {
                if (part != null)
                {
                    if (q.MatchesSelectors(part))
                        q.RegisterEntity(part);
                    part.HandleQuery(q);
                }
            }
        }

        // A mech is only considered "Destroyed" when its torso is gone!
        private void HandleQueryDestroyed(GameQuery_Destroyed q)
        {
            this.bodyParts[BodyPartLocation.TORSO].HandleQuery(q);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);
            else if (q is GameQuery_SubEntities)
                this.HandleQuerySubEntities((GameQuery_SubEntities)q);
            else if (q is GameQuery_Destroyed)
                this.HandleQueryDestroyed((GameQuery_Destroyed)q);

            return q;
        }

        #endregion
    }
}
