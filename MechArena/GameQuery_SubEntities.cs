using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    public enum SubEntitiesSelector
    {
        ALL = 0,
        WEAPON,
        BODY_PART,
        TRACKS_TIME
    }

    // This is definitely astronauting the heck out of my problem.
    public class GameQuery_SubEntities : GameQuery
    {
        // TODO: Don't use Components for selectors! Instead use, "Can you handle this event/query?"
        public static readonly Dictionary<SubEntitiesSelector, List<Type>> SelectorsToComponents =
            new Dictionary<SubEntitiesSelector, List<Type>>() {
                { SubEntitiesSelector.ALL, null },
                { SubEntitiesSelector.WEAPON, new List<Type>() { typeof(Component_Weapon) } },
                { SubEntitiesSelector.BODY_PART, new List<Type>() { typeof(Component_BodyPartLocation) } },
                { SubEntitiesSelector.TRACKS_TIME, new List<Type>() { typeof(Component_TracksTime) } }
            };

        public static bool MatchesSelector(Entity en, SubEntitiesSelector s)
        {
            if (s == SubEntitiesSelector.ALL)
                return true;
            else
                return !SelectorsToComponents[s].Any(t => !en.HasComponentOfType(t));
        }

        private SubEntitiesSelector[] selectors;
        private List<Entity> subEntities;

        public IList<Entity> SubEntities { get { return this.subEntities.AsReadOnly(); } }

        public GameQuery_SubEntities(params SubEntitiesSelector [] selectors)
        {
            if (selectors.Count() == 0)
                throw new ArgumentException("No selectors sent to GameQuery_SubEntities! What'cha doin?");
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
