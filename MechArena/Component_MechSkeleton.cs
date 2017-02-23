using RogueSharp.Random;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena
{
    [Serializable()]
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

        public static IEnumerable<BodyPartLocation> TemplateLocations
        {
            get
            {
                return Component_MechSkeleton.MechTemplate.Select(t => t.Key);
            }
        }

        public Component_MechSkeleton() : base(EntityAttributeType.SPEED)
        {
            this.bodyParts = new Dictionary<BodyPartLocation, Entity>();

            foreach(var bp in MechTemplate.Keys)
            {
                this.bodyParts[bp] = EntityBuilder.BuildBodyPart(bp, MechTemplate[bp], MechTemplate[bp]);
            }
        }

        public Entity InspectBodyPart(BodyPartLocation location)
        {
            if (this.bodyParts.ContainsKey(location))
                return this.bodyParts[location];
            else
                return null;
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

            int attackerBaseToHit = ev.CommandEntity.TryGetAttribute(EntityAttributeType.TO_HIT, ev.ExecutorEntity)
                .Value;
            int weaponBaseDamage = ev.CommandEntity.TryGetAttribute(EntityAttributeType.DAMAGE, ev.ExecutorEntity)
                .Value;

            int targetDodge = ev.Target.TryGetAttribute(EntityAttributeType.DODGE).Value;

            int roll = ev.Rand.Next(1, 20);
            int toHit = attackerBaseToHit;
            int dodge = targetDodge;

            Log.DebugLine(String.Format("{0} attacked {1} - {2} roll+toHit v. {3} dodge, hit? {4}", ev.ExecutorEntity,
                ev.SubTarget, roll + toHit, 10 + dodge, roll + toHit > 10 + dodge));

            if (roll + toHit > 10 + dodge)
            {
                int damage = weaponBaseDamage; // Possible damage modifiers

                // Retarget on appropriate body part
                if (ev.SubTarget == BodyPartLocation.ANY)
                    ev.RegisterRetargeting(this.FindBodyPartLocationByWeight(ev.Rand));

                Entity subTargetEntity = this.bodyParts[ev.SubTarget];

                // This is all damage handling
                if (subTargetEntity.TryGetDestroyed())
                    ev.RegisterAttackResults(false, missedDueToMissingBodyPart: true);
                else
                {
                    subTargetEntity.HandleEvent(new GameEvent_TakeDamage(damage, ev.Rand));

                    if (0 >= subTargetEntity.TryGetAttribute(EntityAttributeType.STRUCTURE).Value)
                        ev.RegisterAttackResults(true, damage: damage, destroyedBodyPart: true);
                    else
                        ev.RegisterAttackResults(true, damage: damage);
                }
            }
            else
            {
                ev.RegisterAttackResults(false);
            }
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

        private void HandleCommand(GameEvent_Command ev)
        {
            var executor = this.Parent.TryGetSubEntities(SubEntitiesSelector.ACTIVE_TRACKS_TIME)
                .Where(e => e == ev.ExecutorEntity)
                .FirstOrDefault();
            if (executor != null)
                executor.HandleEvent(ev);
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
            else if (ev is GameEvent_Command)
                this.HandleCommand((GameEvent_Command)ev);

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
            // TODO: Do not hardcode these values!
            if (q.AttributeType == EntityAttributeType.SPEED)
                q.RegisterBaseValue(50);
            else if (q.AttributeType == EntityAttributeType.DODGE)
                q.RegisterBaseValue(0);
            else if (q.AttributeType == EntityAttributeType.STRUCTURE)
                q.RegisterBaseValue(0);
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
            var timeTrackers = this.Parent.TryGetSubEntities(SubEntitiesSelector.ACTIVE_TRACKS_TIME);
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
