using RogueSharp.Random;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechArena
{
    class Component_MechSkeleton : Component_TracksTime
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

        public Component_MechSkeleton() : base(EntityAttributeType.SPEED)
        {
            this.bodyParts = new Dictionary<BodyPartLocation, Entity>();

            foreach(var bp in MechTemplate.Keys)
            {
                this.bodyParts[bp] = EntityBuilder.BuildBodyPart(bp, MechTemplate[bp], MechTemplate[bp]);
            }
        }

        #region Event Handlers

        // We have NO DAMAGE TRANSFER! A mech with no arm is just harder to hit.
        private BodyPartLocation FindBodyPartLocationByWeight(IRandom rand)
        {
            return rand.RandomByWeight(MechTemplate, (a => a.Value)).Key;
        }

        // All Attack logic currently handled here; might want to put it into its own class for explicit-ness.
        private void HandleAttack(GameEvent_Attack ev)
        {
            if (ev.Target != this.Parent)
                return;

            int attackerBaseToHit = ev.CommandEntity.TryGetAttribute(EntityAttributeType.TO_HIT).Value;
            int weaponBaseDamage = ev.ExecutorEntity.TryGetAttribute(EntityAttributeType.DAMAGE).Value;

            // Resolve pilot skills here
            // Get pilot skill modifiers for attacker
            // Get pilot skill modifiers for defender

            int targetDodge = ev.Target.TryGetAttribute(EntityAttributeType.DODGE).Value;

            int roll = ev.Rand.Next(1, 20);
            int toHit = attackerBaseToHit;
            int dodge = targetDodge;

            if (roll + toHit > 10 + dodge)
            {
                int damage = weaponBaseDamage; // Possible damage modifiers
                Console.WriteLine(String.Format("Attack by {0} hit {1} for {2} damage!", ev.CommandEntity, ev.Target,
                    damage));

                // Retarget on appropriate body part
                if (ev.SubTarget == BodyPartLocation.ANY)
                    ev.SubTarget = this.FindBodyPartLocationByWeight(ev.Rand);

                Entity subTargetEntity = this.bodyParts[ev.SubTarget];

                // This is all damage handling
                if (subTargetEntity.TryGetDestroyed())
                {
                    Console.WriteLine(
                        String.Format("Attack by {0} missed - the {1} of the target was already destroyed!",
                        ev.CommandEntity, ev.SubTarget));
                }
                else
                {
                    subTargetEntity.HandleEvent(new GameEvent_TakeDamage(damage, ev.Rand));

                    // Detach body part from mech if destroyed
                    if (0 >= subTargetEntity.TryGetAttribute(EntityAttributeType.STRUCTURE).Value)
                    {
                        Console.WriteLine(String.Format("Part {0} was destroyed!", ev.SubTarget));
                    }
                }
            }
            else
            {
                Console.WriteLine(String.Format("Attack by {0} missed {1}!", ev.CommandEntity, ev.Target));
            }

            ev.ExecutorEntity.GetComponentOfType<Component_TracksTime>().RegisterActivated(ev.CurrentTick);
            ev.Completed = true;
        }

        // Since there is no damage transfer, will *always* complete the event!
        private void HandleTakeDamage(GameEvent_TakeDamage ev)
        {
            Entity damagedPart = this.bodyParts[this.FindBodyPartLocationByWeight(ev.Rand)];
            if (damagedPart != null)
                damagedPart.HandleEvent(ev);

            ev.Completed = true;
        }

        private void HandleMoveSingle(GameEvent_MoveSingle ev)
        {
            this.RegisterActivated(ev.CurrentTick);
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            // TODO: Move off inheritance
            base._HandleEvent(ev);
            if (ev is GameEvent_Attack)
                this.HandleAttack((GameEvent_Attack)ev);
            else if (ev is GameEvent_TakeDamage)
                this.HandleTakeDamage((GameEvent_TakeDamage)ev);
            else if (ev is GameEvent_MoveSingle)
                this.HandleMoveSingle((GameEvent_MoveSingle)ev);

            return ev;
        }

        #endregion

        #region Query Handlers

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            foreach(var part in this.bodyParts.Values)
            {
                if (part != null && !part.TryGetDestroyed())
                    part.HandleQuery(q);
            }
            if (q.AttributeType == EntityAttributeType.SPEED)
            {
                // TOOD: Base speed not hardcoded to 50!
                q.AddFlatModifier(50, this.Parent);
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

        private void HandleQueryNextTimeTracker(GameQuery_NextTimeTracker q)
        {
            var timeTrackers = this.Parent.TryGetSubEntities(SubEntitiesSelector.TRACKS_TIME);
            foreach(var tracker in timeTrackers)
            {
                q.RegisterEntity(tracker);
            }
        }

        // A mech is only considered "Destroyed" when its torso is gone!
        private void HandleQueryDestroyed(GameQuery_Destroyed q)
        {
            this.bodyParts[BodyPartLocation.TORSO].HandleQuery(q);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            // TODO: Move off inheritance
            base._HandleQuery(q);
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);
            else if (q is GameQuery_SubEntities)
                this.HandleQuerySubEntities((GameQuery_SubEntities)q);
            else if (q is GameQuery_NextTimeTracker)
                this.HandleQueryNextTimeTracker((GameQuery_NextTimeTracker)q);
            else if (q is GameQuery_Destroyed)
                this.HandleQueryDestroyed((GameQuery_Destroyed)q);

            return q;
        }

        #endregion
    }
}
