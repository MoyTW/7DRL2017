using RogueSharp.Random;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor
{
    [Serializable()]
    public class Component_Skeleton : Component_TracksTime
    {
        private static Dictionary<BodyPartLocation, int> MechTemplate = new Dictionary<BodyPartLocation, int>() {
            { BodyPartLocation.HEAD, 2 },
            { BodyPartLocation.TORSO, 6 },
            { BodyPartLocation.LEFT_ARM, 3 },
            { BodyPartLocation.RIGHT_ARM, 3 },
            { BodyPartLocation.LEFT_LEG, 4 },
            { BodyPartLocation.RIGHT_LEG, 4 }
        };

        private Dictionary<BodyPartLocation, Entity> bodyParts;

        public bool IsHeadDestroyed { get; private set; }
        public bool IsTorsoDestroyed { get; private set; }
        private bool IsKilled
        {
            get
            {
                return this.IsHeadDestroyed || this.IsTorsoDestroyed;
            }
        }

        public static IEnumerable<BodyPartLocation> TemplateLocations
        {
            get
            {
                return Component_Skeleton.MechTemplate.Select(t => t.Key);
            }
        }

        public Component_Skeleton() : base(EntityAttributeType.SPEED)
        {
            this.bodyParts = new Dictionary<BodyPartLocation, Entity>();
            // Not technically required, as these will default initialize to false
            this.IsHeadDestroyed = false;
            this.IsTorsoDestroyed = false;

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

        // ISSUE: If damage is done through any alternative avenue and the damage state is rendered dirty, then this
        // won't pick up on it! You have to make sure to re-call this function after ANY damage is dealt to the mech.
        private void AssessDamage()
        {
            this.IsHeadDestroyed = this.Parent.TryGetSubEntities(SubEntitiesSelector.BODY_PART)
                .Where(e => e.GetComponentOfType<Component_BodyPartLocation>().Location == BodyPartLocation.HEAD)
                .First()
                .TryGetDestroyed();
            this.IsTorsoDestroyed = this.Parent.TryGetSubEntities(SubEntitiesSelector.BODY_PART)
                .Where(e => e.GetComponentOfType<Component_BodyPartLocation>().Location == BodyPartLocation.TORSO)
                .First()
                .TryGetDestroyed();
            if (this.IsKilled)
                this.Parent.HandleEvent(new GameEvent_Destroy());
        }

        // All Attack logic currently handled here; might want to put it into its own class for explicit-ness.
        private void HandleReceiveAttack(GameEvent_ReceiveAttack ev)
        {
            if (ev.Target != this.Parent)
                return;
            
            int weaponBaseDamage = ev.Attacker.TryGetAttribute(EntityAttributeType.DAMAGE, ev.Weapon).Value;

            int damage = weaponBaseDamage; // Possible damage modifiers

            Entity subTargetEntity = this.bodyParts[ev.SubTarget];

            // This is all damage handling
            if (subTargetEntity.TryGetDestroyed())
                ev.RegisterAttackResults("MISS (limb missing)");
            else
            {
                subTargetEntity.HandleEvent(new GameEvent_TakeDamage(damage));

                if (0 >= subTargetEntity.TryGetAttribute(EntityAttributeType.STRUCTURE).Value)
                    ev.RegisterAttackResults("HIT (" + damage + " dmg, destroyed limb)");
                else
                    ev.RegisterAttackResults("HIT (" + damage + " dmg)");
            }

            this.AssessDamage();
        }

        // Since there is no damage transfer, will *always* complete the event!
        private void HandleTakeDamage(GameEvent_TakeDamage ev)
        {
            throw new NotImplementedException();
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            // TODO: Move off inheritance
            base._HandleEvent(ev);
            if (ev is GameEvent_ReceiveAttack)
                this.HandleReceiveAttack((GameEvent_ReceiveAttack)ev);
            else if (ev is GameEvent_TakeDamage)
                this.HandleTakeDamage((GameEvent_TakeDamage)ev);
            else if (ev is GameEvent_MoveSingle)
                return ev;

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
                q.RegisterBaseValue(1);
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

        // A mech is only considered "Destroyed" when its torso is gone!
        private void HandleQueryDestroyed(GameQuery_Destroyed q)
        {
            if (this.IsKilled)
                q.RegisterDestroyed();
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            // TODO: Move off inheritance
            base._HandleQuery(q);
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
