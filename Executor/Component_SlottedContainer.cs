using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Executor
{
    [Serializable()]
    public class Component_SlottedContainer : Component
    {
        private int slotsMax;
        private List<Entity> storedEntities;
        
        public int SlotsMax { get { return this.slotsMax; } }
        public int SlotsUsed
        {
            get
            {
                return this.storedEntities.Sum(c => c.GetComponentOfType<Component_Slottable>().SlotsRequired);
            }
        }
        public int SlotsRemaining { get { return this.SlotsMax - this.SlotsUsed; } }

        public Component_SlottedContainer(int slotSpace)
        {
            this.slotsMax = slotSpace;
            this.storedEntities = new List<Entity>();
        }

        public IList<Entity> InspectStoredEntities()
        {
            return this.storedEntities.AsReadOnly();
        }

        public bool CanSlot(Entity en)
        {
            var cs = en.GetComponentOfType<Component_Slottable>();
            if (cs != null && this.SlotsRemaining >= cs.SlotsRequired)
                return true;
            else
                return false;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleSlot(GameEvent_Slot ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (this.storedEntities.Contains(ev.EntityToSlot))
                    throw new ArgumentException("Cannot attach attached item " + ev.EntityToSlot.ToString());

                if (this.CanSlot(ev.EntityToSlot))
                {
                    this.storedEntities.Add(ev.EntityToSlot);
                    ev.Completed = true;
                }
                else
                    throw new InvalidOperationException("Can't slot item " + ev.EntityToSlot + " in " + this.Parent);
            }
        }

        private void HandleUnslot(GameEvent_Unslot ev)
        {
            if (ev.ExecutorEntity == this.Parent)
            {
                if (!this.storedEntities.Contains(ev.EntityToUnslot))
                    throw new ArgumentException("Cannot detach unattached item " + ev.EntityToUnslot.ToString() + "!");

                this.storedEntities.Remove(ev.EntityToUnslot);
                ev.Completed = true;
            }
        }

        protected override GameEvent _HandleEvent(GameEvent ev)
        {
            if (ev is GameEvent_Slot)
                this.HandleSlot((GameEvent_Slot)ev);
            else if (ev is GameEvent_Unslot)
                this.HandleUnslot((GameEvent_Unslot)ev);

            return ev;
        }

        private void HandleQuerySubEntities(GameQuery_SubEntities q)
        {
            foreach (var stored in this.storedEntities)
            {
                if (stored != null)
                {
                    if (q.MatchesSelectors(stored))
                        q.RegisterEntity(stored);
                    stored.HandleQuery(q);
                }
            }
        }

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            foreach (Entity e in this.storedEntities.Where(e => !e.TryGetDestroyed()))
            {
                e.HandleQuery(q);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);
            else if (q is GameQuery_SubEntities)
                this.HandleQuerySubEntities((GameQuery_SubEntities)q);

            return q;
        }
    }
}
