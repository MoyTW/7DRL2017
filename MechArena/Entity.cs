using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena
{
	public class Entity
	{
        #region Vars and Properties

        private List<Component> orderedComponents;

        public string EntityID { get; }
        public string Label { get; }
        public string TypeLabel { get; }

        #endregion

        #region Constructors

        public Entity(string entityID = null, string label = "", string typeLabel = "")
        {
            if (entityID == null)
                this.EntityID = Guid.NewGuid().ToString();
            this.Label = label;
            this.TypeLabel = typeLabel;

            this.orderedComponents = new List<Component>();
        }

        #endregion

        #region Convenience Queries

        public GameQuery_Position TryGetPosition()
        {
            return this.HandleQuery(new GameQuery_Position());
        }

        public GameQuery_EntityAttribute TryGetAttribute(EntityAttributeType attributeType)
        {
            return this.HandleQuery(new GameQuery_EntityAttribute(attributeType));
        }

        #endregion

        #region Component Functions

        public IList<Component> InspectComponents()
        {
            return this.orderedComponents.AsReadOnly();
        }

        // Screw it I'm doin' it! RUNTIME TYPE CHECKS AHOY
        public bool HasComponentOfType<TComponent>() where TComponent : Component
        {
            return this.orderedComponents.Any(c => typeof(TComponent) == c.GetType());
        }

        public bool HasComponentOfType(Type tComponent)
        {
            return this.orderedComponents.Any(c => tComponent == c.GetType());
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

        #endregion

        #region Event/Query Handlers

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

        public TQuery HandleQuery<TQuery>(TQuery q) where TQuery : GameQuery
        {
            foreach (Component c in this.orderedComponents)
            {
                c.HandleQuery(q);
                if (q.Completed)
                    return q;
            }
            return q;
        }

        #endregion

        public override string ToString()
        {
            return this.Label + " [" + this.TypeLabel + "]";
        }
    }
}

