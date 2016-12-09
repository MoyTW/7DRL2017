using System;
using System.Collections.Generic;

namespace MechArena
{
	public class Entity
	{
		private Guid uuid;
        private List<Component> orderedComponents;

        public Entity(Guid uuid)
        {
            this.uuid = uuid;
            this.orderedComponents = new List<Component>();
        }

        public IList<Component> InspectComponents()
        {
            return this.orderedComponents.AsReadOnly();
        }

        // Note that ordering is controlled *only* by the order in which this is called!
        public void AddComponent(Component comp)
        {
            if (this.orderedComponents.Contains(comp))
            {
                throw new ArgumentException("Cannot add " + comp.ToString() + " to entity " + this.ToString() +
                    " as it is already added!");
            }

            this.orderedComponents.Add(comp);
        }

        public void RemoveComponent(Component comp)
        {
            if (!this.orderedComponents.Contains(comp))
            {
                throw new ArgumentOutOfRangeException("Cannot remove " + comp.ToString() + " from entity " +
                    this.ToString() + " as it is not a component of it!");
            }

            this.orderedComponents.Remove(comp);
        }

        public bool HandleEvent(GameEvent ev)
        {
            foreach (Component c in this.orderedComponents)
            {
                GameEvent nextEvent = c.HandleEvent(ev);
                if (nextEvent.Completed)
                    return true;
            }
            return false;
        }
	}
}

