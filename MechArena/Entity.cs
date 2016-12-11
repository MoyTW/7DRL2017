using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena
{
	public class Entity
	{
		private Guid uuid;
        private List<Component> orderedComponents;

        public Entity() : this(Guid.NewGuid()) { }

        public Entity(Guid uuid)
        {
            this.uuid = uuid;
            this.orderedComponents = new List<Component>();
        }

        public IList<Component> InspectComponents()
        {
            return this.orderedComponents.AsReadOnly();
        }

        // Screw it I'm doin' it! RUNTIME TYPE CHECKS AHOY
        public bool HasComponentOfType<TComponent>() where TComponent : Component
        {
            return this.orderedComponents.Any(c => typeof(TComponent) == c.GetType());
        }

        // I mean...I'm basically porting a Python thing I wrote so. That doesn't make it okay I guess.
        // Write now figure out sanity later! Gotta make it by end-of-month!
        public TComponent GetComponentOfType<TComponent>() where TComponent : Component
        {
            return (TComponent)this.orderedComponents.FirstOrDefault(c => typeof(TComponent) == c.GetType());
        }

        // Note that ordering is controlled *only* by the order in which this is called!
        public Entity AddComponent(Component comp)
        {
            if (this.orderedComponents.Contains(comp))
            {
                throw new ArgumentException("Cannot add " + comp.ToString() + " to entity " + this.ToString() +
                    " as it is already added!");
            }

            this.orderedComponents.Add(comp);
            comp.Notify_Added(this);
            return this;
        }

        public Entity RemoveComponent(Component comp)
        {
            if (!this.orderedComponents.Contains(comp))
            {
                throw new ArgumentOutOfRangeException("Cannot remove " + comp.ToString() + " from entity " +
                    this.ToString() + " as it is not a component of it!");
            }

            this.orderedComponents.Remove(comp);
            comp.Notify_Removed();
            return this;
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

        public GameQuery HandleQuery(GameQuery q)
        {
            foreach (Component c in this.orderedComponents)
            {
                c.HandleQuery(q);
                if (q.Completed)
                    return q;
            }
            return q;
        }
	}
}

