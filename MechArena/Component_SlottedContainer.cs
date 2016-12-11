using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena
{
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

        private void HandleSlot(GameEvent_Slot ev)
        {
            if (ev.EntityContainer == this.Parent)
            {
                if (this.storedEntities.Contains(ev.EntityToSlot))
                    throw new ArgumentException("Cannot attach attached item " + ev.EntityToSlot.ToString());

                if (this.CanSlot(ev.EntityToSlot))
                {
                    this.storedEntities.Add(ev.EntityToSlot);
                    ev.Completed = true;
                }
            }
        }

        private void HandleUnslot(GameEvent_Unslot ev)
        {
            if (ev.EntityContainer == this.Parent)
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

        private void HandleQueryEntityAttribute(GameQuery_EntityAttribute q)
        {
            foreach (Entity e in this.storedEntities)
            {
                e.HandleQuery(q);
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_EntityAttribute)
                this.HandleQueryEntityAttribute((GameQuery_EntityAttribute)q);

            return q;
        }
    }
}
