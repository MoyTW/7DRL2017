using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Executor
{
    [Serializable()]
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

        public Entity DeepCopy()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                return (Entity)formatter.Deserialize(stream);
            }
        }

        #endregion

        #region Convenience Queries

        public bool MatchesSelector(SubEntitiesSelector selector)
        {
            foreach (Component c in this.orderedComponents)
            {
                if (c.MatchingSelectors.Contains(selector))
                    return true;
            }
            return false;
        }

        public GameQuery_Position TryGetPosition()
        {
            return this.HandleQuery(new GameQuery_Position());
        }

        public GameQuery_EntityAttribute TryGetAttribute(EntityAttributeType attributeType)
        {
            return this.HandleQuery(new GameQuery_EntityAttribute(attributeType, this));
        }

        public GameQuery_EntityAttribute TryGetAttribute(EntityAttributeType attributeType, Entity baseEntity)
        {
            return this.HandleQuery(new GameQuery_EntityAttribute(attributeType, baseEntity));
        }

        public bool TryGetDestroyed()
        {
            return this.HandleQuery(new GameQuery_Destroyed()).Destroyed;
        }

        public IEnumerable<Entity> TryGetSubEntities(params SubEntitiesSelector[] selectors)
        {
            return this.HandleQuery(new GameQuery_SubEntities(selectors)).SubEntities;
        }

        public int TryGetTicksToLive(int currentTick)
        {
            return this.HandleQuery(new GameQuery_TicksToLive(currentTick)).TicksToLive;
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
            return this.orderedComponents.Any(c => this.HasComponentOfType(typeof(TComponent)));
        }

        public bool HasComponentOfType(Type tComponent)
        {
            // TODO: When switching out of Inheritance for subcomponents, this will need to be changed!
            // If instead you build a "resolver" this does not need to be changed.
            return this.orderedComponents.Any(c => c.GetType().IsSubclassOf(tComponent) || c.GetType() == tComponent);
        }

        // I mean...I'm basically porting a Python thing I wrote so. That doesn't make it okay I guess.
        // Write now figure out sanity later! Gotta make it by end-of-month!
        public TComponent GetComponentOfType<TComponent>() where TComponent : Component
        {
            return (TComponent)this.orderedComponents
                .FirstOrDefault(c => c.GetType().IsSubclassOf(typeof(TComponent)) || c.GetType() == typeof(TComponent));
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
            return this.Label;
        }
    }
}

