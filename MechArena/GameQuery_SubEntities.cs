using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public enum SubEntitiesSelector
    {
        WEAPON = 0,
        BODY_PART
    }

    // This is definitely astronauting the heck out of my problem.
    public class GameQuery_SubEntities : GameQuery
    {
        public static readonly Dictionary<SubEntitiesSelector, List<Type>> SelectorsToComponents =
            new Dictionary<SubEntitiesSelector, List<Type>>() {
                { SubEntitiesSelector.WEAPON, new List<Type>() { typeof(Component_Weapon) } },
                { SubEntitiesSelector.BODY_PART, new List<Type>() { typeof(Component_BodyPartLocation) } }
            };

        public static bool MatchesSelector(Entity en, SubEntitiesSelector s)
        {
            return !SelectorsToComponents[s].Any(t => !en.HasComponentOfType(t));
        }

        private SubEntitiesSelector[] selectors;
        private List<Entity> subEntities;

        public IList<Entity> SubEntities { get { return this.subEntities.AsReadOnly(); } }

        public GameQuery_SubEntities(params SubEntitiesSelector [] selectors)
        {
            this.subEntities = new List<Entity>();
            this.selectors = selectors;
        }

        public bool MatchesSelectors(Entity en)
        {
            return selectors.Any(s => MatchesSelector(en, s));
        }

        public void RegisterEntity(Entity en)
        {
            if (!this.MatchesSelectors(en))
                throw new ArgumentException("Attempted to register invalid item!");
            this.subEntities.Add(en);
        }
    }
}
